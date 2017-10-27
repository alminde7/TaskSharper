using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using TaskSharper.Domain.Notification;

namespace TaskSharper.Service.Hubs
{
    public class SignalRNotificationPublisher : INotificationPublisher
    {
        private readonly IHubContext _context;
        public SignalRNotificationPublisher()
        {
            _context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
        }

        public void Publish<T>(T eventMessage)
        {
            // https://stackoverflow.com/questions/16079813/how-to-use-a-variable-as-a-method-name-using-dynamic-objects
            IClientProxy proxy = _context.Clients.All;
            proxy.Invoke(typeof(T).Name, eventMessage);
        }
    }
}
