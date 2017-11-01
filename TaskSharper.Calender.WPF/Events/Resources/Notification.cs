using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TaskSharper.Calender.WPF.Properties;

namespace TaskSharper.Calender.WPF.Events.Resources
{
    public class Notification
    {
        public string Title { get; set; }
        public string Message { get; set; }

        public NotificationTypeEnum NotificationType { get; set; } 
    }
}
