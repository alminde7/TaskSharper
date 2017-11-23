using TaskSharper.Configuration.Config;
using TaskSharper.Domain.Configuration;

namespace TaskSharper.Configuration.Settings
{
    public class LoggingSettingsHandler : SettingsHandler<LoggingSettings>
    {
        public LoggingSettingsHandler()
        {
            FilePath = Shared.Configuration.Config.TaskSharperConfigStore + "/LoggingSettings.json";
        }
    }
}
