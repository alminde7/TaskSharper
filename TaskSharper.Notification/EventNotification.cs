using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Timers;
using Serilog;
using TaskSharper.Domain.Calendar;
using TaskSharper.Domain.Notification;
using TaskSharper.Shared.Extensions;
using TaskSharper.Shared.Wrappers;

namespace TaskSharper.Notification
{
    public class EventNotification : INotification
    {
        public ILogger Logger { get; set; }
        public IEnumerable<int> NotificationOffsets { get; set; }
        public INotificationPublisher NotificationPublisher { get; }
        public ConcurrentDictionary<string, IList<NotificationObject>> EventNotifications { get; set; }

        private Timer DailyCleanUpTimer { get; set; }

        public EventNotification(IEnumerable<int> notificationOffsets, ILogger logger, INotificationPublisher notificationPublisher)
        {
            EventNotifications = new ConcurrentDictionary<string, IList<NotificationObject>>();
            Logger = logger.ForContext<EventNotification>();

            NotificationOffsets = notificationOffsets;
            NotificationPublisher = notificationPublisher;
            
            var timerbuilder = new TimerBuilder()
                .Create();
            timerbuilder
                .Create()
                .AutoReset()
                .SetDailyScheduler(new TimeSpan(0, 2, 0, 0), CleanUp)
                .Start();

            DailyCleanUpTimer = new Timer().StartDailyScheduler(new TimeSpan(0,2,0,0), CleanUp);
        }

        private void CleanUp()
        {
            try
            {
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

        public void Attach(Event calEvent)
        {
            // Remove event if it already exist - to ensure that updated events is removed from notifications. 
            if (EventNotifications.ContainsKey(calEvent.Id))
            {
                EventNotifications.TryRemove(calEvent.Id, out _);
            }

            // If event has been completed, do not add. 
            if (calEvent.Status == EventStatus.Completed ||  calEvent.Status == EventStatus.Cancelled) return;

            var notificationList = new List<NotificationObject>();
            
            if (NotificationOffsets != null)
            {
                foreach (var notificationOffset in NotificationOffsets)
                {
                    if (calEvent.Start.Value + TimeSpan.FromMinutes(notificationOffset) < DateTime.Now) continue; // Notification time is in the past - no reason to add notification
                    if (calEvent.Start.Value > DateTime.Now.AddDays(20)) continue; // later than 20 days in future is not allowed.

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

        public void Attach(IEnumerable<Event> calEvents)
        {
            foreach (var calEvent in calEvents)
            {
                Attach(calEvent);
            }
        }

        public void Detatch(string eventId)
        {
            if (EventNotifications.ContainsKey(eventId))
            {
                EventNotifications.TryRemove(eventId, out _);
            }
        }

        public void Detatch(IEnumerable<string> eventIds)
        {
            foreach (var eventId in eventIds)
            {
                Detatch(eventId);
            }
        }

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
        
        private double CalculateTimeToFire(DateTime notificationTime)
        {
            var timeToFire = (notificationTime - DateTime.Now).TotalMilliseconds;
            return Math.Abs(timeToFire);
        } // Calculated in milliseconds
    }
}