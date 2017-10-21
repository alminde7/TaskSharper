using System;
using System.ComponentModel;
using System.Windows;
using Prism.Unity;
using Microsoft.Practices.Unity;
using Prism.Logging;
using Prism.Regions;
using RestSharp;
using Serilog;
using TaskSharper.Calender.WPF.Views;
using TaskSharper.Domain.Calendar;
using TaskSharper.Shared.Logging;
using TaskSharper.Calender.WPF.Config;
using TaskSharper.Service.RestClient;
using TaskSharper.Service.RestClient.Factories;

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
            
            Container.RegisterInstance(typeof(ILogger), logger);

            //Create restsclientobject
            var restClient = new RestClient();
            Container.RegisterInstance(typeof(IRestClient), restClient);
            Container.RegisterType<IRestRequestFactory, RestRequestFactory>();
            Container.RegisterType<IEventRestClient, EventRestClient>();
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
