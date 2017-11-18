using System;
using System.Globalization;
using System.Media;
using System.Threading.Tasks;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using TaskSharper.Domain.Calendar;
using TaskSharper.WPF.Common.Config;
using TaskSharper.WPF.Common.Events;
using TaskSharper.WPF.Common.Events.NotificationEvents;
using TaskSharper.WPF.Common.Events.Resources;
using TaskSharper.WPF.Common.Media;
using WPFLocalizeExtension.Engine;

namespace TaskSharper.WPF.Common.Components.Notification
{
    public class NotificationViewModel : BindableBase
    {
        private bool _isPopupOpen;
        private string _notificationTitle;
        private string _notificationMessage;
        private string _notificationStart;
        private string _notificationEventType;
        private string _category;
        private IEventAggregator _eventAggregator;
        private NotificationTypeEnum _notificationType;
        private bool _spinnerVisible;
        private CultureInfo _culture;

        public DelegateCommand CloseNotificationCommand { get; set; }

        public DelegateCommand<string> ChangeLanguageCommand { get; set; }

        public string NotificationStart
        {
            get => _notificationStart;
            set => SetProperty(ref _notificationStart, value);
        }

        public string NotificationEventType
        {
            get => _notificationEventType;
            set => SetProperty(ref _notificationEventType, value);
        }

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
        public string Category
        {
            get => _category;
            set => SetProperty(ref _category, value);
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
            eventAggregator.GetEvent<CultureChangedEvent>().Subscribe(UpdateCultureHandler);
            CloseNotificationCommand = new DelegateCommand(ClosePopUp);
        }

        private void UpdateCultureHandler()
        {
            _culture = CultureInfo.CurrentCulture;
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
            NotificationEventType = notification.Type.ToString();
            NotificationType = notification.NotificationType;
            if (notification.Category != null)
            {
                Category = CategoryToIconConverter.ConvertToFontAwesomeIcon(notification.Category.Name,
                    notification.Type);
            }
            else
            {
                Category = "Info";
            }

            if (notification.Start != null)
            {
                string dateTimeMin = "";
                if (DateTime.Now > notification.Start.Value)
                {
                    TimeSpan substractedDateTime = DateTime.Now.Subtract(notification.Start.Value);
                    dateTimeMin = new DateTime(substractedDateTime.Ticks).ToString("mm");
                    if (dateTimeMin.StartsWith("0"))
                        dateTimeMin = dateTimeMin.TrimStart('0');

                    var textFormat = LocalizeDictionary.Instance
                        .GetLocalizedObject("NotificationPastEvent", null, LocalizeDictionary.Instance.Culture)
                        .ToString();
                    NotificationStart = string.Format(textFormat, NotificationEventType.ToLower(),
                        dateTimeMin);
                }
                else
                {
                    TimeSpan substractedDateTime = notification.Start.Value.Subtract(DateTime.Now);
                    dateTimeMin = new DateTime(substractedDateTime.Ticks).AddMinutes(1).ToString("mm");
                    if (dateTimeMin.StartsWith("0"))
                        dateTimeMin = dateTimeMin.TrimStart('0');

                    var textFormat = LocalizeDictionary.Instance
                        .GetLocalizedObject("NotificationPastEvent", null, LocalizeDictionary.Instance.Culture)
                        .ToString();
                    NotificationStart = string.Format(textFormat, NotificationEventType.ToLower(), dateTimeMin);
                }   
            }
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
