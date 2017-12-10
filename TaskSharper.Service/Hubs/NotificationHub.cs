using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Serilog;
using TaskSharper.Domain.Calendar;
using TaskSharper.Domain.Models;
using TaskSharper.Domain.Notification;

namespace TaskSharper.Service.Hubs
{
    public class NotificationHub : Hub
    {
        private readonly ILogger _logger;

        public NotificationHub(ILogger logger)
        {
            _logger = logger;
        }

        public override Task OnConnected()
        {
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            return base.OnDisconnected(stopCalled);
        }

        public override Task OnReconnected()
        {
            return base.OnReconnected();
        }

        public void PublishNotification(Event calEvent)
        {
            IClientProxy proxy = Clients.All;
            proxy.Invoke(typeof(Event).Name, calEvent);
        }
    }
}
