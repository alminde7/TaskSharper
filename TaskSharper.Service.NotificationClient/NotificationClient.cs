using System;
using Microsoft.AspNet.SignalR.Client;
using TaskSharper.Domain.Calendar;
using TaskSharper.Service.NotificationClient.HubConnectionClient;

namespace TaskSharper.Service.NotificationClient
{
    public class NotificationClient : IDisposable
    {
        private const string HubName = "Notification";
        private const string EventName = "EventNotification";

        private readonly IHubProxy _notificationHub;
        private readonly IHubConnectionClient _connection;

        public bool IsConnected { get; private set; }

        public NotificationClient(IHubConnectionClient connection)
        {
            this._connection = connection;
            _notificationHub = _connection.CreateHubProxy(HubName);
        }

        public async void Connect()
        {
            await _connection.Start().ContinueWith(task =>
            {
                if (task.IsFaulted) // Could not connect
                {
                    IsConnected = false;
                    throw new Exception("Could not connect to hub");
                }
                else // connection succesfully
                {
                    IsConnected = true;
                }
            });
        }

        public void Subscribe(Action<Event> callback)
        {
            if (!IsConnected)
            {
                throw new Exception("There is no connection to the notification server");
            }
            
            _notificationHub.On<Event>(EventName, callback);
        }

        public void Dispose()
        {
            _connection.Stop();
        }
    }
}
