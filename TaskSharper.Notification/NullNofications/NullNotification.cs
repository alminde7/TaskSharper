using System.Collections.Generic;
using TaskSharper.Domain.Calendar;
using TaskSharper.Domain.Configuration;
using TaskSharper.Domain.Configuration.Notification;
using TaskSharper.Domain.Models;
using TaskSharper.Domain.Notification;

namespace TaskSharper.Notification.NullNofications
{
    // NullObject pattern
    // Used when notifications is disabled. Instead of adding a bunch of if/else to the EventManager, 
    // making the code ugly, this implementation can be injected, thereby not doing notifications.
    /// <summary>
    /// NullNotification used to disable notifications
    /// </summary>
    public class NullNotification : INotification
    {
        public NotificationSettings NotificationSettings { get; set; }
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
