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
using RestSharp;
using Serilog;

namespace TaskSharper.Appointments.WPF
{
    public class Bootstrapper : UnityBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void InitializeShell()
        {
            Application.Current.MainWindow.Show();
        }
        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();

            // Register views
        

            // Register other dependencies
            var logger = LogConfiguration.ConfigureWPF();

            // Singletons
            Container.RegisterInstance(typeof(ILogger), logger);
            Container.RegisterInstance(typeof(IRestClient), new RestClient());

            // Not singletons
            Container.RegisterType<IRestRequestFactory, RestRequestFactory>();
            Container.RegisterType<IEventRestClient, EventRestClient>();

            var hubConnectionClient = new HubConnectionProxy("http://localhost:8000");
            Container.RegisterInstance(typeof(IHubConnectionProxy), hubConnectionClient);
            Container.RegisterType<INotificationClient, NotificationClient>();
        }
    }
}
