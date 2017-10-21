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
        private static string _elasticSearchUrl = $"http://{AppConfig.ElasticsearchHost}:{AppConfig.ElasticsearchPort}";

        public static ILogger Configure()
        {
            var applicationName = Assembly.GetCallingAssembly().GetName().Name;
            var machineName = Environment.MachineName;

            var logger =
                new LoggerConfiguration()
                    .Enrich.WithProperty("MachineName", machineName)
                    .Enrich.WithProperty("Application", applicationName)
                    .WriteTo.RollingFile($"{Config.TaskSharperLogStore}/log-{{Date}}.txt")
                    .MinimumLevel.Information();

            if (ConnectionHelper.CheckConnectionElasticsearch(_elasticSearchUrl))
            {
                logger.AddElasticsearch(_elasticSearchUrl);
            }

            logger.AddCorrelationId();

            return logger.CreateLogger();
        }
    }
}
