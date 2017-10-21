using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using TaskSharper.Domain.Calendar;
using TaskSharper.Domain.Notification;

namespace TaskSharper.Service.Hubs
{
    public class NotificationHub : Hub
    {
        private readonly INotification _eventNotification;

        public NotificationHub(INotification eventNotification)
        {
            _eventNotification = eventNotification;
            _eventNotification.Callback = PublishNotification;
        }

        public void PublishNotification(Event calEvent)
        {
            Clients.All.EventNotification(calEvent);
        }
    }
}
