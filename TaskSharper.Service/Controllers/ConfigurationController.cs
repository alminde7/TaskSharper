using System;
using System.IO;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNet.SignalR.Json;
using Microsoft.Practices.Unity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TaskSharper.Domain.Configuration;
using TaskSharper.Service.Config;

namespace TaskSharper.Service.Controllers
{
    public class ConfigurationController : ApiController
    {
        private readonly LoggingConfiguration _loggingConfig;
        private readonly NotificationConfiguration _notificationConfiguration;
        private readonly CacheConfiguration _cacheConfiguration;

        public ConfigurationController(ICacheConfiguration cacheConfig, ILoggingConfiguration loggingConfig, INotificationConfiguration notificationInformation)
        {
            _loggingConfig = loggingConfig as LoggingConfiguration;
            _notificationConfiguration = notificationInformation as NotificationConfiguration;
            _cacheConfiguration = cacheConfig as CacheConfiguration;
        }

        [HttpGet]
        public async Task<IHttpActionResult> Get()
        {
            var repo = new ConfigurationRepository();
            var data = await repo.Load();

            var viewModel = new ConfigurationViewModel()
            {
                LoggingConfiguration = _loggingConfig,
                NotificationConfiguration = _notificationConfiguration,
                CacheConfiguration = _cacheConfiguration
            };

            return Ok(data);
        }

        [HttpPost]
        public IHttpActionResult Post(ConfigurationViewModel newConfig)
        {
            try
            {
                if (!ValidateConfiguration())
                {
                    return BadRequest();
                }

                UnityConfig.GetContainer().RegisterInstance(typeof(ICacheConfiguration), newConfig.CacheConfiguration);
                UnityConfig.GetContainer().RegisterInstance(typeof(ILoggingConfiguration), newConfig.LoggingConfiguration);
                UnityConfig.GetContainer().RegisterInstance(typeof(INotificationConfiguration), newConfig.NotificationConfiguration);

                var repo = new ConfigurationRepository();
                repo.Save(new ConfigurationModel()
                {
                    CacheConfiguration = newConfig.CacheConfiguration,
                    LoggingConfiguration = newConfig.LoggingConfiguration,
                    NotificationConfiguration = newConfig.NotificationConfiguration
                });

                return Ok();
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }

        private bool ValidateConfiguration()
        {
            return true;
        }
    }

    public class ConfigurationRepository
    {
        public void Save(ConfigurationModel model)
        {
            string path = @"C:\Users\Alminde\Documents\TaskSharper\Config\config.json";

            using (StreamWriter file = File.CreateText(path))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, model);
            }
        }

        public async Task<ConfigurationModel> Load()
        {
            ConfigurationModel model;

            string path = @"C:\Users\Alminde\Documents\TaskSharper\Config\config.json";

            using (StreamReader file = File.OpenText(path))
            {
                var jsonString = await file.ReadToEndAsync();
                model = JsonConvert.DeserializeObject<ConfigurationModel>(jsonString);
            }
            return model;
        }
    }

    public class ConfigurationViewModel
    {
        public LoggingConfiguration LoggingConfiguration { get; set; }
        public NotificationConfiguration NotificationConfiguration { get; set; }
        public CacheConfiguration CacheConfiguration { get; set; }
    }

    public class ConfigurationModel
    {
        public LoggingConfiguration LoggingConfiguration { get; set; }
        public NotificationConfiguration NotificationConfiguration { get; set; }
        public CacheConfiguration CacheConfiguration { get; set; }
    }
}
