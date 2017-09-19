using System;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using TaskSharper.Shared.Configuration;

namespace TaskSharper.Shared.Extensions
{
    public static class LoggerExtensions
    {
        public static LoggerConfiguration AddElasticsearch(this LoggerConfiguration logger, string connectionString)
        {
            var elasticsearchOptions = new ElasticsearchSinkOptions(new Uri(connectionString))
            {
                AutoRegisterTemplate = true,
                ConnectionTimeout = Config.ElasticsearchConnectionTimeout
            };

            logger.WriteTo.Elasticsearch(elasticsearchOptions);

            return logger;
        }
    }
}