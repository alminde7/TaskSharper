namespace TaskSharper.Domain.Configuration.Logging
{
    public class LoggingSettings
    {
        public LoggingSettings()
        {
            ElasticsearchConfig = new ElasticsearchConfig();
        }
        public bool EnableLoggingToFile { get; set; } = true;
        public bool EnableElasticsearchLogging { get; set; } = true;
        public ElasticsearchConfig ElasticsearchConfig { get; set; }
    }
}