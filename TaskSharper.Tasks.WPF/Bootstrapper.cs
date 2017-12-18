using System;
using System.Windows;
using Prism.Regions;
using Prism.Unity;
using RestSharp;
using Serilog;
using TaskSharper.Domain.Calendar;
using TaskSharper.Domain.Notification;
using TaskSharper.Service.NotificationClient;
using TaskSharper.Service.NotificationClient.HubConnectionClient;
using TaskSharper.Service.RestClient.Factories;
using TaskSharper.Shared.Logging;
using Microsoft.Practices.Unity;
using Prism.Logging;
using TaskSharper.Configuration.Config;
using TaskSharper.Configuration.Settings;
using TaskSharper.Domain.Configuration.Logging;
using TaskSharper.Domain.RestClient;
using TaskSharper.Service.RestClient.Clients;
using TaskSharper.Tasks.WPF.Config;
using TaskSharper.Tasks.WPF.Views;
using TaskSharper.WPF.Common.Components.EventModification;

namespace TaskSharper.Tasks.WPF
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
            regionManager.RequestNavigate(ViewConstants.REGION_Main, ViewConstants.VIEW_TaskOverview);
        }
        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();

            var logSettings = new LoggingSettingsHandler().Load();
            var clientSettings = new ClientSettingsHandler().Load();

            // Register views
            Container.RegisterTypeForNavigation<TaskCardContainerView>(ViewConstants.VIEW_TaskOverview);
            Container.RegisterTypeForNavigation<EventModificationView>(ViewConstants.VIEW_ModifyTaskView);
            //Container.RegisterTypeForNavigation<>(ViewConstants.VIEW_AppointmentDetails);

            // Register other dependencies
            var logger = LogConfiguration.ConfigureWPF(logSettings);

            // Singletons
            Container.RegisterInstance(typeof(ILogger), logger);
            Container.RegisterInstance(typeof(IRestClient), new RestClient());

            // Not singletons
            Container.RegisterType<IRestRequestFactory, RestRequestFactory>();
            Container.RegisterType<IEventRestClient, EventRestClient>(new InjectionConstructor(clientSettings.APIServerUrl, "events", typeof(IRestClient), typeof(IRestRequestFactory), typeof(ILogger)));
            Container.RegisterType<ITaskRestClient, EventRestClient>(new InjectionConstructor(clientSettings.APIServerUrl, "tasks", typeof(IRestClient), typeof(IRestRequestFactory), typeof(ILogger)));
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
