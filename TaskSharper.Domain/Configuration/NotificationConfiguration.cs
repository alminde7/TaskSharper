using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskSharper.Domain.Configuration
{
    public class NotificationConfiguration : INotificationConfiguration
    {
        public NotificationConfiguration()
        {
            //NotificaitInformations = new Dictionary<string, NotificationCategoryInformation>();
            NotificationOffsets = new List<int>();
        }

        public bool IsEnabled { get; set; }
        public IList<int> NotificationOffsets { get; set; }

        //public Dictionary<string, NotificationCategoryInformation> NotificaitInformations { get; set; }
    }

    public class NotificationCategoryInformation
    {
        public NotificationCategoryInformation()
        {
            NotificationOffsets = new List<int>();
        }
        public bool IsEnabled { get; set; }
        public IList<int> NotificationOffsets { get; set; }
    }
}
