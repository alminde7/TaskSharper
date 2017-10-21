using System;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Hubs;
using TaskSharper.Domain.Calendar;

namespace TaskSharper.Service.NotificationClient
{
    public class NotificationClient : IDisposable
    {
        private readonly IHubProxy _notificationHub;
        private readonly IHubConnectionClient _connection;

        public bool IsConnected { get; private set; }

        public NotificationClient(IHubConnectionClient connection)
        {
            this._connection = connection;
            _notificationHub = _connection.CreateHubProxy("Notification");
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
            
            _notificationHub.On<Event>("EventNotification", callback);
        }

        public void Dispose()
        {
            _connection.Stop();
        }
    }

    public class HubConnectionClient : IHubConnectionClient
    {
        private readonly HubConnection _connection;

        public HubConnectionClient(string url)
        {
            _connection = new HubConnection(url);
        }

        public Task Start()
        {
            return _connection.Start();
        }

        public IHubProxy CreateHubProxy(string hubName)
        {
            return _connection.CreateHubProxy(hubName);
        }

        public void Stop()
        {
            _connection.Stop();
        }
    }

    public interface IHubConnectionClient
    {
        Task Start();
        IHubProxy CreateHubProxy(string hubName);
        void Stop();
    }
}
