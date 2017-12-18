using System.Timers;

namespace TaskSharper.Notification
{
    /// <summary>
    /// NotificationObject hold timer data.
    /// </summary>
    public class NotificationObject
    {
        public Timer Timer { get; set; }
        public bool HasFired { get; set; }
    }
}