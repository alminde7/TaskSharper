using System;
using System.Reflection;
using Elasticsearch.Net;
using Serilog;
using Serilog.Core;
using TaskSharper.Domain.Configuration;
using TaskSharper.Shared.Configuration;
using TaskSharper.Shared.Extensions;
using TaskSharper.Shared.Helpers;

namespace TaskSharper.Shared.Logging
{
    public class LogConfiguration
    {
        // NOTE TO SELF:: An enricher is called on every log event

        private static string _elasticSearchUrl = $"http://{AppConfig.ElasticsearchHost}:{AppConfig.ElasticsearchPort}";
        
        public static ILogger ConfigureWPF(LoggingSettings settings)
        {
            return BaseConfig(settings).CreateLogger();
        }

        public static ILogger ConfigureAPI(LoggingSettings settings)
        {
            var logger = BaseConfig(settings);
            logger.Enrich.With(new CorrelationIdEnricher());
            logger.WriteTo.Console(); // Enables insight into Service without the need to Kibana - primarily for development purposes

            return logger.CreateLogger();
        }

        private static LoggerConfiguration BaseConfig(LoggingSettings settings)
        {
            var applicationName = Assembly.GetCallingAssembly().GetName().Name;
            var machineName = Environment.MachineName;

            var logger = new LoggerConfiguration()
                .Enrich.WithProperty("MachineName", machineName)
                .Enrich.WithProperty("Application", applicationName)
                .Enrich.FromLogContext()
                .MinimumLevel.Information();

            if (settings.EnableLoggingToFile)
            {
                logger.WriteTo.RollingFile($"{Config.TaskSharperLogStore}/log-{{Date}}.txt");
            }

            if (settings.EnableElasticsearchLogging)
            {
                logger.AddElasticsearch(settings.ElasticsearchConfig.Url);
            }

            return logger;
        }
    }
}
