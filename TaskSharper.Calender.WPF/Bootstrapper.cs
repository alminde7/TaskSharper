using System;
using System.Collections.Generic;
using System.Windows;
using Google.Apis.Auth.OAuth2;
using Prism.Unity;
using Microsoft.Practices.Unity;
using Prism.Logging;
using Prism.Regions;
using Serilog;
using TaskSharper.Calender.WPF.Views;
using TaskSharper.DataAccessLayer.Google.Calendar.Service;
using TaskSharper.Domain.Calendar;
using TaskSharper.Shared.Logging;
using Google.Apis.Calendar.v3;
using Google.Apis.Services;
using TaskSharper.BusinessLayer;
using TaskSharper.CacheStore;
using TaskSharper.Calender.WPF.Config;
using TaskSharper.DataAccessLayer.Google;
using TaskSharper.DataAccessLayer.Google.Authentication;
using TaskSharper.Domain.BusinessLayer;
using TaskSharper.Domain.Cache;
using TaskSharper.Domain.Notification;
using TaskSharper.Notification;
using EventManager = TaskSharper.BusinessLayer.EventManager;

namespace TaskSharper.Calender.WPF
{
    public class Bootstrapper : UnityBootstrapper
    {
        private ILogger _logger;

        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void InitializeShell()
        {
            // TODO:: Think of a more sofisticated way of doing this. 
            Container.Resolve<IEventManager>().UpdateCacheStore(DateTime.Now.AddMonths(-3),DateTime.Now.AddMonths(3));

            Application.Current.MainWindow.Show();

            // Set default Calendar on start.
            var regionManager = Container.Resolve<IRegionManager>();
            regionManager.RequestNavigate(ViewConstants.REGION_Calendar, ViewConstants.VIEW_CalendarWeek);
        }
        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();

            Container.RegisterTypeForNavigation<CalendarDayView>(ViewConstants.VIEW_CalendarDay);
            Container.RegisterTypeForNavigation<CalendarWeekView>(ViewConstants.VIEW_CalendarWeek);
            Container.RegisterTypeForNavigation<CalendarMonthView>(ViewConstants.VIEW_CalendarMonth);
            Container.RegisterTypeForNavigation<CalendarEventDetailsView>(ViewConstants.VIEW_CalendarEventDetails);

            // Create logger
            var logger = LogConfiguration.Configure();
            _logger = logger;

            // Create Google Authentication object
            var googleService = new CalendarService(new BaseClientService.Initializer()
            {
                ApplicationName = Constants.TaskSharper,
                HttpClientInitializer = new GoogleAuthentication(logger).Authenticate()
            });

            //Create Notification object
            var notificationObject = new EventNotification(new List<int>(){-15,-5,0,5,10,15}, TempNotificationHandler);
            
            Container.RegisterInstance(typeof(CalendarService), googleService);
            Container.RegisterInstance(typeof(ILogger), logger);
            Container.RegisterInstance(typeof(INotification), notificationObject);

            Container.RegisterType<ICalendarService, GoogleCalendarService>();
            Container.RegisterType<IEventManager, EventManager>();
            Container.RegisterType<ICacheStore, EventCache>(new ContainerControlledLifetimeManager());
        }

        protected override ILoggerFacade CreateLogger()
        {
            return new SerilogLogger();
        }

        // TODO:: Implement correct event handling
        public void TempNotificationHandler(Event calEvent)
        {
            _logger?.ForContext("Notification", typeof(Bootstrapper)).Information("TEMP_Notification for event with id: {EventId}", calEvent.Id);
        }
    }

    public class SerilogLogger : ILoggerFacade
    {
        public void Log(string message, Category category, Priority priority)
        {
            switch (category)
            {
                case Category.Debug:
                    Serilog.Log.Logger.Debug(message);
                    break;
                case Category.Exception:
                    Serilog.Log.Logger.Error(message);
                    break;
                case Category.Info:
                    Serilog.Log.Logger.Information(message);
                    break;
                case Category.Warn:
                    Serilog.Log.Logger.Warning(message);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(category), category, null);
            }
        }
    }
}
