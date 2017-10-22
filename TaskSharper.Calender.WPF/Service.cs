using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Events;
using TaskSharper.Calender.WPF.Events.NotificationEvents;
using TaskSharper.Calender.WPF.Events.Resources;
using TaskSharper.Domain.Calendar;
using TaskSharper.Domain.Notification;

namespace TaskSharper.Calender.WPF
{
    public class Service
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly INotificationClient _notificationClient;

        public Service(IEventAggregator eventAggregator, INotificationClient notificationClient)
        {
            _eventAggregator = eventAggregator;
            _notificationClient = notificationClient;
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
            catch (Exception e)
            {
                _notificationClient.Dispose();
                // TODO:: Implement proper error handling
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
