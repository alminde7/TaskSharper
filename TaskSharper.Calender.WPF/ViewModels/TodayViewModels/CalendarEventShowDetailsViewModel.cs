using System;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Serilog;
using TaskSharper.Calender.WPF.Config;
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
        private bool _eventIsNotCompleted; // XAML cannot do the NOT-operator ("!") without a custom converter, so this is done instead

        public DelegateCommand EventDetailsClickCommand { get; set; }
        public DelegateCommand BackCommand { get; set; }
        public DelegateCommand EventDeleteCommand { get; set; }

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

        public bool EventIsNotCompleted
        {
            get => _eventIsNotCompleted;
            set => SetProperty(ref _eventIsNotCompleted, value);
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
            EventDeleteCommand = new DelegateCommand(DeleteEvent);
        }

        public async void OnNavigatedTo(NavigationContext navigationContext)
        {
            var id = navigationContext.Parameters["id"].ToString();

            try
            {
                SelectedEvent = await _calendarService.GetAsync(id);
                EventIsNotCompleted = !SelectedEvent.MarkedAsDone;
            }
            catch (ConnectionException)
            {
                _eventAggregator.GetEvent<NotificationEvent>().Publish(new ConnectionErrorNotification());
            }
            catch (UnauthorizedAccessException)
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
            var navigationParameters = new NavigationParameters();
            navigationParameters.Add("Id", SelectedEvent.Id);
            navigationParameters.Add("Type", SelectedEvent.Type);
            navigationParameters.Add("Region", ViewConstants.REGION_Calendar);
            _regionManager.RequestNavigate(ViewConstants.REGION_Calendar, ViewConstants.VIEW_CalendarEventDetails, navigationParameters);
        }

        private async void DeleteEvent()
        {
            try
            {
                await _calendarService.DeleteAsync(SelectedEvent.Id, SelectedEvent.Category.Id);
                Back();
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
                _logger.Error(e, "Error while deleting an event.");
                _eventAggregator.GetEvent<NotificationEvent>().Publish(new Notification
                {
                    Event = SelectedEvent,
                    Title = "Error!",
                    Message = "Something went wrong. Please try again. If this continues to occur, contact an administrator.",
                    NotificationType = NotificationTypeEnum.Error
                });
            }

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
