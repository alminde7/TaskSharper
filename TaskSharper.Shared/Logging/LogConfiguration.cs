﻿using System;
using System.Reflection;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using TaskSharper.Shared.Configuration;

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
                    .WriteTo.Elasticsearch(
                        new ElasticsearchSinkOptions(new Uri(_elasticSearchUrl))
                        {
                            AutoRegisterTemplate = true
                        })
                    .MinimumLevel.Information()
                    .CreateLogger();
            return logger;
        }
    }
}