using System;
using System.ComponentModel;
using System.Windows;
using Prism.Regions;
using Prism.Unity;
using TaskSharper.Domain.Calendar;
using TaskSharper.Domain.Notification;
using TaskSharper.Service.NotificationClient;
using TaskSharper.Service.NotificationClient.HubConnectionClient;
using TaskSharper.Service.RestClient;
using TaskSharper.Service.RestClient.Factories;
using TaskSharper.Shared.Logging;
using Microsoft.Practices.Unity;
using Prism.Logging;
using RestSharp;
using Serilog;
using TaskSharper.Appointments.WPF.Config;
using TaskSharper.Appointments.WPF.Views;
using TaskSharper.Service.RestClient.Clients;

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

            // Register views
            Container.RegisterTypeForNavigation<AppointmentCardContainerView>(ViewConstants.VIEW_AppointmentOverview);
            //Container.RegisterTypeForNavigation<>(ViewConstants.VIEW_AppointmentDetails);

            // Register other dependencies
            var logger = LogConfiguration.ConfigureWPF();

            // Singletons
            Container.RegisterInstance(typeof(ILogger), logger);
            Container.RegisterInstance(typeof(IRestClient), new RestClient());

            // Not singletons
            Container.RegisterType<IRestRequestFactory, RestRequestFactory>();
            Container.RegisterType<IAppointmentRestClient, EventRestClient>(new InjectionConstructor("appointments", typeof(IRestClient), typeof(IRestRequestFactory), typeof(ILogger)));
            Container.RegisterType<IStatusRestClient, StatusRestClient>();

            var hubConnectionClient = new HubConnectionProxy("http://localhost:8000");
            Container.RegisterInstance(typeof(IHubConnectionProxy), hubConnectionClient);
            Container.RegisterType<INotificationClient, NotificationClient>();
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
