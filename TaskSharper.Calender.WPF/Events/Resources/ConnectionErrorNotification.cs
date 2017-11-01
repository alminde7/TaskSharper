namespace TaskSharper.Calender.WPF.Events.Resources
{
    public class ConnectionErrorNotification : Notification
    {
        public ConnectionErrorNotification()
        {
            Title = Properties.Resources.NoConnection;
            Message = Properties.Resources.NoConnectionMessage;
            NotificationType = NotificationTypeEnum.Error;
        }
    }
}