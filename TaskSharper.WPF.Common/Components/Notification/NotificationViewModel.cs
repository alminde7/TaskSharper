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
        private bool _spinnerVisible;

        public DelegateCommand CloseNotificationCommand { get; set; }

        public DelegateCommand<string> ChangeLanguageCommand { get; set; }

        public string NotificationTitle
        {
            get => _notificationTitle;
            set => SetProperty(ref _notificationTitle, value);
        }
        public string NotificationMessage
        {
            get => _notificationMessage;
            set => SetProperty(ref _notificationMessage, value);
        }
        public bool SpinnerVisible
        {
            get => _spinnerVisible;
            set => SetProperty(ref _spinnerVisible, value);
        }

        public bool IsPopupOpen
        {
            get => _isPopupOpen;
            set => SetProperty(ref _isPopupOpen, value);
        }

        private void ClosePopUp()
        {
            IsPopupOpen = false;
            SetSpinnerVisibility(EventResources.SpinnerEnum.Hide);
        }

        private void SetSpinnerVisibility(EventResources.SpinnerEnum state)
        {
            switch (state)
            {
                case EventResources.SpinnerEnum.Show:
                    SpinnerVisible = true;
                    break;
                case EventResources.SpinnerEnum.Hide:
                    SpinnerVisible = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        public NotificationViewModel()
        {
            NotificationTitle = "test";
            NotificationMessage = "test1234";
            IsPopupOpen = true;
        }
    }
}
