namespace TaskSharper.Domain.Configuration
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