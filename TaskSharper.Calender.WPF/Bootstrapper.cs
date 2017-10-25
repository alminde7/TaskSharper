using System;
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
using TaskSharper.Domain.Notification;
using TaskSharper.Service.NotificationClient;
using TaskSharper.Service.NotificationClient.HubConnectionClient;
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

            var service = Container.Resolve<Service>();
            service.StartContinousService().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    // shit went wrong
                }
                else
                {
                    // shit went good
                }
            });
        }
        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();

            // Register views
            Container.RegisterTypeForNavigation<CalendarDayView>(ViewConstants.VIEW_CalendarDay);
            Container.RegisterTypeForNavigation<CalendarWeekView>(ViewConstants.VIEW_CalendarWeek);
            Container.RegisterTypeForNavigation<CalendarMonthView>(ViewConstants.VIEW_CalendarMonth);
            Container.RegisterTypeForNavigation<CalendarEventDetailsView>(ViewConstants.VIEW_CalendarEventDetails);

            // Register other dependencies
            var logger = LogConfiguration.ConfigureWPF();
            _logger = logger;
            
            // Singletons
            Container.RegisterInstance(typeof(ILogger), logger);
            Container.RegisterInstance(typeof(IRestClient), new RestClient());

            // Not singletons
            Container.RegisterType<IRestRequestFactory, RestRequestFactory>();
            Container.RegisterType<IEventRestClient, EventRestClient>();

            var hubConnectionClient = new HubConnectionProxy("http://localhost:8000");
            Container.RegisterInstance(typeof(IHubConnectionProxy), hubConnectionClient);
            Container.RegisterType<INotificationClient, NotificationClient>();

            Container.RegisterInstance(typeof(Service));
        }

        protected override ILoggerFacade CreateLogger()
        {
            return new SerilogLogger();
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
