using System;
using TaskSharper.Configuration.Settings;
using TaskSharper.Domain.Configuration;
using TaskSharper.Domain.Configuration.Client;

namespace TaskSharper.Configuration.Config
{
    public sealed class ClientConfig
    {
        private static readonly Lazy<ClientSettings> Settings = new Lazy<ClientSettings>(() =>
        {
            var settingsLoader = new ClientSettingsHandler();
            return settingsLoader.Load();
        });

        public static ClientSettings Get()
        {
            return Settings.Value;
        }
    }
}
