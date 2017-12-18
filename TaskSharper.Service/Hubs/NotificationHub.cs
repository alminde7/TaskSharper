using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Serilog;
using TaskSharper.Domain.Models;

namespace TaskSharper.Service.Hubs
{
    /// <summary>
    /// Notification hub used for client to connect to.
    /// </summary>
    public class NotificationHub : Hub
    {
        private readonly ILogger _logger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger"></param>
        public NotificationHub(ILogger logger)
        {
            _logger = logger;
        }
        
        public void PublishNotification(Event calEvent)
        {
            IClientProxy proxy = Clients.All;
            proxy.Invoke(typeof(Event).Name, calEvent);
        }
    }
}
