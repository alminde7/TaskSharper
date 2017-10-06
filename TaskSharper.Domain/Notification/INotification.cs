using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskSharper.Domain.Calendar;

namespace TaskSharper.Domain.Notification
{
    public interface INotification
    {
        IEnumerable<int> NotificationOffsets { get; set; }
        Action<Event> Callback { get; set; }

        void Attach(Event calEvent);
        void Attach(IEnumerable<Event> calEvent);

        void Detatch(string eventId);
        void Detatch(IEnumerable<string> eventIds);
    }
}
