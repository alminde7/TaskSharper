using System;
using TaskSharper.Configuration.Settings;
using TaskSharper.Domain.Configuration;
using TaskSharper.Domain.Configuration.Service;

namespace TaskSharper.Configuration.Config
{
    public class ServiceConfig
    {
        private static readonly Lazy<ServiceSettings> Settings = new Lazy<ServiceSettings>(() =>
        {
            var settingsHandler = new ServiceSettingsHandler();
            return settingsHandler.Load().Result;
        });

        public static ServiceSettings Get()
        {
            return Settings.Value;
        }
    }
}
