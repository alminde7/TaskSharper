using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using TaskSharper.Domain.Calendar;
using TaskSharper.Domain.Notification;
using TaskSharper.Shared.Extensions;

namespace TaskSharper.Notification
{
    public class EventNotification : INotification
    {
        public Action<Event> Callback { get; set; }
        public IEnumerable<int> NotificationOffsets { get; set; }
        public ConcurrentDictionary<string, IList<NotificationObject>> EventNotifications { get; set; }

        public EventNotification(IEnumerable<int> notificationOffsets, Action<Event> notificationCallback)
        {
            EventNotifications = new ConcurrentDictionary<string, IList<NotificationObject>>();

            NotificationOffsets = notificationOffsets;
            Callback = notificationCallback;
        }

        public void Attach(Event calEvent)
        {
            if (calEvent.Status == Event.EventStatus.Completed ||  calEvent.Status == Event.EventStatus.Cancelled) return;

            var notificationList = new List<NotificationObject>();
            
            if (NotificationOffsets != null)
            {
                foreach (var notificationOffset in NotificationOffsets)
                {
                    if (calEvent.Start.Value + TimeSpan.FromMinutes(notificationOffset) < DateTime.Now) continue; // Notification time is in the past - no reason to add notification

                    var obj = CreateNotification(calEvent, calEvent.Start.Value.AddMinutes(notificationOffset));
                    notificationList.Add(obj);
                }
            }
            else // No Notification offsets provided
            {
                notificationList.Add(CreateNotification(calEvent, calEvent.Start.Value));
            }

            EventNotifications.AddOrUpdate(calEvent.Id, notificationList);
        }

        public void Attach(IEnumerable<Event> calEvents)
        {
            foreach (var calEvent in calEvents)
            {
                if (calEvent.Status == Event.EventStatus.Completed || calEvent.Status == Event.EventStatus.Cancelled) continue;

                var notificationList = new List<NotificationObject>();

                if (NotificationOffsets != null)
                {
                    foreach (var notificationOffset in NotificationOffsets)
                    {
                        if (calEvent.Start.Value + TimeSpan.FromMinutes(notificationOffset) < DateTime.Now) continue; // Notification time is in the past - no reason to add notification

                        var obj = CreateNotification(calEvent, calEvent.Start.Value.AddMinutes(notificationOffset));
                        notificationList.Add(obj);
                    }
                }
                else // No Notification offsets provided
                {
                    notificationList.Add(CreateNotification(calEvent, calEvent.Start.Value));
                }

                EventNotifications.AddOrUpdate(calEvent.Id, notificationList);
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
                if (EventNotifications.ContainsKey(eventId))
                {
                    EventNotifications.TryRemove(eventId, out _);
                }
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
                Callback(calEvent);
                timer.Close();
                notObj.HasFired = true;
            };
            timer.Start();

            return notObj;
        }
        
        private double CalculateTimeToFire(DateTime notificationTime)
        {
            var timeToFire = (notificationTime - DateTime.Now).TotalMilliseconds;
            return Math.Abs(timeToFire);
        } // Calculated in milliseconds
    }
}