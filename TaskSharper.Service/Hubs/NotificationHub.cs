﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Serilog;
using TaskSharper.Domain.Calendar;
using TaskSharper.Domain.Notification;

namespace TaskSharper.Service.Hubs
{
    public class NotificationHub : Hub
    {
        private readonly INotification _eventNotification;
        private readonly ILogger _logger;

        public NotificationHub(INotification eventNotification, ILogger logger)
        {
            _eventNotification = eventNotification;
            _logger = logger;
            _eventNotification.Callback = PublishNotification;
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
            Clients.All.EventNotification(calEvent);
        }
    }
}
