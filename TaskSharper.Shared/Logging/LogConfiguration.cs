using System;
using System.Reflection;
using Elasticsearch.Net;
using Serilog;
using Serilog.Core;
using TaskSharper.Shared.Configuration;
using TaskSharper.Shared.Extensions;

namespace TaskSharper.Shared.Logging
{
    public class LogConfiguration
    {
        private static string _elasticSearchUrl = $"http://{AppConfig.ElasticsearchHost}:{AppConfig.ElasticsearchPort}";

        public static ILogger Configure()
        {
            var applicationName = Assembly.GetExecutingAssembly().GetName().Name;
            var machineName = Environment.MachineName;

            var logger =
                new LoggerConfiguration()
                    .Enrich.WithProperty("MachineName", machineName)
                    .Enrich.WithProperty("Application", applicationName)
                    .AddElasticsearch(_elasticSearchUrl)
                    .MinimumLevel.Information()
                    .CreateLogger();
            return logger;
        }
    }
}
