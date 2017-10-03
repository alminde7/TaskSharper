using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using TaskSharper.Domain.Calendar;
using TaskSharper.Shared.Extensions;

namespace TaskSharper.Notification
{
    public class EventNotification
    {
        public IList<int> NotificationOffsets { get; set; }
        public ConcurrentDictionary<string, IList<NotificationObject>> EventNotifications { get; set; }

        public EventNotification()
        {
            NotificationOffsets = new List<int>();
            EventNotifications = new ConcurrentDictionary<string, IList<NotificationObject>>();

            //TODO:: Specify these values in a configuration file of some sort?
            NotificationOffsets.Add(-15);
            NotificationOffsets.Add(-5);
            NotificationOffsets.Add(0);
            NotificationOffsets.Add(5);
            NotificationOffsets.Add(10);
            NotificationOffsets.Add(15);
        }

        public void Attach(Event calEvent, Action<Event> callback)
        {
            if (calEvent.Status == Event.EventStatus.Completed ||
                calEvent.Status == Event.EventStatus.Cancelled) return;

            var notificationList = new List<NotificationObject>();

            // Loop through specified notification intervals
            foreach (var notificationOffset in NotificationOffsets)
            {
                var obj = CreateNotification(calEvent, callback, calEvent.Start.Value.AddMinutes(notificationOffset));
                notificationList.Add(obj);
            }

            EventNotifications.AddOrUpdate(calEvent.Id, notificationList);
        }

        private NotificationObject CreateNotification(Event calEvent, Action<Event> callback, DateTime notificationTime)
        {
            var notObj = new NotificationObject();
            
            var data = CalculateTimeToFire(notificationTime);

            var timer = new Timer();
            if (!data.EventHasHappend)
            {
                timer.Interval = data.TimeToFire;
            }

            timer.AutoReset = false;
            timer.Elapsed += (sender, args) =>
            {
                callback(calEvent);
                timer.Close();
                notObj.HasFired = true;
            };

            return notObj;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="notificationTime"></param>
        /// <returns>Double in milliseconds</returns>
        private (double TimeToFire, bool EventHasHappend) CalculateTimeToFire(DateTime notificationTime)
        {
            var timeToFire = (notificationTime - DateTime.Now).TotalMilliseconds;
            return (Math.Abs(timeToFire), timeToFire > 0);
        }


        public void CleanUp()
        {
            foreach (var notification in EventNotifications)
            {
                for (int i = 0; i < notification.Value.Count; i++)
                {
                    if(notification.Value[i].HasFired)
                        notification.Value.RemoveAt(i);
                }
            }
        }
    }

    public class NotificationObject
    {
        public Timer Timer { get; set; }
        public bool HasFired { get; set; }
    }
}