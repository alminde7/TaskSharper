using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Commands;
using Prism.Mvvm;
using TaskSharper.WPF.Common.Events.Resources;

namespace TaskSharper.WPF.Common.Components.Notification
{
    public class NotificationViewModel : BindableBase
    {

        private bool _isPopupOpen;
        private string _notificationTitle;
        private string _notificationMessage;
        private NotificationTypeEnum _notificationType;

        public DelegateCommand CloseNotificationCommand { get; set; }

        public DelegateCommand<string> ChangeLanguageCommand { get; set; }

        public NotificationViewModel()
        {
            
        }
    }
}
