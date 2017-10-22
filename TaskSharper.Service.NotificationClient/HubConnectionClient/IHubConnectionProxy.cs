using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;

namespace TaskSharper.Service.NotificationClient.HubConnectionClient
{
    public interface IHubConnectionProxy
    {
        Task Start();
        IHubProxy CreateHubProxy(string hubName);
        void Stop();
    }
}