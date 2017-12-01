using System;
using System.Globalization;
using System.Media;
using System.Threading.Tasks;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Serilog;
using TaskSharper.Domain.Calendar;
using TaskSharper.WPF.Common.Config;
using TaskSharper.WPF.Common.Events;
using TaskSharper.WPF.Common.Events.NotificationEvents;
using TaskSharper.WPF.Common.Events.Resources;
using TaskSharper.WPF.Common.Media;
using WPFLocalizeExtension.Engine;

namespace TaskSharper.WPF.Common.Components.Notification
{
    /// <summary>
    /// NotificationViewModel is the ViewModel for the NotificationView.xaml
    /// It is made for the purpose of being a genereal component of showing notifications. 
    /// </summary>
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

        /// <summary>
        /// The category property is databinded to the view for the purpose of showcaseing font awesome icons,
        /// depending on the category type. 
        /// </summary>
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

        /// <summary>
        /// IsPopupOpen property is databinded to the view for the purpose of enabling and disabling the popup visually
        /// </summary>
        public bool IsPopupOpen
        {
            get => _isPopupOpen;
            set => SetProperty(ref _isPopupOpen, value);
        }

        /// <summary>
        /// Depending on the NotificationTypeEnum the popup changes background colour.
        /// </summary>
        public NotificationTypeEnum NotificationType
        {
            get => _notificationType;
            set => SetProperty(ref _notificationType, value);
        }

        /// <summary>
        /// Constructor that subscribe to Notification events and culture changes, and sends the event to a designated action. 
        /// </summary>
        /// <param name="eventAggregator"></param>
        /// <param name="logger"></param>
        /// <param name="dataService"></param>
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

        /// <summary>
        /// After receiving the event of culture change, the culture have already been changed.
        /// This function only sets the private culture to the updated version. 
        /// </summary>
        private void UpdateCultureHandler()
        {
            _culture = CultureInfo.CurrentCulture;
        }

        /// <summary>
        /// This is the action that handles the Notification event that is subscribed to in the constructor.
        /// 
        /// If there is an issue with internet connection, then this notification should only be shown once,
        /// when shown a static variable is set.
        /// 
        /// If the notification event is null or already marked as dismiss/complete then it will not be shown. 
        /// </summary>
        /// <param name="notification">The notification event containing either a task or appointment event, or a
        /// internetconnection event.</param>
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

        /// <summary>
        /// When the button dismiss/complete has been pressed the delegateCommand CompleteTaskCommand will call this function.
        /// It will throw an new event to the dataservice with an updated MarkedAsDone on the event. 
        /// And finaly call the method ClosePopUp(). 
        /// </summary>
        private void CompleteTask()
        {
            if (NotificationEvent != null)
                NotificationEvent.MarkedAsDone = true;

            _dataService.UpdateAsync(NotificationEvent);

            ClosePopUp();
        }
        /// <summary>
        /// Sets the property IsPopupOpen to false which makes the Popup collapse in the view. 
        /// also publish the event of not showing the spinner. The spinner event is set to show because it greys out 
        /// all content behind the notification event. 
        /// </summary>
        private void ClosePopUp()
        {
            IsPopupOpen = false;
            _eventAggregator.GetEvent<SpinnerEvent>().Publish(EventResources.SpinnerEnum.Hide);
        }

        /// <summary>
        /// ShowNotification method starts the spinner and sets all of the properties from the event into properties of the 
        /// viewModel. 
        /// 
        /// From the event category the CategoryToIconConverter will generate the correct font-awesome string, which will 
        /// be serialized in the view. 
        /// 
        /// Depending on the start and end time the different method will be called to generate the correct ui text. 
        /// 
        /// Its made async to await the notification sound being played. This operation could take more than 0,1 and thereby 
        /// blocking the UI thread.
        /// </summary>
        /// <param name="notification">The notification event containing either a task or appointment event</param>
        /// <returns></returns>
        private async Task ShowNotification(Events.Resources.Notification notification)
        {
            _eventAggregator.GetEvent<SpinnerEvent>().Publish(EventResources.SpinnerEnum.Show);
            NotificationTitle = notification.Title;
            NotificationMessage = notification.Message;
            NotificationEventType = notification.Event.Type.ToString();
            NotificationType = notification.NotificationType;
            NotificationEvent = notification.Event;

            if (notification.Event.Category != null)
            {
                Category = CategoryToIconConverter.ConvertToFontAwesomeIcon(notification.Event.Category.Name,
                    notification.Event.Type);
            }
            else
            {
                Category = "Info";
            }

            if (notification.Event.Start != null)
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
            await PlayNotificationSound();
        }
        /// <summary>
        /// Uses AppDomain.CurrentDomain.BaseDirectory so that when doployed and when using the launcher application 
        /// the path route is set correct. 
        /// 
        /// Uses SoundPlayer library for playing sound files. 
        /// </summary>
        /// <returns></returns>
        private Task PlayNotificationSound()
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

            return Task.CompletedTask;
        }

        /// <summary>
        /// Helper function for when the Notification is in the current timeframe, and have to be displayed acordingly.
        /// </summary>
        /// <param name="startToEndTime">The string telling when the event is occour from and to</param>
        private void CurrentTimeNotificationText(string startToEndTime)
        {
            var textFormat = LocalizeDictionary.Instance
                .GetLocalizedObject("NotificationNowEvent", null, LocalizeDictionary.Instance.Culture)
                .ToString();

            NotificationTimeText = string.Format(textFormat, startToEndTime);
        }

        /// <summary>
        /// Helper function for when the event is in the past timeframe, and have to be displayed acordingly.
        /// </summary>
        /// <param name="startToEndTime">The string telling when the event is occour from and to</param>
        /// <param name="dateTimeMin">The string telling when the event had happent</param>
        /// <param name="notification">The event</param>
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

        /// <summary>
        /// Helper function for when the event is in the future timeframe, and have to be displayed acordingly.
        /// </summary>
        /// <param name="startToEndTime">The string telling when the event is occour from and to</param>
        /// <param name="dateTimeMin">The string telling when the event is going to happen</param>
        /// <param name="notification">The event</param>
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
