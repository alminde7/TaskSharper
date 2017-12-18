using System;
using TaskSharper.Configuration.Settings;
using TaskSharper.Domain.Configuration;
using TaskSharper.Domain.Configuration.Service;

namespace TaskSharper.Configuration.Config
{
    /// <summary>
    /// ServiceConfig used to get service settings
    /// </summary>
    public class ServiceConfig
    {
        /// <summary>
        /// Settings is lazy loaded. Loaded first time when needed, and then returned. Singleton implementation
        /// </summary>
        private static readonly Lazy<ServiceSettings> Settings = new Lazy<ServiceSettings>(() =>
        {
            var settingsHandler = new ServiceSettingsHandler();
            return settingsHandler.Load();
        });

        /// <summary>
        /// Return service settings
        /// </summary>
        /// <returns></returns>
        public static ServiceSettings Get()
        {
            return Settings.Value;
        }
    }
}
