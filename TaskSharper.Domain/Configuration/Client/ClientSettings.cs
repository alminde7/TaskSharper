namespace TaskSharper.Domain.Configuration.Client
{
    public class ClientSettings
    {
        public string APIServerUrl { get; set; } = "http://localhost:8000/api/";
        public string NotificationServerUrl { get; set; } = "http://localhost:8000";
    }
}