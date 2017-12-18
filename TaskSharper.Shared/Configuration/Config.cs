using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskSharper.Shared.Configuration
{
    public class Config
    {
        public static readonly TimeSpan ElasticsearchConnectionTimeout = TimeSpan.FromMilliseconds(500);

        public static readonly string TaskSharperCredentialStore = SetupFileSystem.GetClientConnectionPath();

        public static readonly string TaskSharperLogStore = SetupFileSystem.GetLogPath();

        public static readonly string TaskSharperConfigStore = SetupFileSystem.GetConfigPath();

    }

    public class SetupFileSystem
    {
        private static string SetupBasePath()
        {
            var basePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "TaskSharper");

            if (!Directory.Exists(basePath))
            {
                Directory.CreateDirectory(basePath);
            }

            return basePath;
        }
        
        public static string GetClientConnectionPath()
        {
            var pathToCred = Path.Combine(SetupBasePath(), ".credentials");

            if (!Directory.Exists(pathToCred))
            {
                Directory.CreateDirectory(pathToCred);
            }

            return pathToCred;
        }

        public static string GetLogPath()
        {
            var pathToLog = Path.Combine(SetupBasePath(), "Logs");

            if (!Directory.Exists(pathToLog))
            {
                Directory.CreateDirectory(pathToLog);
            }

            return pathToLog;
        }

        public static string GetConfigPath()
        {
            var pathToConfig = Path.Combine(SetupBasePath(), "Config");

            if (!Directory.Exists(pathToConfig))
            {
                Directory.CreateDirectory(pathToConfig);
            }

            return pathToConfig;
        }
    }
}
