using TaskSharper.Domain.Configuration.Cache;
using TaskSharper.Domain.Configuration.Notification;

namespace TaskSharper.Domain.Configuration.Service
{
    public class ServiceSettings
    {
        public ServiceSettings()
        {
            Cache = new CacheSettings();
            Notification = new NotificationSettings();
        }

        public CacheSettings Cache { get; set; }
        public NotificationSettings Notification { get; set; }
    }
}