namespace TaskSharper.Domain.Configuration.Notification
{
    public class NotificationSettings
    {
        public NotificationSettings()
        {
            Appointments = new AppointmentsSettings();
            Tasks = new TasksSettings();
        }
        public bool EnableNotifications { get; set; } = true;
        public NoneTypeSettings NoneType { get; set; }
        public AppointmentsSettings Appointments { get; set; }
        public TasksSettings Tasks { get; set; }    

    }
}