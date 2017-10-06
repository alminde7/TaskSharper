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

        public DelegateCommand<string> NavigateCommand { get; set; }

        public MainWindowViewModel(IRegionManager regionManager, IEventAggregator eventAggregator, ILogger logger)
        {
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;
            _logger = logger;

            _eventAggregator.GetEvent<SpinnerEvent>().Subscribe(SetSpinnerVisibility);

            NavigateCommand = new DelegateCommand<string>(Navigate);
        }

        public bool Toggle { get; set; }

        private void Navigate(string uri)
        {
            LocalizeDictionary.Instance.Culture = (Toggle ? new CultureInfo("da-DK") : new CultureInfo("en-US"));
            Thread.CurrentThread.CurrentUICulture = (Toggle ? new CultureInfo("da-DK") : new CultureInfo("en-US"));
            Toggle = !Toggle;
            _regionManager.RequestNavigate(ViewConstants.REGION_Calendar, uri);
        }

        public bool SpinnerVisible
        {
            get => _spinnerVisible;
            set => SetProperty(ref _spinnerVisible, value);
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
