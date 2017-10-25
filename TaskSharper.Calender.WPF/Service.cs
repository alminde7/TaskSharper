using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Events;
using Serilog;
using TaskSharper.Calender.WPF.Events.NotificationEvents;
using TaskSharper.Calender.WPF.Events.Resources;
using TaskSharper.Domain.Calendar;
using TaskSharper.Domain.Notification;
using TaskSharper.Service.NotificationClient.Exceptions;

namespace TaskSharper.Calender.WPF
{
    public class Service
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly INotificationClient _notificationClient;
        private readonly ILogger _logger;

        public Service(IEventAggregator eventAggregator, INotificationClient notificationClient, ILogger logger)
        {
            _eventAggregator = eventAggregator;
            _notificationClient = notificationClient;
            _logger = logger;
        }

        public async Task StartContinousService()
        {
            await StartSocket();
        }

        private async Task StartSocket()
        {
            try
            {
                await _notificationClient.Connect();
                _notificationClient.Subscribe(EventNotificationHandler);
            }
            catch (ConnectionException e)
            {
                _notificationClient.Dispose();
                _logger.Error(e, "Socket|Faild to establish connection to server");
                // TODO:: What to do in this case ?
                throw;
            }
        }

        private void EventNotificationHandler(Event calEvent)
        {
            _eventAggregator.GetEvent<NotificationEvent>().Publish(new Notification()
            {
                Message = calEvent.Description,
                Title = calEvent.Title
            });
        }
    }
}
