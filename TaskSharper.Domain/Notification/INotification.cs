using System;
using System.Collections.Generic;
using TaskSharper.Domain.Calendar;

namespace TaskSharper.Domain.Notification
{
    public interface INotification
    {
        IEnumerable<int> NotificationOffsets { get; set; }

        void Attach(Event calEvent);
        void Attach(IEnumerable<Event> calEvent);

        void Detatch(string eventId);
        void Detatch(IEnumerable<string> eventIds);
    }
}
