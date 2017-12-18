
using TaskSharper.Domain.Configuration;
using TaskSharper.Domain.Configuration.Client;

namespace TaskSharper.Configuration.Settings
{
    /// <summary>
    /// Handles path to ClientSettings file
    /// </summary>
    public class ClientSettingsHandler : SettingsHandler<ClientSettings>
    {
        /// <summary>
        /// Constructor. Sets the path to client settings
        /// </summary>
        public ClientSettingsHandler()
        {
            FilePath = Shared.Configuration.Config.TaskSharperConfigStore + "/ClientSettings.json";
        }
    }
}