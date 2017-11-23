using TaskSharper.Configuration.Config;
using TaskSharper.Domain.Configuration;

namespace TaskSharper.Configuration.Settings
{
    public class ClientSettingsHandler : SettingsHandler<ClientSettings>
    {
        public ClientSettingsHandler()
        {
            FilePath = Shared.Configuration.Config.TaskSharperConfigStore + "/ClientSettings.json";
        }
    }
}