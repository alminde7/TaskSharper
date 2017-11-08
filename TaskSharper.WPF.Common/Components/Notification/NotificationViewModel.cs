using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using TaskSharper.WPF.Common.Config;
using TaskSharper.WPF.Common.Events;
using TaskSharper.WPF.Common.Events.NotificationEvents;
using TaskSharper.WPF.Common.Events.Resources;
using TaskSharper.WPF.Common.Properties;

namespace TaskSharper.WPF.Common.Components.Notification
{
    public class NotificationViewModel : BindableBase
    {
        private bool _isPopupOpen;
        private string _notificationTitle;
        private string _notificationMessage;
        private IEventAggregator _eventAggregator;
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

        public NotificationTypeEnum NotificationType
        {
            get => _notificationType;
            set => SetProperty(ref _notificationType, value);
        }

        public NotificationViewModel(IEventAggregator eventAggregator)
        {
            IsPopupOpen = false;
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<NotificationEvent>().Subscribe(HandleNotificationEvent);
            _eventAggregator.GetEvent<SpinnerEvent>().Subscribe(SetSpinnerVisibility);
            _eventAggregator.GetEvent<ScrollButtonsEvent>().Subscribe(SetScrollButtonsVisibility);

            CloseNotificationCommand = new DelegateCommand(ClosePopUp);
        }
        private void SetSpinnerVisibility(EventResources.SpinnerEnum state)
        {
            Console.WriteLine(state);
        }
        private void SetScrollButtonsVisibility(EventResources.ScrollButtonsEnum state)
        {
            Console.WriteLine(state);
        }

        private async void HandleNotificationEvent(Events.Resources.Notification notification)
        {
            if (notification is ConnectionErrorNotification)
            {
                if (ApplicationStatus.InternetConnection)
                {
                    await ShowNotification(notification);
                    ApplicationStatus.InternetConnection = false;
                }
                else
                {                 
                    _eventAggregator.GetEvent<SpinnerEvent>().Publish(EventResources.SpinnerEnum.Hide);
                }
            }
            else
            {
                await ShowNotification(notification);
            }
        }

        private void ClosePopUp()
        {
            IsPopupOpen = false;
            _eventAggregator.GetEvent<SpinnerEvent>().Publish(EventResources.SpinnerEnum.Hide);
        }

        private Task ShowNotification(Events.Resources.Notification notification)
        {
            _eventAggregator.GetEvent<SpinnerEvent>().Publish(EventResources.SpinnerEnum.Show);
            NotificationTitle = notification.Title;
            NotificationMessage = notification.Message;
            NotificationType = notification.NotificationType;

            IsPopupOpen = true;
            System.Windows.Threading.Dispatcher.CurrentDispatcher.Invoke(() =>
            {
                var path = "Media/WindowsNotifyCalendar.wav";
                SoundPlayer notificationSound = new SoundPlayer(path);
                notificationSound.Load();
                notificationSound.Play();
            });

            return Task.CompletedTask;
        }
    }
}
