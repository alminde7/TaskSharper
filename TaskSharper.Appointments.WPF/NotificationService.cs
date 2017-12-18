using System;
using System.Threading.Tasks;
using Prism.Events;
using Serilog;
using TaskSharper.Domain.Models;
using TaskSharper.Domain.Notification;
using TaskSharper.Domain.ServerEvents;
using TaskSharper.Shared.Exceptions;
using TaskSharper.WPF.Common.Events;
using TaskSharper.WPF.Common.Events.NotificationEvents;
using TaskSharper.WPF.Common.Events.Resources;

namespace TaskSharper.Appointments.WPF
{
    /// <summary>
    /// Service that run at the startup of the Appointment application.
    /// </summary>
    public class NotificationService
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly INotificationClient _notificationClient;
        private readonly ILogger _logger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="eventAggregator">PRISM EventAggregator to publish and subscribe to methods</param>
        /// <param name="notificationClient">Client to subscribe to events from the service</param>
        /// <param name="logger">Logger</param>
        public NotificationService(IEventAggregator eventAggregator, INotificationClient notificationClient, ILogger logger)
        {
            _eventAggregator = eventAggregator;
            _notificationClient = notificationClient;
            _logger = logger;
        }

        /// <summary>
        /// Start service, and connects to server
        /// </summary>
        /// <returns></returns>
        public async Task StartContinousService()
        {
            try
            {
                await _notificationClient.Connect();
            }
            catch (ConnectionException e)
            {
                _logger.Error(e, "Faild to establish socket connection to server");
                _eventAggregator.GetEvent<NotificationEvent>().Publish(new Notification()
                {
                    Message = "An error orcurred. You will not receive any notification",
                    Title = "Connection error"
                });
            }
            catch (Exception e)
            {
                _logger.Error(e, "An error orcurred connecting to the server via a socket connection");
                _eventAggregator.GetEvent<NotificationEvent>().Publish(new Notification()
                {
                    Message = "An error orcurred",
                    Title = "Error"
                });
            }

            ConfigureSubscription();
        }

        /// <summary>
        /// Configure subscriptions to events and callbacks
        /// </summary>
        private void ConfigureSubscription()
        {
            if (!_notificationClient.IsConnected)
            {
                _logger.Error("Could not configure socket subscriptions as there is no connection to the server");
                _eventAggregator.GetEvent<NotificationEvent>().Publish(new Notification()
                {
                    Message = "An error orcurred. You will not receive any notification",
                    Title = "Connection error"
                });
                return;
            }

            //NOTE:: Subscribe socket server events here
            _notificationClient.Subscribe<Event>(x =>
            {
                _eventAggregator.GetEvent<NotificationEvent>().Publish(new Notification()
                {
                    Message = x.Description,
                    Title = x.Title,
                    Event = x
                });
            });

            _notificationClient.Subscribe<GettingExternalDataEvent>(x =>
            {
                _eventAggregator.GetEvent<SpinnerEvent>().Publish(EventResources.SpinnerEnum.Show);
            });
            _notificationClient.Subscribe<FinishedGettingExternalDataEvent>(x => _eventAggregator.GetEvent<SpinnerEvent>().Publish(EventResources.SpinnerEnum.Hide));

        }
    }
}
