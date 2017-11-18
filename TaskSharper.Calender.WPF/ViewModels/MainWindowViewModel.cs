using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Media;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Serilog;
using TaskSharper.Calender.WPF.Config;
using TaskSharper.Calender.WPF.Properties;
using TaskSharper.Domain.Calendar;
using TaskSharper.WPF.Common.Events;
using TaskSharper.WPF.Common.Events.NotificationEvents;
using TaskSharper.WPF.Common.Events.Resources;
using TaskSharper.WPF.Common.Events.ScrollEvents;
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

        public DelegateCommand<string> NavigateCommand { get; set; }
        
        public DelegateCommand<string> ChangeLanguageCommand { get; set; }
        public DelegateCommand CloseApplicationCommand { get; set; }
        public DelegateCommand ScrollUpCommand { get; set; }
        public DelegateCommand ScrollDownCommand { get; set; }

        public MainWindowViewModel(IRegionManager regionManager, IEventAggregator eventAggregator, ILogger logger, IStatusRestClient statusRestClient)
        {
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;
            _logger = logger.ForContext<MainWindowViewModel>();
            _statusRestClient = statusRestClient;

            _eventAggregator.GetEvent<SpinnerEvent>().Subscribe(SetSpinnerVisibility);
            _eventAggregator.GetEvent<ScrollButtonsEvent>().Subscribe(SetScrollButtonsVisibility);
 

            NavigateCommand = new DelegateCommand<string>(Navigate);
            ChangeLanguageCommand = new DelegateCommand<string>(ChangeLanguage);
            CloseApplicationCommand = new DelegateCommand(CloseApplication);

            ScrollUpCommand = new DelegateCommand(ScrollUp);
            ScrollDownCommand = new DelegateCommand(ScrollDown);

            ScrollButtonsVisible = true;

            // NOTE:: This is getting called before the service has actually started. Properbly only a problem when developing. 
            //CheckServiceStatus();
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
                _eventAggregator.GetEvent<NotificationEvent>().Publish(notificationStatus);
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

        private void ChangeLanguage(string culture)
        {
            _logger.ForContext("Click", typeof(MainWindowViewModel)).Information("Change language clicked with culture {@Culture}", culture);
            if(LocalizeDictionary.Instance.Culture.Name != culture)
            {
                _logger.ForContext("Language", typeof(MainWindowViewModel)).Information("Changed culture to {@Culture}", culture);
                LocalizeDictionary.Instance.Culture = new CultureInfo(culture);
                _eventAggregator.GetEvent<CultureChangedEvent>().Publish();
            }
        }

        private void CloseApplication()
        {
            Application.Current.Shutdown();
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
