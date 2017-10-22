using System;
using System.Threading.Tasks;
using TaskSharper.Domain.Calendar;

namespace TaskSharper.Domain.Notification
{
    public interface INotificationClient
    {
        Task Connect();
        void Subscribe(Action<Event> callback);
        void Dispose();
    }
}