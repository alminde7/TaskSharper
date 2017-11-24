using System.Collections.Generic;
using TaskSharper.Domain.Calendar;
using TaskSharper.Domain.Notification;

namespace TaskSharper.Notification.NullNofications
{
    // Used when cache is disabled. Instead of adding a bunch of if/else to the EventManager, 
    // making the code ugly, this implementation can be injected, thereby forcing nothing to be cached.
    public class NullNotification : INotification
    {
        public IEnumerable<int> NotificationOffsets { get; set; }
        public void Attach(Event calEvent)
        {
            
        }

        public void Attach(IEnumerable<Event> calEvent)
        {
            
        }

        public void Detatch(string eventId)
        {
            
        }

        public void Detatch(IEnumerable<string> eventIds)
        {
            
        }
    }
}
