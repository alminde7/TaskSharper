using System;
using TaskSharper.Configuration.Settings;
using TaskSharper.Domain.Configuration;

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
