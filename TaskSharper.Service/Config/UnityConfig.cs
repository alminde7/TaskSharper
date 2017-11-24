using System;
using Google.Apis.Calendar.v3;
using Google.Apis.Services;
using Microsoft.Practices.Unity;
using Serilog;
using TaskSharper.BusinessLayer;
using TaskSharper.CacheStore;
using TaskSharper.CacheStore.NullCache;
using TaskSharper.Configuration.Config;
using TaskSharper.DataAccessLayer.Google.Authentication;
using TaskSharper.DataAccessLayer.Google.Calendar.Service;
using TaskSharper.Domain.BusinessLayer;
using TaskSharper.Domain.Cache;
using TaskSharper.Domain.Calendar;
using TaskSharper.Domain.Configuration;
using TaskSharper.Domain.Configuration.Cache;
using TaskSharper.Domain.Configuration.Notification;
using TaskSharper.Domain.Notification;
using TaskSharper.Notification;
using TaskSharper.Notification.NullNofications;
using TaskSharper.Service.Hubs;
using TaskSharper.Shared.Logging;
using Constants = TaskSharper.DataAccessLayer.Google.Constants;

namespace TaskSharper.Service.Config
{
    // http://csharpindepth.com/articles/general/singleton.aspx#cctor
    public sealed class UnityConfig
    {
        private static readonly Lazy<IUnityContainer> Container = new Lazy<IUnityContainer>(() =>
        {
            var container = new UnityContainer();
            RegisterTypes(container);
            return container;
        });

        public static IUnityContainer GetContainer()
        {
            return Container.Value;
        }

        private static void RegisterTypes(IUnityContainer container)
        {
            // Get settings
            var logSettings = LoggingConfig.Get();
            var serviceSettings = ServiceConfig.Get();

            // Create logger and attach to global logger -> to enable attribute logging
            var logger = LogConfiguration.ConfigureAPI(logSettings);
            Log.Logger = logger;
            container.RegisterType<ILogger>(new ContainerControlledLifetimeManager(), new InjectionFactory((ctr, type, name) => LogConfiguration.ConfigureAPI(logSettings)));

            //container.RegisterInstance(typeof(CalendarService), googleService);
            container.RegisterType<CalendarService>(new TransientLifetimeManager(), new InjectionFactory(
                ctr => new CalendarService(new BaseClientService.Initializer
                {
                    ApplicationName = Constants.TaskSharper,
                    HttpClientInitializer = new GoogleAuthentication(logger).Authenticate()
                })));

            container.RegisterType<ICalendarService, GoogleCalendarService>();

            container.RegisterType<IEventManager, EventManager>(new TransientLifetimeManager());
            container.RegisterType<INotificationPublisher, SignalRNotificationPublisher>();
            
            RegisterCache(container, serviceSettings.Cache);
            RegisterNotification(container, serviceSettings.Notification);
        }

        private static void RegisterCache(IUnityContainer container, CacheSettings settings)
        {
            if (settings.EnableCache)
            {
                container.RegisterType<IEventCache, EventCache>(new ContainerControlledLifetimeManager());
                container.RegisterType<IEventCategoryCache, EventCategoriesCache>(new ContainerControlledLifetimeManager());
            }
            else
            {
                container.RegisterType<IEventCache, NullEventCache>(new ContainerControlledLifetimeManager());
                container.RegisterType<IEventCategoryCache, NullEventCategoryCache>(new ContainerControlledLifetimeManager());
            }
        }
            

        private static void RegisterNotification(IUnityContainer container, NotificationSettings settings)
        {
            if (settings.EnableNotifications)
            {
                var notificationObject = new EventNotification(settings, container.Resolve<ILogger>(), container.Resolve<INotificationPublisher>());
                container.RegisterInstance(typeof(INotification), notificationObject);
            }
            else
            {
                var notificationObject = new NullNotification();
                container.RegisterInstance(typeof(INotification), notificationObject);
            }
        }
    }
}
