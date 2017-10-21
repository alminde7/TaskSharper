using System;
using System.Runtime.Remoting.Messaging;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using TaskSharper.Shared.Configuration;
using TaskSharper.Shared.Constants;

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

        public static LoggerConfiguration AddCorrelationId(this LoggerConfiguration logger)
        {
            var correlationId = CallContext.LogicalGetData(Http.Header_CorrelationId) as string;

            if (string.IsNullOrWhiteSpace(correlationId))
            {
                correlationId = Guid.NewGuid().ToString();
                CallContext.LogicalSetData(Http.Header_CorrelationId, correlationId);
            }

            logger.Enrich.WithProperty("CorrelationId", correlationId);

            return logger;
        }
    }
}