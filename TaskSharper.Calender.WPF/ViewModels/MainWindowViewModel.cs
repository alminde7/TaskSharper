using System;
using System.Globalization;
using System.Media;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Serilog;
using TaskSharper.Calender.WPF.Config;
using TaskSharper.Calender.WPF.Events;
using TaskSharper.Calender.WPF.Events.NotificationEvents;
using TaskSharper.Calender.WPF.Events.Resources;
using TaskSharper.Calender.WPF.Events.ScrollEvents;
using TaskSharper.Calender.WPF.Properties;
using TaskSharper.Domain.Calendar;
using WPFLocalizeExtension.Engine;

namespace TaskSharper.Calender.WPF.ViewModels
{
    public partial class MainWindowViewModel : BindableBase 
    {
        private readonly IRegionManager _regionManager;
        private IEventAggregator _eventAggregator;
        private IStatusRestClient _statusRestClient;
        private readonly ILogger _logger;
        private bool _spinnerVisible;
        private bool _scrollButtonsVisible;
        private bool _isPopupOpen;
        private string _notificationTitle;
        private string _notificationMessage;
        private NotificationTypeEnum _notificationType;

        public DelegateCommand<string> NavigateCommand { get; set; }
        public DelegateCommand CloseNotificationCommand { get; set; }
        public DelegateCommand<string> ChangeLanguageCommand { get; set; }
        public DelegateCommand ScrollUpCommand { get; set; }
        public DelegateCommand ScrollDownCommand { get; set; }

        public MainWindowViewModel(IRegionManager regionManager, IEventAggregator eventAggregator, ILogger logger, IStatusRestClient statusRestClient)
        {
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;
            _logger = logger.ForContext<MainWindowViewModel>();
            _statusRestClient = statusRestClient;

            _eventAggregator.GetEvent<SpinnerEvent>().Subscribe(SetSpinnerVisibility);
            _eventAggregator.GetEvent<NotificationEvent>().Subscribe(HandleNotificationEvent);
            _eventAggregator.GetEvent<ScrollButtonsEvent>().Subscribe(SetScrollButtonsVisibility);

            NavigateCommand = new DelegateCommand<string>(Navigate);
            CloseNotificationCommand = new DelegateCommand(ClosePopUp);
            ChangeLanguageCommand = new DelegateCommand<string>(ChangeLanguage);

            ScrollUpCommand = new DelegateCommand(ScrollUp);
            ScrollDownCommand = new DelegateCommand(ScrollDown);

            IsPopupOpen = false;
            ScrollButtonsVisible = true;

            // NOTE:: This is getting called before the service has actually started. Properbly only a problem when developing. 
            CheckServiceStatus();
        }

        private async void CheckServiceStatus() 
        {
            var statusResult = await _statusRestClient.IsAliveAsync();

            if(!statusResult)
            { 
                var notificationStatus = new Notification
                {
                    Title = Resources.NoConnection,
                    Message = Resources.NoConnectionMessage,
                    NotificationType = NotificationTypeEnum.Error
                };
                await ShowNotification(notificationStatus);
            }
        }
        private void ScrollUp()
        {
            _eventAggregator.GetEvent<ScrollUpEvent>().Publish();
        }

        private void ScrollDown()
        {
            _eventAggregator.GetEvent<ScrollDownEvent>().Publish();
        }

        public bool Toggle { get; set; }

        private void Navigate(string uri)
        {
            _regionManager.RequestNavigate(ViewConstants.REGION_Calendar, uri);
        }

        private void ClosePopUp()
        {
            IsPopupOpen = false;
            SetSpinnerVisibility(EventResources.SpinnerEnum.Hide);
        }

        public bool IsPopupOpen
        {
            get => _isPopupOpen;
            set => SetProperty(ref _isPopupOpen, value);
        }

        private void ChangeLanguage(string culture)
        {
            _logger.ForContext("Click", typeof(MainWindowViewModel)).Information("Change language clicked with culture {@Culture}", culture);
            if (LocalizeDictionary.Instance.Culture.Name != culture)
            {
                _logger.ForContext("Language", typeof(MainWindowViewModel)).Information("Changed culture to {@Culture}", culture);
                LocalizeDictionary.Instance.Culture = new CultureInfo(culture);
                _eventAggregator.GetEvent<CultureChangedEvent>().Publish();
                NotificationTitle = Resources.NoConnection;
                NotificationMessage = Resources.NoConnectionMessage;
            }
        }

        public bool SpinnerVisible
        {
            get => _spinnerVisible;
            set => SetProperty(ref _spinnerVisible, value);
        }

        public bool ScrollButtonsVisible
        {
            get => _scrollButtonsVisible;
            set => SetProperty(ref _scrollButtonsVisible, value);
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

        public NotificationTypeEnum NotificationType
        {
            get => _notificationType;
            set => SetProperty(ref _notificationType, value);
        }

        private async void HandleNotificationEvent(Notification notification)
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
                    SetSpinnerVisibility(EventResources.SpinnerEnum.Hide);
                }
            }
            else
            {
                await ShowNotification(notification);
            }
        }

        private Task ShowNotification(Notification notification)
        {
            SetSpinnerVisibility(EventResources.SpinnerEnum.Show);
            NotificationTitle = notification.Title;
            NotificationMessage = notification.Message;
            NotificationType = notification.NotificationType;
            
            IsPopupOpen = true;
            System.Windows.Threading.Dispatcher.CurrentDispatcher.Invoke(() =>
            {
                const string path = "Media/WindowsNotifyCalendar.wav";
                var notificationSound = new SoundPlayer(path);
                notificationSound.Load();
                notificationSound.Play();
            });
            

            return Task.CompletedTask;
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

        private void SetScrollButtonsVisibility(EventResources.ScrollButtonsEnum state)
        {
            switch (state)
            {
                case EventResources.ScrollButtonsEnum.Show:
                    ScrollButtonsVisible = true;
                    break;
                case EventResources.ScrollButtonsEnum.Hide:
                    ScrollButtonsVisible = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }
    }
}
