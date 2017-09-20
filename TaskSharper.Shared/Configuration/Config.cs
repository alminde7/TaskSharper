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
    }
}
