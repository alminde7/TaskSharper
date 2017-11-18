using System.Collections.Generic;

namespace TaskSharper.Domain.Configuration
{
    public class LoggingConfiguration : ILoggingConfiguration
    {
        public LoggingConfiguration()
        {
            ElasticsearchConfiguration = new ElasticsearchConfig();
        }
        public bool LogLocal { get; set; }
        public bool LogElasticsearch { get; set; }
        public ElasticsearchConfig ElasticsearchConfiguration { get; set; }
    }

    public class ElasticsearchConfig
    {
        public ElasticsearchConfig()
        {
            Hosts = new List<string>();
            ElasticsearchAuthentication = new ElasticsearchAuthentication();
        }
        public IEnumerable<string> Hosts { get; set; }
        public bool EnableAuthentication { get; set; }

        public ElasticsearchAuthentication ElasticsearchAuthentication { get; set; }
    }

    public class ElasticsearchAuthentication
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
