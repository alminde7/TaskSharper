using TaskSharper.Configuration.Config;
using TaskSharper.Domain.Configuration;
using TaskSharper.Domain.Configuration.Service;

namespace TaskSharper.Configuration.Settings
{
    public class ServiceSettingsHandler : SettingsHandler<ServiceSettings>
    {
        public ServiceSettingsHandler()
        {
            FilePath = Shared.Configuration.Config.TaskSharperConfigStore + "/ServiceSettings.json";
        }
    }
}
