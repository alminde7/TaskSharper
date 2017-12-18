using TaskSharper.Domain.Configuration.Logging;

namespace TaskSharper.Configuration.Settings
{
    /// <summary>
    /// Handles path to LoggingSettings file
    /// </summary>
    public class LoggingSettingsHandler : SettingsHandler<LoggingSettings>
    {
        /// <summary>
        /// Constructor. Set the path to logging settings file
        /// </summary>
        public LoggingSettingsHandler()
        {
            FilePath = Shared.Configuration.Config.TaskSharperConfigStore + "/LoggingSettings.json";
        }
    }
}
