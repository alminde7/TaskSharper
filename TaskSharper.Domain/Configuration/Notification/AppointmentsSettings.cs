using System.Collections.Generic;

namespace TaskSharper.Domain.Configuration.Notification
{
    public class AppointmentsSettings
    {
        public bool EnableAppointmentNotifications { get; set; } = true;
        public List<int> NotificationOffsets { get; set; } = new List<int>() { -15, -5, 0, 5, 10, 15 };
    }
}