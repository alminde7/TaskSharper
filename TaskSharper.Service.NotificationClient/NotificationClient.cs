using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using Serilog;
using TaskSharper.Domain.Notification;
using TaskSharper.Service.NotificationClient.HubConnectionClient;
using TaskSharper.Shared.Exceptions;

namespace TaskSharper.Service.NotificationClient
{
    /// <summary>
    /// Notification client used to connect to a SignalR server.
    /// </summary>
    public class NotificationClient : IDisposable, INotificationClient
    {
        private const string HubName = "NotificationHub";

        private readonly IHubProxy _notificationHub;
        private readonly IHubConnectionProxy _connection;
        private readonly ILogger _logger;

        public bool IsConnected { get; private set; }
        public int ConnectionRetries { get; set; } = 5;
        public int ConnectionIntervalInMs { get; set; } = 1000;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="logger"></param>
        public NotificationClient(IHubConnectionProxy connection, ILogger logger)
        {
            this._connection = connection;
            _logger = logger.ForContext<NotificationClient>();
            _notificationHub = _connection.CreateHubProxy(HubName);
        }
        
        /// <summary>
        /// Start connection to server. 
        /// </summary>
        /// <returns></returns>
        public async Task Connect()
        {
            int nrOfRetries = 0;
            while (!IsConnected && nrOfRetries < ConnectionRetries)
            {
                _logger.Information("Connecting to hub {@SignalRHub} on server {@SignalRServer}, attempt {@ConnectionAttempt}", HubName, _connection.Url, nrOfRetries +1);
                await _connection.Start().ContinueWith(task => { IsConnected = !task.IsFaulted; });
                if (IsConnected) break;
                nrOfRetries++;
                Thread.Sleep(ConnectionIntervalInMs);
            }

            if (!IsConnected)
            {
                throw new ConnectionException(
                    $"Faild to connect to hub {HubName} on server {_connection.Url} after {nrOfRetries+1} attempts");
            }
            else
            {
                _logger.Information("Succesfully established connection to {@SignalRHub} on server {@SignalRServer}", HubName, _connection.Url);
            }
        }

        /// <summary>
        /// Creates a subscription to a server event, with specified callback.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="callback"></param>
        public void Subscribe<T>(Action<T> callback)
        {
            if (!IsConnected)
            {
                throw new ConnectionException($"There is no connection to hub {HubName} on server {_connection.Url}");
            }
            
            _notificationHub.On<T>(typeof(T).Name, callback);
        }

        /// <summary>
        /// Stop connection.
        /// </summary>
        public void Dispose()
        {
            _connection.Stop();
        }
    }
}
