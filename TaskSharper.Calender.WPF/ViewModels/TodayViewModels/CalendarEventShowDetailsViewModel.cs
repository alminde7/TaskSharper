using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Serilog;
using TaskSharper.Calender.WPF.Config;
using TaskSharper.Domain.BusinessLayer;
using TaskSharper.Domain.Calendar;
using TaskSharper.Shared.Exceptions;
using TaskSharper.WPF.Common.Events.NotificationEvents;
using TaskSharper.WPF.Common.Events.Resources;

namespace TaskSharper.Calender.WPF.ViewModels
{
    public class CalendarEventShowDetailsViewModel : BindableBase , INavigationAware
    {
        private readonly IEventRestClient _calendarService;
        private readonly IRegionManager _regionManager;
        private readonly IEventAggregator _eventAggregator;
        private readonly ILogger _logger;
        private Event _selectedEvent;
        private bool _eventIsTypeTask;
        private bool _eventIsTypeAppointment;
        private bool _eventIsStatusConfirmed;
        private bool _eventIsStatusTentative;

        public DelegateCommand EventDetailsClickCommand { get; set; }
        public DelegateCommand BackCommand { get; set; }

        public bool EventIsStatusConfirmed
        {
            get => _eventIsStatusConfirmed;
            set => SetProperty(ref _eventIsStatusConfirmed, value);
        }

        public bool EventIsStatusTentative
        {
            get => _eventIsStatusTentative;
            set => SetProperty(ref _eventIsStatusTentative, value);
        }

        public bool EventIsTypeTask
        {
            get => _eventIsTypeTask;
            set => SetProperty(ref _eventIsTypeTask, value);
        }

        public bool EventIsTypeAppointment
        {
            get => _eventIsTypeAppointment;
            set => SetProperty(ref _eventIsTypeAppointment, value);
        }

        public Event SelectedEvent
        {
            get => _selectedEvent;
            set => SetProperty(ref _selectedEvent, value);
        }

        public CalendarEventShowDetailsViewModel(IEventRestClient calendarService, IRegionManager regionManager, IEventAggregator eventAggregator, ILogger logger)
        {
            _calendarService = calendarService;
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;
            _logger = logger.ForContext<CalendarEventShowDetailsViewModel>();

            BackCommand = new DelegateCommand(Back);
            EventDetailsClickCommand = new DelegateCommand(EventEditDetailsClick);
        }

        public async void OnNavigatedTo(NavigationContext navigationContext)
        {
            var id = navigationContext.Parameters["id"].ToString();

            try
            {
                SelectedEvent = await _calendarService.GetAsync(id);
            }
            catch (ConnectionException e)
            {
                _eventAggregator.GetEvent<NotificationEvent>().Publish(new ConnectionErrorNotification());
            }
            catch (UnauthorizedAccessException e)
            {
                _eventAggregator.GetEvent<NotificationEvent>().Publish(new UnauthorizedErrorNotification());
                SelectedEvent = new Event() {Type = EventType.None};
            }
            catch (Exception e)
            {
                _logger.Error(e, "Exception was thrown.");
            }

            EventIsTypeTask = SelectedEvent.Type == EventType.Task;
            EventIsTypeAppointment = SelectedEvent.Type == EventType.Appointment;
            EventIsStatusConfirmed = SelectedEvent.Status == EventStatus.Confirmed;
            EventIsStatusTentative = SelectedEvent.Status == EventStatus.Tentative;

        }

        private void EventEditDetailsClick()
        {
            Navigate(ViewConstants.VIEW_CalendarEventDetails);
        }
        private void Back()
        {
            _regionManager.Regions["CalendarRegion"].NavigationService.Journal.GoBack();
        }
        private void Navigate(string uri)
        {
            _regionManager.RequestNavigate(ViewConstants.REGION_Calendar, uri + $"?id={_selectedEvent.Id}");
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
