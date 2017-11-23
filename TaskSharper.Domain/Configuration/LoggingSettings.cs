namespace TaskSharper.Domain.Configuration
{
    public class LoggingSettings
    {
        public LoggingSettings()
        {
            ElasticsearchConfig = new ElasticsearchConfig();
        }
        public bool EnableLoggingToFile { get; set; } = true;
        public bool EnableElasticsearchLogging { get; set; } = true;
        public string MinimumLogLevel { get; set; } = "Information";
        public ElasticsearchConfig ElasticsearchConfig { get; set; }
    }
}