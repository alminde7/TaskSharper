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
using TaskSharper.Appointments.WPF.Config;
using TaskSharper.Appointments.WPF.Events;
using TaskSharper.Domain.Calendar;
using TaskSharper.Shared.Exceptions;
using TaskSharper.WPF.Common.Events.NotificationEvents;
using TaskSharper.WPF.Common.Events.Resources;
using TaskSharper.WPF.Common.Events.ScrollEvents;
using TaskSharper.WPF.Common.Events.ViewEvents;

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

        public DelegateCommand AddAppointmentCommand { get; set; }
        public DelegateCommand DeleteAppointmentCommand { get; set; }
        public DelegateCommand ScrollUpCommand { get; set; }
        public DelegateCommand ScrollDownCommand { get; set; }

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

            AddAppointmentCommand = new DelegateCommand(NavigateToAddAppointment);
            DeleteAppointmentCommand = new DelegateCommand(DeleteAppointment);
            ScrollUpCommand = new DelegateCommand(ScrollUp);
            ScrollDownCommand = new DelegateCommand(ScrollDown);

            _eventAggregator.GetEvent<AppointmentSelectedEvent>().Subscribe(eventObj =>
            {
                SelectedAppointment = eventObj;
            });
        }

        private void NavigateToAddAppointment()
        {
            var navigationParameters = new NavigationParameters();
            navigationParameters.Add("Type", EventType.Appointment);
            navigationParameters.Add("Region", ViewConstants.REGION_Main);
            _regionManager.RequestNavigate(ViewConstants.REGION_Main, ViewConstants.VIEW_ModifyAppointmentView, navigationParameters);
        }

        private async void DeleteAppointment()
        {
            try
            {
                await _dataService.DeleteAsync(SelectedAppointment.Id, SelectedAppointment.Category.Id);
                await UpdateView();
                IsAppointmentSelected = false;
            }
            catch (ConnectionException)
            {
                _eventAggregator.GetEvent<NotificationEvent>().Publish(new ConnectionErrorNotification());
            }
            catch (UnauthorizedAccessException)
            {
                _eventAggregator.GetEvent<NotificationEvent>().Publish(new UnauthorizedErrorNotification());
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error while deleting an appointment");
            }
        }

        private async Task UpdateView()
        {
            var start = DateTime.Today.AddDays(-7).Date;
            var end = DateTime.Today.AddDays(7).Date;
            var events = new List<Event>();
            try
            {
                var result = await _dataService.GetAsync(start, end);
                events = result.ToList();
            }
            catch (ConnectionException)
            {
                _eventAggregator.GetEvent<NotificationEvent>().Publish(new ConnectionErrorNotification());
            }
            catch (UnauthorizedAccessException)
            {
                _eventAggregator.GetEvent<NotificationEvent>().Publish(new UnauthorizedErrorNotification());
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error while getting appointments");
            }

            AppointmentCards?.Clear();
            foreach (var @event in events.OrderBy(o => o.Start).ThenBy(o => o.End))
            {
                AppointmentCards?.Add(new AppointmentCardViewModel(_dataService, _eventAggregator, _regionManager, _logger)
                {
                    Appointment = @event
                });
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

        public async void OnNavigatedTo(NavigationContext navigationContext)
        {
            _eventAggregator.GetEvent<BackButtonEvent>().Publish(BackButtonStatus.Hide);
            await UpdateView();
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            _eventAggregator.GetEvent<BackButtonEvent>().Publish(BackButtonStatus.Show);
        }
    }
}
