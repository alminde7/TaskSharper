using System;
using System.Threading.Tasks;
using TaskSharper.Domain.Calendar;

namespace TaskSharper.Domain.Notification
{
    public interface INotificationClient
    {
        bool IsConnected { get; }
        int ConnectionRetries { get; set; }
        int ConnectionIntervalInMs { get; set; }

        Task Connect();
        void Subscribe<T>(Action<T> callback);
        void Dispose();
    }
}