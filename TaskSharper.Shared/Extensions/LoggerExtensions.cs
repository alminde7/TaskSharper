using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;
using TaskSharper.Shared.Configuration;
using TaskSharper.Shared.Constants;

namespace TaskSharper.Shared.Extensions
{
    public static class LoggerExtensions
    {
        /// <summary>
        /// Add elasticsearch to logger configuration. 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
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

    /// <summary>
    /// Configure logger to log correlation id on each log.
    /// </summary>
    public class CorrelationIdEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            try
            {
                var correlationId = CallContext.LogicalGetData(HttpConstants.Header_CorrelationId) as string;

                if (string.IsNullOrWhiteSpace(correlationId))
                {
                    logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(HttpConstants.Header_CorrelationId, Guid.NewGuid().ToString()));
                }
                else
                {
                    logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(HttpConstants.Header_CorrelationId, correlationId));
                }
            }
            catch (Exception)
            {
                // DO nothing - dont screw up application with logging error
            }

        }
    }
}