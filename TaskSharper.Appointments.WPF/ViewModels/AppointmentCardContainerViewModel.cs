using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Serilog;
using TaskSharper.Appointments.WPF.Events;
using TaskSharper.Domain.Calendar;

namespace TaskSharper.Appointments.WPF.ViewModels
{
    public class AppointmentCardContainerViewModel : BindableBase, INavigationAware
    {
        private readonly IAppointmentRestClient _dataService;
        private readonly IEventAggregator _eventAggregator;
        private readonly IRegionManager _regionManager;
        private readonly ILogger _logger;
        private bool _isAppointmentSelected;
        private Event _selectedAppointment;

        public ObservableCollection<AppointmentCardViewModel> AppointmentCards { get; set; }

        public DelegateCommand DeleteAppointmentCommand { get; set; }

        public bool IsAppointmentSelected
        {
            get => _isAppointmentSelected;
            set => SetProperty(ref _isAppointmentSelected, value);
        }
        public Event SelectedAppointment
        {
            get => _selectedAppointment;
            set
            {
                IsAppointmentSelected = value != null;
                SetProperty(ref _selectedAppointment, value);
            }
        }

        public AppointmentCardContainerViewModel(IAppointmentRestClient dataService, IEventAggregator eventAggregator, IRegionManager regionManager, ILogger logger)
        {
            _dataService = dataService;
            _eventAggregator = eventAggregator;
            _regionManager = regionManager;
            _logger = logger.ForContext<AppointmentCardContainerViewModel>();

            AppointmentCards = new ObservableCollection<AppointmentCardViewModel>();

            DeleteAppointmentCommand = new DelegateCommand(DeleteAppointment);

            _eventAggregator.GetEvent<AppointmentSelectedEvent>().Subscribe(eventObj =>
            {
                SelectedAppointment = eventObj;
            });
        }

        private async void DeleteAppointment()
        {
            try
            {
                await _dataService.DeleteAsync(SelectedAppointment.Id);
                await UpdateView();
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error while deleting an appointment.");
            }
        }

        private async Task UpdateView()
        {
            var start = DateTime.Today.AddDays(-7).Date;
            var end = DateTime.Today.AddDays(7).Date;
            var events = await _dataService.GetAsync(start, end);

            AppointmentCards?.Clear();
            foreach (var @event in events)
            {
                AppointmentCards?.Add(new AppointmentCardViewModel(_dataService, _eventAggregator, _regionManager, _logger)
                {
                    Event = @event
                });
            }
        }

        public async void OnNavigatedTo(NavigationContext navigationContext)
        {
            await UpdateView();
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            
        }
    }
}
