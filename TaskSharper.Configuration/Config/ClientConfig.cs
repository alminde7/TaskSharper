﻿using System;
using TaskSharper.Configuration.Settings;
using TaskSharper.Domain.Configuration;

namespace TaskSharper.Configuration.Config
{
    public sealed class ClientConfig
    {
        private static readonly Lazy<ClientSettings> Settings = new Lazy<ClientSettings>(() =>
        {
            var settingsLoader = new ClientSettingsHandler();
            return settingsLoader.Load().Result;
        });

        public static ClientSettings Get()
        {
            return Settings.Value;
        }
    }
}