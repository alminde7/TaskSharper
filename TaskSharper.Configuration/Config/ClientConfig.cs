using System;
using TaskSharper.Configuration.Settings;
using TaskSharper.Domain.Configuration;
using TaskSharper.Domain.Configuration.Client;

namespace TaskSharper.Configuration.Config
{
    /// <summary>
    /// ClientConfig used to get client settings
    /// </summary>
    public sealed class ClientConfig
    {
        /// <summary>
        /// Settings is lazy loaded. Loaded first time when needed, and then returned. Singleton implementation
        /// </summary>
        private static readonly Lazy<ClientSettings> Settings = new Lazy<ClientSettings>(() =>
        {
            var settingsLoader = new ClientSettingsHandler();
            return settingsLoader.Load();
        });

        /// <summary>
        /// Return settings
        /// </summary>
        /// <returns></returns>
        public static ClientSettings Get()
        {
            return Settings.Value;
        }
    }
}
