using System;
using System.Configuration;

namespace TaskSharper.Shared.Configuration
{
    public class AppConfig
    {
        public static string ElasticsearchUser = GetAppsetting("ElkUsername");
        public static string ElasticsearchPassword = GetAppsetting("ElkPassword");
        public static string ElasticsearchHost = GetAppsetting("ElasticsearchHost");
        public static string ElasticsearchPort = GetAppsetting("ElasticsearchPort");

        public static bool ElkAuthIsActivated = Convert.ToBoolean(GetAppsetting("ElkAutenthenticationActivated"));

        public static string GetAppsetting(string key)
        {
            var value = ConfigurationManager.AppSettings[key];

            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException($"Please add {key} to App.config");
            }
            return value;
        }
    }
}
