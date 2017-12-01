using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Serilog;
using TaskSharper.Appointments.WPF.Config;
using TaskSharper.Appointments.WPF.Events;
using TaskSharper.Appointments.WPF.Properties;
using TaskSharper.Domain.Calendar;
using TaskSharper.WPF.Common.Events;
using TaskSharper.WPF.Common.Events.Resources;
using TaskSharper.WPF.Common.Events.ScrollEvents;
using WPFLocalizeExtension.Engine;

namespace TaskSharper.Appointments.WPF.ViewModels
{
    /// <inheritdoc />
    /// <summary>
    /// ViewModel for the MainWindow view of the Appointment application.
    /// </summary>
    public class MainWindowViewModel : BindableBase
    {
        private readonly IRegionManager _regionManager;
        private readonly IEventAggregator _eventAggregator;
        private readonly ILogger _logger;
        private readonly IStatusRestClient _statusRestClient;
        private bool _spinnerVisible;
        private bool _isAppointmentSelected;

        public DelegateCommand<string> NavigateCommand { get; set; }
        public DelegateCommand BackCommand { get; set; }
        public DelegateCommand<string> ChangeLanguageCommand { get; set; }
        public DelegateCommand CloseApplicationCommand { get; set; }

        /// <summary>
        /// Used to bind in the view whether or not the loading spinner should be visible.
        /// </summary>
        public bool SpinnerVisible
        {
            get => _spinnerVisible;
            set => SetProperty(ref _spinnerVisible, value);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="regionManager">Regionmanager used for navigation</param>
        /// <param name="eventAggregator">Event aggregator for subscribing to and publishing events</param>
        /// <param name="logger">Logger for logging</param>
        /// <param name="statusRestClient">Rest client for the Status service</param>
        public MainWindowViewModel(IRegionManager regionManager, IEventAggregator eventAggregator, ILogger logger, IStatusRestClient statusRestClient)
        {
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;
            _logger = logger.ForContext<MainWindowViewModel>();
            _statusRestClient = statusRestClient;

            _eventAggregator.GetEvent<SpinnerEvent>().Subscribe(SetSpinnerVisibility);

            NavigateCommand = new DelegateCommand<string>(Navigate);
            BackCommand = new DelegateCommand(Back);
            ChangeLanguageCommand = new DelegateCommand<string>(ChangeLanguage);
            CloseApplicationCommand = new DelegateCommand(CloseApplication);
        }

        /// <summary>
        /// Handler for the Back-button.
        /// </summary>
        private void Back()
        {
            _regionManager.Regions[ViewConstants.REGION_Main].NavigationService.Journal.GoBack();
        }

        /// <summary>
        /// Handler for changing language using the language flag icons in the view.
        /// </summary>
        /// <param name="culture">Name of the culture, eg. da-DK or en-US</param>
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

        /// <summary>
        /// Handler for the close application button.
        /// </summary>
        private void CloseApplication()
        {
            Application.Current.Shutdown();
        }

        /// <summary>
        /// Method for navigating in the view.
        /// </summary>
        /// <param name="uri">Name of the view that should be navigated to</param>
        private void Navigate(string uri)
        {
            _regionManager.RequestNavigate(ViewConstants.REGION_Main, uri);
        }

        /// <summary>
        /// Handler for hiding or showing the loading spinner.
        /// </summary>
        /// <param name="state">Possible values are: Show, Hide</param>
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
