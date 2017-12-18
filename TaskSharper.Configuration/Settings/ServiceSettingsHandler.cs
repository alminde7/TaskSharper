using TaskSharper.Domain.Configuration.Service;

namespace TaskSharper.Configuration.Settings
{
    /// <summary>
    /// Handles path to ServiceSettings file
    /// </summary>
    public class ServiceSettingsHandler : SettingsHandler<ServiceSettings>
    {
        /// <summary>
        /// Constructor. Set the path to service settings
        /// </summary>
        public ServiceSettingsHandler()
        {
            FilePath = Shared.Configuration.Config.TaskSharperConfigStore + "/ServiceSettings.json";
        }
    }
}
