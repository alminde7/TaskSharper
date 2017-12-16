using System;
using System.Globalization;
using System.Windows;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Serilog;
using TaskSharper.Calender.WPF.Config;
using TaskSharper.Domain.Calendar;
using TaskSharper.Domain.RestClient;
using TaskSharper.WPF.Common.Components.SetCulture;
using TaskSharper.WPF.Common.Events;
using TaskSharper.WPF.Common.Events.NotificationEvents;
using TaskSharper.WPF.Common.Events.Resources;
using TaskSharper.WPF.Common.Events.ScrollEvents;
using TaskSharper.WPF.Common.Properties;
using WPFLocalizeExtension.Engine;


namespace TaskSharper.Calender.WPF.ViewModels
{
    /// <inheritdoc />
    /// <summary>
    /// ViewModel for the MainWindow window.
    /// </summary>
    public partial class MainWindowViewModel : BindableBase 
    {
        private readonly IRegionManager _regionManager;
        private IEventAggregator _eventAggregator;
        private IStatusRestClient _statusRestClient;
        private readonly ILogger _logger;
        private bool _spinnerVisible;
        private bool _scrollButtonsVisible;
        private Culture _culture;

        /// <summary>
        /// Command that is used when navigating to Month, Week or Day views.
        /// </summary>
        public DelegateCommand<string> NavigateCommand { get; set; }
        
        /// <summary>
        /// Command that is used when changing language.
        /// </summary>
        public DelegateCommand<string> ChangeLanguageCommand { get; set; }

        /// <summary>
        /// Command that is used when clicking the Home-icon to close the application.
        /// </summary>
        public DelegateCommand CloseApplicationCommand { get; set; }

        /// <summary>
        /// Command that is used when clicking the scroll up button.
        /// </summary>
        public DelegateCommand ScrollUpCommand { get; set; }

        /// <summary>
        /// Command that is used when clicking the scroll down button.
        /// </summary>
        public DelegateCommand ScrollDownCommand { get; set; }

        /// <summary>
        /// Constructor. The parameters can be dependency injected.
        /// </summary>
        /// <param name="regionManager">Region manager for navigation</param>
        /// <param name="eventAggregator">Event aggregator for subscribing to and publishing events</param>
        /// <param name="logger">Logger for logging</param>
        /// <param name="statusRestClient">Rest client to the Status service</param>
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

            _culture = new Culture();

            // NOTE:: This is getting called before the service has actually started. Properbly only a problem when developing. 
            //CheckServiceStatus();
        }

        /// <summary>
        /// Checking the service status.
        /// </summary>
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

        /// <summary>
        /// Event handler for the scroll up event.
        /// </summary>
        private void ScrollUp()
        {
            _eventAggregator.GetEvent<ScrollUpEvent>().Publish();
        }

        /// <summary>
        /// Event handler for the scroll down event.
        /// </summary>
        private void ScrollDown()
        {
            _eventAggregator.GetEvent<ScrollDownEvent>().Publish();
        }

        public bool Toggle { get; set; }

        /// <summary>
        /// To navigate to a different view.
        /// </summary>
        /// <param name="uri">URI of the view to be navigated to</param>
        private void Navigate(string uri)
        {
            _regionManager.RequestNavigate(ViewConstants.REGION_Calendar, uri);
        }

        /// <summary>
        /// Handler for changing the localization/language of the application.
        /// </summary>
        /// <param name="culture">Name of the culture. Eg. da-DK or en-US.</param>
        private void ChangeLanguage(string culture)
        {
            _logger.ForContext("Click", typeof(MainWindowViewModel)).Information("Change language clicked with culture {@Culture}", culture);
            if(LocalizeDictionary.Instance.Culture.Name != culture)
            {
                _logger.ForContext("Language", typeof(MainWindowViewModel)).Information("Changed culture to {@Culture}", culture);
                _culture.Set(culture);
                _eventAggregator.GetEvent<CultureChangedEvent>().Publish();
            }
        }

        /// <summary>
        /// Handler for closing the application.
        /// </summary>
        private void CloseApplication()
        {
            Application.Current.Shutdown();
        }

        /// <summary>
        /// To binding in the view for whether or not the loading spinner is visible.
        /// </summary>
        public bool SpinnerVisible
        {
            get => _spinnerVisible;
            set => SetProperty(ref _spinnerVisible, value);
        }

        /// <summary>
        /// To binding in the view for whether or not the scroll buttons are visible.
        /// </summary>
        public bool ScrollButtonsVisible
        {
            get => _scrollButtonsVisible;
            set => SetProperty(ref _scrollButtonsVisible, value);
        }

        /// <summary>
        /// To set the spinner visiblity.
        /// </summary>
        /// <param name="state">Show or Hide</param>
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

        /// <summary>
        /// To set the scroll buttons visibility.
        /// </summary>
        /// <param name="state">Show or hide</param>
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
