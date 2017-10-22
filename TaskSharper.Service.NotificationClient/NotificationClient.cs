using System;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using Serilog;
using TaskSharper.Domain.Calendar;
using TaskSharper.Domain.Notification;
using TaskSharper.Service.NotificationClient.Exceptions;
using TaskSharper.Service.NotificationClient.HubConnectionClient;

namespace TaskSharper.Service.NotificationClient
{
    public class NotificationClient : IDisposable, INotificationClient
    {
        private const string HubName = "NotificationHub";
        private const string EventName = "EventNotification";

        private readonly IHubProxy _notificationHub;
        private readonly IHubConnectionProxy _connection;

        public bool IsConnected { get; private set; }

        public NotificationClient(IHubConnectionProxy connection)
        {
            this._connection = connection;
            _notificationHub = _connection.CreateHubProxy(HubName);
        }

        public async Task Connect()
        {
            await _connection.Start().ContinueWith(task =>
            {
                if (task.IsFaulted) // Could not connect
                {
                    IsConnected = false;
                    throw new ConnectionException($"Faild to connect to hub {HubName} on server {_connection.Url}");
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
                throw new ConnectionException($"There is no connection to hub {HubName} on server {_connection.Url}");
            }
            
            _notificationHub.On<Event>(EventName, callback);
        }

        public void Dispose()
        {
            _connection.Stop();
        }
    }
}
