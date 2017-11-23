using System;
using TaskSharper.Domain.Calendar;

namespace TaskSharper.WPF.Common.Events.Resources
{
    public class Notification
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public Event Event { get; set; }
        public NotificationTypeEnum NotificationType { get; set; }
    }
}
