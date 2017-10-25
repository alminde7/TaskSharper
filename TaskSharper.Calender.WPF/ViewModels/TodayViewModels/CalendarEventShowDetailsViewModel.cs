using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using TaskSharper.Calender.WPF.Config;
using TaskSharper.Domain.BusinessLayer;
using TaskSharper.Domain.Calendar;

namespace TaskSharper.Calender.WPF.ViewModels
{
    public class CalendarEventShowDetailsViewModel : BindableBase , INavigationAware
    {
        private readonly IEventManager _calendarService;
        private readonly IRegionManager _regionManager;
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

        public CalendarEventShowDetailsViewModel(IEventManager calendarService, IRegionManager regionManager)
        {
            _calendarService = calendarService;
            _regionManager = regionManager;

            BackCommand = new DelegateCommand(Back);
            EventDetailsClickCommand = new DelegateCommand(EventEditDetailsClick);
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            var id = navigationContext.Parameters["id"].ToString();

            SelectedEvent = _calendarService.GetEvent(id);

            EventIsTypeTask = SelectedEvent.Type == Event.EventType.Task;
            EventIsTypeAppointment = SelectedEvent.Type == Event.EventType.Appointment;
            EventIsStatusConfirmed = SelectedEvent.Status == Event.EventStatus.Confirmed ||
                                     SelectedEvent.Status == Event.EventStatus.Completed;
            EventIsStatusTentative = SelectedEvent.Status == Event.EventStatus.Tentative;

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
