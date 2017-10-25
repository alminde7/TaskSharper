using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;

namespace TaskSharper.Service.NotificationClient.HubConnectionClient
{
    public class HubConnectionProxy : IHubConnectionProxy
    {
        private readonly HubConnection _connection;

        public string Url { get; }

        public HubConnectionProxy(string url)
        {
            _connection = new HubConnection(url);
            Url = url;
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
}