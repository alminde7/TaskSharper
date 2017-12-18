using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Timers;
using Serilog;
using TaskSharper.Domain.Calendar;
using TaskSharper.Domain.Configuration.Notification;
using TaskSharper.Domain.Models;
using TaskSharper.Domain.Notification;
using TaskSharper.Shared.Extensions;

namespace TaskSharper.Notification
{
    /// <summary>
    /// Handles reminders for events. 
    /// </summary>
    public class EventNotification : INotification
    {
        public ILogger Logger { get; set; }
        public NotificationSettings NotificationSettings { get; set; }
        public INotificationPublisher NotificationPublisher { get; }

        /// <summary>
        /// Datastructure that hold the notification data.
        /// </summary>
        public ConcurrentDictionary<string, IList<NotificationObject>> EventNotifications { get; set; }

        /// <summary>
        /// Timer used to trigger a dailt cleanup of notification, every day. This is to removed already fired notification.
        /// </summary>
        private Timer DailyCleanUpTimer { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="settings">Settings object from settings file. Used to configure notification behaviour</param>
        /// <param name="logger"></param>
        /// <param name="notificationPublisher">NotificationPublisher used to publish events</param>
        public EventNotification(NotificationSettings settings, ILogger logger, INotificationPublisher notificationPublisher)
        {
            EventNotifications = new ConcurrentDictionary<string, IList<NotificationObject>>();
            Logger = logger.ForContext<EventNotification>();

            NotificationSettings = settings;
            NotificationPublisher = notificationPublisher;
            
            DailyCleanUpTimer = new Timer()
                .SetDailyScheduler(new TimeSpan(0,2,0,0), CleanUp)
                .StartTimer();
        }

        /// <summary>
        /// Clean of the notification datastructure for events that is in the past or events that have been triggered
        /// </summary>
        private void CleanUp()
        {
            try
            {
                Logger.ForContext("CleanUp", typeof(EventNotification)).Information("Doing daily cleaning for notifications");
                foreach (var eventNotification in EventNotifications)
                {
                    foreach (var notificationObject in eventNotification.Value)
                    {
                        if (notificationObject.HasFired)
                        {
                            eventNotification.Value.Remove(notificationObject);
                        }
                    }

                    if (eventNotification.Value.Count <= 0)
                    {
                        EventNotifications.TryRemove(eventNotification.Key, out _);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.ForContext("CleanUp", typeof(EventNotification)).Error(e, "Error while doing daily cleaning of notification");
            }
        }

        /// <summary>
        /// Add notification. Will ignore it if the event is in the past or if the event has been completed.
        /// </summary>
        /// <param name="calEvent">The event that need reminder</param>
        public void Attach(Event calEvent)
        {
            // Remove event if it already exist - to ensure that updated events is removed from notifications. 
            if (EventNotifications.ContainsKey(calEvent.Id))
            {
	            EventNotifications.TryRemove(calEvent.Id, out var obj);
	            foreach (var notificationObject in obj)
		            notificationObject.Timer.Dispose();
            }

            // If event has been completed, do not add. 
            if (calEvent.Status == EventStatus.Completed || calEvent.Status == EventStatus.Cancelled || calEvent.MarkedAsDone) return;

            // Check whether notification for the specific event type is enabled
            if (!IsNotificationsEnabled(calEvent, NotificationSettings)) return;

            var notificationList = new List<NotificationObject>();

            // Get notification offsets based on the event type.
            var notificationOffsets = GetNotificationOffsetsForEventType(calEvent.Type, NotificationSettings);
            
            if (notificationOffsets != null)
            {
                foreach (var notificationOffset in notificationOffsets)
                {
                    if (NotificationIsInThePast(calEvent.Start.Value + TimeSpan.FromMinutes(notificationOffset))) continue;

                    if (NotificationIsToLongInTheFuture(calEvent.Start.Value)) continue; 

                    var obj = CreateNotification(calEvent, calEvent.Start.Value.AddMinutes(notificationOffset));
                    notificationList.Add(obj);
                }
            }
            else // No Notification offsets provided - notify at start time of event
            {
                notificationList.Add(CreateNotification(calEvent, calEvent.Start.Value));
            }

            EventNotifications.AddOrUpdate(calEvent.Id, notificationList);
        }

        /// <summary>
        /// Add a list of notifications
        /// </summary>
        /// <param name="calEvents">The events that need reminders</param>
        public void Attach(IEnumerable<Event> calEvents)
        {
            foreach (var calEvent in calEvents)
            {
                Attach(calEvent);
            }
        }

        /// <summary>
        /// Remove a notification
        /// </summary>
        /// <param name="eventId">The event to be removed from notification</param>
        public void Detatch(string eventId)
        {
            if (EventNotifications.ContainsKey(eventId))
            {
                EventNotifications.TryRemove(eventId, out var obj);
                foreach (var notificationObject in obj)
                    notificationObject.Timer.Dispose();
            }
        }

        /// <summary>
        /// Remove a list of notifications
        /// </summary>
        /// <param name="eventIds">The events to be removed from notification</param>
        public void Detatch(IEnumerable<string> eventIds)
        {
            foreach (var eventId in eventIds)
            {
                Detatch(eventId);
            }
        }

        /// <summary>
        /// Create the Timer object as configure the callback on timer trigger
        /// </summary>
        /// <param name="calEvent"></param>
        /// <param name="notificationTime"></param>
        /// <returns></returns>
        private NotificationObject CreateNotification(Event calEvent, DateTime notificationTime)
        {
            var notObj = new NotificationObject();
            var data = CalculateTimeToFire(notificationTime); // Calculate in milliseconds the time to fire the notification

            // Initialize timer
            var timer = new Timer();
            timer.Interval = data;
            timer.AutoReset = false;
            timer.Elapsed += (sender, args) =>
            {
                NotificationPublisher.Publish(calEvent);
                timer.Close();
                notObj.HasFired = true;
            };
            timer.Start();
            notObj.Timer = timer;
            return notObj;
        }
        
        /// <summary>
        /// Calculates the until the notification
        /// </summary>
        /// <param name="notificationTime"></param>
        /// <returns>Time to notification in milliseconds</returns>
        private double CalculateTimeToFire(DateTime notificationTime)
        {
            var timeToFire = (notificationTime - DateTime.Now).TotalMilliseconds;
            return Math.Abs(timeToFire);
        } // Calculated in milliseconds

        private List<int> GetNotificationOffsetsForEventType(EventType type, NotificationSettings settings)
        {
            switch (type)
            {
                case EventType.None:
                    return settings.NoneType.NotificationOffsets;
                case EventType.Appointment:
                    return settings.Appointments.NotificationOffsets;
                case EventType.Task:
                    return settings.Tasks.NotificationOffsets;
                default:
                    return null;
            }
        }

        private bool NotificationIsInThePast(DateTime notificationTime)
        {
            return notificationTime < DateTime.Now;
        }

        private bool NotificationIsToLongInTheFuture(DateTime notificationTime)
        {
            // Hardcoded maximum of 20 days because of the limitation of an int. 
            // Could use long, but a user will properbly use the application
            // at some point during the 20 days, which mean that the notification
            // will be updated.
            return notificationTime > DateTime.Now.AddDays(20);
        }

        private bool IsNotificationsEnabled(Event calEvent, NotificationSettings setting)
        {
            switch (calEvent.Type)
            {
                case EventType.None:
                    return setting.NoneType.EnableNoneTypetNotifications;
                case EventType.Appointment:
                    return setting.Appointments.EnableAppointmentNotifications;
                case EventType.Task:
                    return setting.Tasks.EnableTaskNotifications;
                default:
                    return true;
            }
        }
    }

}