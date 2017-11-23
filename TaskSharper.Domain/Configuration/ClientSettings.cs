namespace TaskSharper.Domain.Configuration
{
    public class ClientSettings
    {
        public string ResourceServerUrl { get; set; } = "http://localhost:8000";
        public string NotificationServerUrl { get; set; } = "http://localhost:8000";
    }
}