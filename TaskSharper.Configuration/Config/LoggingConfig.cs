using System;
using TaskSharper.Configuration.Settings;
using TaskSharper.Domain.Configuration;
using TaskSharper.Domain.Configuration.Logging;

namespace TaskSharper.Configuration.Config
{
    public class LoggingConfig
    {
        private static readonly Lazy<LoggingSettings> Settings = new Lazy<LoggingSettings>(() =>
        {
            var settingsHandler = new LoggingSettingsHandler();
            return settingsHandler.Load().Result;
        });

        public static LoggingSettings Get()
        {
            return Settings.Value;
        }
    }
}
