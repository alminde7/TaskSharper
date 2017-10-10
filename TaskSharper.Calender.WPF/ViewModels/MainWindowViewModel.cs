using System;
using System.Globalization;
using System.Threading;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Serilog;
using TaskSharper.Calender.WPF.Config;
using TaskSharper.Calender.WPF.Events;
using TaskSharper.Calender.WPF.Events.NotificationEvents;
using TaskSharper.Calender.WPF.Events.Resources;
using TaskSharper.Calender.WPF.Views;
using TaskSharper.Domain.BusinessLayer;
using WPFLocalizeExtension.Engine;

namespace TaskSharper.Calender.WPF.ViewModels
{
    public partial class MainWindowViewModel : BindableBase 
    {
        private readonly IRegionManager _regionManager;
        private IEventAggregator _eventAggregator;
        private readonly ILogger _logger;
        private bool _spinnerVisible;
        private bool _isPopupOpen;
        private string _notificationTitle;
        private string _notificationMessage;

        public DelegateCommand<string> NavigateCommand { get; set; }
        public DelegateCommand CloseNotificationCommand { get; set; }
        public DelegateCommand<string> ChangeLanguageCommand { get; set; }

        public MainWindowViewModel(IRegionManager regionManager, IEventAggregator eventAggregator, ILogger logger)
        {
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;
            _logger = logger.ForContext<MainWindowViewModel>();

            _eventAggregator.GetEvent<SpinnerEvent>().Subscribe(SetSpinnerVisibility);
            _eventAggregator.GetEvent<NotificationEvent>().Subscribe(ShowNotification);
            NavigateCommand = new DelegateCommand<string>(Navigate);
            CloseNotificationCommand = new DelegateCommand(ClosePopUp);

            NavigateCommand = new DelegateCommand<string>(Navigate);
            ChangeLanguageCommand = new DelegateCommand<string>(ChangeLanguage);
            IsPopupOpen = false;
        }

        public bool Toggle { get; set; }

        private void Navigate(string uri)
        {
            _regionManager.RequestNavigate(ViewConstants.REGION_Calendar, uri);
        }

        private void ClosePopUp()
        {
            IsPopupOpen = false;
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
            }
        }

        public bool SpinnerVisible
        {
            get => _spinnerVisible;
            set => SetProperty(ref _spinnerVisible, value);
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

        private void ShowNotification(Events.Resources.Notification notification)
        {
            NotificationTitle = notification.Title;
            NotificationMessage = notification.Message;
            IsPopupOpen = true;
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
    }
}
