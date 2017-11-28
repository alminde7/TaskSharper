using System;
using System.Windows;
using Prism.Regions;
using Prism.Unity;
using TaskSharper.Domain.Calendar;
using TaskSharper.Domain.Notification;
using TaskSharper.Service.NotificationClient;
using TaskSharper.Service.NotificationClient.HubConnectionClient;
using TaskSharper.Service.RestClient.Factories;
using TaskSharper.Shared.Logging;
using Microsoft.Practices.Unity;
using Prism.Logging;
using RestSharp;
using Serilog;
using TaskSharper.Appointments.WPF.Config;
using TaskSharper.Appointments.WPF.Views;
using TaskSharper.Configuration.Config;
using TaskSharper.Configuration.Settings;
using TaskSharper.Service.RestClient.Clients;
using TaskSharper.WPF.Common.Components.EventModification;

namespace TaskSharper.Appointments.WPF
{
    public class Bootstrapper : UnityBootstrapper
    {
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
            regionManager.RequestNavigate(ViewConstants.REGION_Main, ViewConstants.VIEW_AppointmentOverview);
        }
        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();

            var logSettings = new LoggingSettingsHandler().Load();
            var clientSettings = new ClientSettingsHandler().Load();

            // Register views
            Container.RegisterTypeForNavigation<AppointmentCardContainerView>(ViewConstants.VIEW_AppointmentOverview);
            Container.RegisterTypeForNavigation<EventModificationView>(ViewConstants.VIEW_ModifyAppointmentView);
            //Container.RegisterTypeForNavigation<>(ViewConstants.VIEW_AppointmentDetails);

            // Register other dependencies
            var logger = LogConfiguration.ConfigureWPF(logSettings);

            // Singletons
            Container.RegisterInstance(typeof(ILogger), logger);
            Container.RegisterInstance(typeof(IRestClient), new RestClient());

            // Not singletons
            Container.RegisterType<IRestRequestFactory, RestRequestFactory>();
            Container.RegisterType<IEventRestClient, EventRestClient>(new InjectionConstructor(clientSettings.APIServerUrl, "events", typeof(IRestClient), typeof(IRestRequestFactory), typeof(ILogger)));
            Container.RegisterType<IAppointmentRestClient, EventRestClient>(new InjectionConstructor(clientSettings.APIServerUrl, "appointments", typeof(IRestClient), typeof(IRestRequestFactory), typeof(ILogger)));
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
