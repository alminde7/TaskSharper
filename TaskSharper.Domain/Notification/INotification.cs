using System;
using System.Collections.Generic;
using TaskSharper.Domain.Calendar;
using TaskSharper.Domain.Configuration;
using TaskSharper.Domain.Configuration.Notification;

namespace TaskSharper.Domain.Notification
{
    public interface INotification
    {
        NotificationSettings NotificationSettings { get; set; }

        void Attach(Event calEvent);
        void Attach(IEnumerable<Event> calEvent);

        void Detatch(string eventId);
        void Detatch(IEnumerable<string> eventIds);
    }
}
