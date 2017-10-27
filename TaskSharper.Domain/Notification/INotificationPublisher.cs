using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskSharper.Domain.Notification
{
    public interface INotificationPublisher
    {
        void Publish<T>(T eventMessage);
    }
}
