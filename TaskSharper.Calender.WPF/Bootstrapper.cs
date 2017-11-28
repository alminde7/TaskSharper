using System;
using System.Windows;
using Prism.Unity;
using Microsoft.Practices.Unity;
using Prism.Events;
using Prism.Logging;
using Prism.Regions;
using RestSharp;
using Serilog;
using TaskSharper.Calender.WPF.Views;
using TaskSharper.Domain.Calendar;
using TaskSharper.Shared.Logging;
using TaskSharper.Calender.WPF.Config;
using TaskSharper.Configuration.Config;
using TaskSharper.Domain.Notification;
using TaskSharper.Service.NotificationClient;
using TaskSharper.Service.NotificationClient.HubConnectionClient;
using TaskSharper.Service.RestClient.Clients;
using TaskSharper.Service.RestClient.Factories;
using TaskSharper.WPF.Common.Components.EventModification;

namespace TaskSharper.Calender.WPF
{
    public class Bootstrapper : UnityBootstrapper
    {
        private ILogger _logger;

        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override async void InitializeShell()
        {
            var service = Container.Resolve<NotificationService>();
            await service.StartContinousService();

            Application.Current.MainWindow.Show();

            // Set default Calendar on start.
            var regionManager = Container.Resolve<IRegionManager>();
            regionManager.RequestNavigate(ViewConstants.REGION_Calendar, ViewConstants.VIEW_CalendarWeek);
        }
        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();

            var logSettings = LoggingConfig.Get();
            var clientSettings = ClientConfig.Get();

            //EventAggregator
            var singletonEventAggregator = new EventAggregator();
            Container.RegisterInstance(typeof(IEventAggregator), singletonEventAggregator,new ContainerControlledLifetimeManager());

            // Register views
            Container.RegisterTypeForNavigation<CalendarDayView>(ViewConstants.VIEW_CalendarDay);
            Container.RegisterTypeForNavigation<CalendarWeekView>(ViewConstants.VIEW_CalendarWeek);
            Container.RegisterTypeForNavigation<CalendarMonthView>(ViewConstants.VIEW_CalendarMonth);
            Container.RegisterTypeForNavigation<CalendarEventShowDetailsView>(ViewConstants.VIEW_CalendarEventShowDetails);
            Container.RegisterTypeForNavigation<EventModificationView>(ViewConstants.VIEW_CalendarEventDetails);

            // Register other dependencies
            var logger = LogConfiguration.ConfigureWPF(logSettings);
            _logger = logger;
            
            // Singletons
            Container.RegisterInstance(typeof(ILogger), logger);
            Container.RegisterInstance(typeof(IRestClient), new RestClient());
           

            // Not singletons
            Container.RegisterType<IRestRequestFactory, RestRequestFactory>();
            Container.RegisterType<IEventRestClient, EventRestClient>(new InjectionConstructor(clientSettings.APIServerUrl, "events", typeof(IRestClient), typeof(IRestRequestFactory), typeof(ILogger)));
            Container.RegisterType<IStatusRestClient, StatusRestClient>(new InjectionConstructor(clientSettings.APIServerUrl, typeof(IRestClient), typeof(IRestRequestFactory), typeof(ILogger)));

            var hubConnectionClient = new HubConnectionProxy(clientSettings.NotificationServerUrl);
            Container.RegisterInstance(typeof(IHubConnectionProxy), hubConnectionClient);
            Container.RegisterType<INotificationClient, NotificationClient>();

            Container.RegisterInstance(typeof(NotificationService));
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
