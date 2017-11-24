using TaskSharper.WPF.Common.Events.Resources;

namespace TaskSharper.WPF.Common.Events.NotificationEvents
{
    public class UnauthorizedErrorNotification : Notification
    {
        public UnauthorizedErrorNotification()
        {
            Title = Properties.Resources.ErrorUnauthorizedTitle;
            Message = Properties.Resources.ErrorUnauthorizedMessage;
            NotificationType = NotificationTypeEnum.Error;
        }
    }
}
