using System;
using TaskSharper.Domain.Calendar;

namespace TaskSharper.WPF.Common.Events.Resources
{
    public class Notification
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public DateTime? Start { get; set; }
        public EventStatus Status { get; set; }
        public EventType Type { get; set; }
        public NotificationTypeEnum NotificationType { get; set; }
        public EventCategory Category { get; set; }
    }
}
