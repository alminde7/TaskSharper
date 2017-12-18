using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using TaskSharper.Domain.Notification;

namespace TaskSharper.Service.Hubs
{
    /// <summary>
    /// Used to publish events over a SignalR socket connection
    /// </summary>
    public class SignalRNotificationPublisher : INotificationPublisher
    {
        private readonly IHubContext _context;

        /// <summary>
        /// Constructor.
        /// </summary>
        public SignalRNotificationPublisher()
        {
            _context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="eventMessage"></param>
        public void Publish<T>(T eventMessage)
        {
            // https://stackoverflow.com/questions/16079813/how-to-use-a-variable-as-a-method-name-using-dynamic-objects
            IClientProxy proxy = _context.Clients.All;
            proxy.Invoke(typeof(T).Name, eventMessage);
        }
    }
}
