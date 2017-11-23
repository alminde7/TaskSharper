namespace TaskSharper.Domain.Configuration
{
    public class NotificationSettings
    {
        public NotificationSettings()
        {
            Appointments = new AppointmentsSettings();
            Tasks = new TasksSettings();
        }
        public bool EnableNotifications { get; set; } = true;
        public AppointmentsSettings Appointments { get; set; }
        public TasksSettings Tasks { get; set; }    

    }
}