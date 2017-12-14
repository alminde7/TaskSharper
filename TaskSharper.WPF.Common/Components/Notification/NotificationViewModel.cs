using System;
using System.Globalization;
using System.Media;
using System.Threading.Tasks;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Serilog;
using TaskSharper.Domain.Calendar;
using TaskSharper.Domain.Models;
using TaskSharper.Domain.RestClient;
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
        private string _notificationTimeText;
        private string _notificationEventType;
        private string _category;
        private IEventAggregator _eventAggregator;
        private NotificationTypeEnum _notificationType;
        private Event _event;
        private readonly IEventRestClient _dataService;
        private readonly ILogger _logger;
        private bool _spinnerVisible;
        private string _soundPath;
        private CultureInfo _culture;

        public DelegateCommand CloseNotificationCommand { get; set; }

        public DelegateCommand CompleteTaskCommand { get; set; }

        public DelegateCommand<string> ChangeLanguageCommand { get; set; }

        public string NotificationTimeText
        {
            get => _notificationTimeText;
            set => SetProperty(ref _notificationTimeText, value);
        }

        public string NotificationEventType
        {
            get => _notificationEventType;
            set => SetProperty(ref _notificationEventType, value);
        }

        public Event NotificationEvent
        {
            get => _event;
            set => SetProperty(ref _event, value);
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

        public NotificationViewModel(IEventAggregator eventAggregator, ILogger logger, IEventRestClient dataService)
        {
            IsPopupOpen = false;
            _eventAggregator = eventAggregator;
            _dataService = dataService;
            _logger = logger;
            _eventAggregator.GetEvent<NotificationEvent>().Subscribe(HandleNotificationEvent);
            eventAggregator.GetEvent<CultureChangedEvent>().Subscribe(UpdateCultureHandler);
            CloseNotificationCommand = new DelegateCommand(ClosePopUp);
            CompleteTaskCommand = new DelegateCommand(CompleteTask);
        }

        private void UpdateCultureHandler()
        {
            _culture = CultureInfo.CurrentCulture;
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
                if (notification.Event != null && !notification.Event.MarkedAsDone)
                {
                    await ShowNotification(notification);
                }
            }
        }

        private void CompleteTask()
        {
            if (NotificationEvent != null)
                NotificationEvent.MarkedAsDone = true;

            _dataService.UpdateAsync(NotificationEvent);

            ClosePopUp();
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
            if(notification.Event != null)
            NotificationEventType = notification.Event.Type.ToString();
            NotificationType = notification.NotificationType;
            NotificationEvent = notification.Event;

            if (notification.Event?.Category != null)
            {
                Category = CategoryToIconConverter.ConvertToFontAwesomeIcon(notification.Event.Category.Name,
                    notification.Event.Type);
            }
            else
            {
                Category = "Info";
            }

            if (notification.Event?.Start != null)
            {
                if (notification.Event.End != null)
                {
                    var startToEndTime = notification.Event.Start.Value.ToString("HH:mm") + "-" +
                                         notification.Event.End.Value.ToString("HH:mm");

                    if (DateTime.Now.Minute == notification.Event.Start.Value.Minute)
                    {
                        CurrentTimeNotificationText(startToEndTime);
                    }
                    else
                    {
                        if (DateTime.Now > notification.Event.Start.Value)
                        {
                            PastTimeNotificationText(startToEndTime, "", notification);
                        }
                        else if (DateTime.Now < notification.Event.Start.Value)
                        {
                            FutureTimeNotificationText(startToEndTime, "", notification);
                        }
                    }
                }
            }
            IsPopupOpen = true;
            PlayNotificationSound();

            return Task.CompletedTask;
        }

        private void PlayNotificationSound()
        {
            System.Windows.Threading.Dispatcher.CurrentDispatcher.Invoke(() =>
            {
                _soundPath = AppDomain.CurrentDomain.BaseDirectory + "/Media/WindowsNotifyCalendar.wav";
                try
                {
                    SoundPlayer notificationSound = new SoundPlayer(_soundPath);
                    notificationSound.Load();
                    notificationSound.Play();
                }
                catch (Exception e)
                {
                    _logger.ForContext("Error", typeof(NotificationViewModel)).Information("An error occured when trying to play notification sound: {0}", e.Message);
                }
            });
        }

        private void CurrentTimeNotificationText(string startToEndTime)
        {
            var textFormat = LocalizeDictionary.Instance
                .GetLocalizedObject("NotificationNowEvent", null, LocalizeDictionary.Instance.Culture)
                .ToString();

            NotificationTimeText = string.Format(textFormat, startToEndTime);
        }
        private void PastTimeNotificationText(string startToEndTime, string dateTimeMin, Events.Resources.Notification notification)
        {
            TimeSpan substractedDateTime = DateTime.Now.Subtract(notification.Event.Start.Value);
            dateTimeMin = new DateTime(substractedDateTime.Ticks).ToString("mm");
            if (dateTimeMin.StartsWith("0"))
                dateTimeMin = dateTimeMin.TrimStart('0');

            var textFormat = LocalizeDictionary.Instance
                .GetLocalizedObject("NotificationPastEvent", null, LocalizeDictionary.Instance.Culture)
                .ToString();
            NotificationTimeText = string.Format(textFormat, NotificationEventType.ToLower(),
                dateTimeMin, startToEndTime);
        }
        private void FutureTimeNotificationText(string startToEndTime, string dateTimeMin, Events.Resources.Notification notification)
        {
            TimeSpan substractedDateTime = notification.Event.Start.Value.Subtract(DateTime.Now);
            dateTimeMin = new DateTime(substractedDateTime.Ticks).AddMinutes(1).ToString("mm");
            if (dateTimeMin.StartsWith("0"))
                dateTimeMin = dateTimeMin.TrimStart('0');

            var textFormat = LocalizeDictionary.Instance
                .GetLocalizedObject("NotificationPressentEvent", null, LocalizeDictionary.Instance.Culture)
                .ToString();
            NotificationTimeText = string.Format(textFormat, NotificationEventType.ToLower(), dateTimeMin, startToEndTime);
        }
    }
}
