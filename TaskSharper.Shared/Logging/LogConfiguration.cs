using System;
using System.Reflection;
using Elasticsearch.Net;
using Serilog;
using Serilog.Core;
using TaskSharper.Shared.Configuration;
using TaskSharper.Shared.Extensions;
using TaskSharper.Shared.Helpers;

namespace TaskSharper.Shared.Logging
{
    public class LogConfiguration
    {
        // NOTE TO SELF:: An enricher is called on every log event
        // https://nblumhardt.com/2016/08/context-and-correlation-structured-logging-concepts-in-net-5/

        private static string _elasticSearchUrl = $"http://{AppConfig.ElasticsearchHost}:{AppConfig.ElasticsearchPort}";
        
        public static ILogger ConfigureWPF()
        {
            return BaseConfig().CreateLogger();
        }

        public static ILogger ConfigureAPI()
        {
            var logger = BaseConfig();
            logger.Enrich.With(new CorrelationIdEnricher());

            return logger.CreateLogger();
        }

        private static LoggerConfiguration BaseConfig()
        {
            var applicationName = Assembly.GetCallingAssembly().GetName().Name;
            var machineName = Environment.MachineName;

            var logger = new LoggerConfiguration()
                .Enrich.WithProperty("MachineName", machineName)
                .Enrich.WithProperty("Application", applicationName)
                .Enrich.FromLogContext()
                .WriteTo.RollingFile($"{Config.TaskSharperLogStore}/log-{{Date}}.txt")
                .MinimumLevel.Information();

            if (ConnectionHelper.CheckConnectionElasticsearch(_elasticSearchUrl))
            {
                logger.AddElasticsearch(_elasticSearchUrl);
            }

            return logger;
        }
    }
}
