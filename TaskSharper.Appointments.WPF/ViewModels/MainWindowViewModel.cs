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
    public class MainWindowViewModel : BindableBase
    {
        private readonly IRegionManager _regionManager;
        private readonly IEventAggregator _eventAggregator;
        private readonly ILogger _logger;
        private readonly IStatusRestClient _statusRestClient;
        private bool _spinnerVisible;
        private bool _isAppointmentSelected;
        
        public DelegateCommand<string> ChangeLanguageCommand { get; set; }
        public DelegateCommand CloseApplicationCommand { get; set; }
        public DelegateCommand ScrollUpCommand { get; set; }
        public DelegateCommand ScrollDownCommand { get; set; }

        public bool SpinnerVisible
        {
            get => _spinnerVisible;
            set => SetProperty(ref _spinnerVisible, value);
        }

        public bool IsAppointmentSelected
        {
            get => _isAppointmentSelected;
            set => SetProperty(ref _isAppointmentSelected, value);
        }

        public MainWindowViewModel(IRegionManager regionManager, IEventAggregator eventAggregator, ILogger logger, IStatusRestClient statusRestClient)
        {
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;
            _logger = logger.ForContext<MainWindowViewModel>();
            _statusRestClient = statusRestClient;

            _eventAggregator.GetEvent<SpinnerEvent>().Subscribe(SetSpinnerVisibility);
            _eventAggregator.GetEvent<AppointmentSelectedEvent>().Subscribe(eventObj => IsAppointmentSelected = true);
            
            ChangeLanguageCommand = new DelegateCommand<string>(ChangeLanguage);
            CloseApplicationCommand = new DelegateCommand(CloseApplication);
            ScrollUpCommand = new DelegateCommand(ScrollUp);
            ScrollDownCommand = new DelegateCommand(ScrollDown);
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

        private void CloseApplication()
        {
            Application.Current.Shutdown();
        }

        private void Navigate(string uri)
        {
            _regionManager.RequestNavigate(ViewConstants.REGION_Main, uri);
        }

        private void ScrollUp()
        {
            _eventAggregator.GetEvent<ScrollUpEvent>().Publish();
        }

        private void ScrollDown()
        {
            _eventAggregator.GetEvent<ScrollDownEvent>().Publish();
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
