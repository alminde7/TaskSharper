using System;
using TaskSharper.Configuration.Settings;
using TaskSharper.Domain.Configuration;
using TaskSharper.Domain.Configuration.Logging;

namespace TaskSharper.Configuration.Config
{
    /// <summary>
    /// LoggingConfig used to get logging settings
    /// </summary>
    public class LoggingConfig
    {
        /// <summary>
        /// Settings is lazy loaded. Loaded first time when needed, and then returned. Singleton implementation
        /// </summary>
        private static readonly Lazy<LoggingSettings> Settings = new Lazy<LoggingSettings>(() =>
        {
            var settingsHandler = new LoggingSettingsHandler();
            return settingsHandler.Load();
        });

        /// <summary>
        /// Return logging settings
        /// </summary>
        /// <returns></returns>
        public static LoggingSettings Get()
        {
            return Settings.Value;
        }
    }
}
