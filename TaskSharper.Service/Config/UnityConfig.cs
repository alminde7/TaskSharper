﻿using System;
using System.Collections.Generic;
using Google.Apis.Calendar.v3;
using Google.Apis.Services;
using Microsoft.Practices.Unity;
using Serilog;
using TaskSharper.BusinessLayer;
using TaskSharper.CacheStore;
using TaskSharper.Configuration.Config;
using TaskSharper.DataAccessLayer.Google.Authentication;
using TaskSharper.DataAccessLayer.Google.Calendar.Service;
using TaskSharper.Domain.BusinessLayer;
using TaskSharper.Domain.Cache;
using TaskSharper.Domain.Calendar;
using TaskSharper.Domain.Notification;
using TaskSharper.Notification;
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
            var logSettings = LoggingConfig.Get();

            // Create logger
            var logger = LogConfiguration.ConfigureAPI(logSettings);
            Log.Logger = logger;
            
            // Create Google Authentication object
            var googleService = new CalendarService(new BaseClientService.Initializer()
            {
                ApplicationName = Constants.TaskSharper,
                HttpClientInitializer = new GoogleAuthentication(logger).Authenticate()
            });

            container.RegisterType<ICalendarService, GoogleCalendarService>();

            container.RegisterType<IEventManager, EventManager>(new TransientLifetimeManager());
            container.RegisterType<IEventCache, EventCache>(new ContainerControlledLifetimeManager());
            container.RegisterType<IEventCategoryCache, EventCategoriesCache>(new ContainerControlledLifetimeManager());
            container.RegisterType<INotificationPublisher, SignalRNotificationPublisher>();

            container.RegisterType<ILogger>(new ContainerControlledLifetimeManager(), new InjectionFactory((ctr, type, name) => LogConfiguration.ConfigureAPI(logSettings)));

            //Create Notification object
            var notificationObject = new EventNotification(new List<int>() { -15,-5,0,5,10,15 }, logger, container.Resolve<INotificationPublisher>());

            container.RegisterInstance(typeof(CalendarService), googleService);
            //container.RegisterInstance(typeof(ILogger), logger);
            container.RegisterInstance(typeof(INotification), notificationObject);
        }
    }
}
