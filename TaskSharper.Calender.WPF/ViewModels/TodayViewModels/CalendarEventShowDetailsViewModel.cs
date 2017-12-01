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
    /// <inheritdoc cref="BindableBase" />
    /// <inheritdoc cref="INavigationAware" />
    /// <summary>
    /// ViewModel for the CalendarEvent details user control.
    /// </summary>
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

        /// <summary>
        /// Command that is used to bind to the Event details click event.
        /// </summary>
        public DelegateCommand EventDetailsClickCommand { get; set; }

        /// <summary>
        /// Command that is used to bind to the Back button click event.
        /// </summary>
        public DelegateCommand BackCommand { get; set; }

        /// <summary>
        /// Command that is used to bind to the Delete button click event.
        /// </summary>
        public DelegateCommand EventDeleteCommand { get; set; }

        /// <summary>
        /// Whether or not the status of the event is "Confirmed".
        /// </summary>
        public bool EventIsStatusConfirmed
        {
            get => _eventIsStatusConfirmed;
            set => SetProperty(ref _eventIsStatusConfirmed, value);
        }

        /// <summary>
        /// Whether or not the status of the event is Tentative.
        /// </summary>
        public bool EventIsStatusTentative
        {
            get => _eventIsStatusTentative;
            set => SetProperty(ref _eventIsStatusTentative, value);
        }

        /// <summary>
        /// Whether or not the event is a task.
        /// </summary>
        public bool EventIsTypeTask
        {
            get => _eventIsTypeTask;
            set => SetProperty(ref _eventIsTypeTask, value);
        }

        /// <summary>
        /// Whether or not the event is an appointment.
        /// </summary>
        public bool EventIsTypeAppointment
        {
            get => _eventIsTypeAppointment;
            set => SetProperty(ref _eventIsTypeAppointment, value);
        }

        /// <summary>
        /// Whether or not the event is completed.
        /// </summary>
        public bool EventIsNotCompleted
        {
            get => _eventIsNotCompleted;
            set => SetProperty(ref _eventIsNotCompleted, value);
        }

        /// <summary>
        /// The event that is used for data binding in the view.
        /// </summary>
        public Event SelectedEvent
        {
            get => _selectedEvent;
            set => SetProperty(ref _selectedEvent, value);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="calendarService">Calendar service for data management</param>
        /// <param name="regionManager">Regionmanager used for navigation</param>
        /// <param name="eventAggregator">Event aggregator for subscribing to and publishing events</param>
        /// <param name="logger">Logger for logging</param>
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

        /// <inheritdoc />
        /// <summary>
        /// Implementation of the OnNavigatedTo method.
        /// Defines what happens when view is navigated to the CalendarEventShowDetails view.
        /// </summary>
        /// <param name="navigationContext">Navigation context that contains information for the navigation request.</param>
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

        /// <summary>
        /// Handler for clicking the Edit button.
        /// </summary>
        private void EventEditDetailsClick()
        {
            var navigationParameters = new NavigationParameters();
            navigationParameters.Add("Id", SelectedEvent.Id);
            navigationParameters.Add("Type", SelectedEvent.Type);
            navigationParameters.Add("Region", ViewConstants.REGION_Calendar);
            _regionManager.RequestNavigate(ViewConstants.REGION_Calendar, ViewConstants.VIEW_CalendarEventDetails, navigationParameters);
        }

        /// <summary>
        /// Handler for clicking the Delete button.
        /// </summary>
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

        /// <summary>
        /// Handler for clicking the back button.
        /// </summary>
        private void Back()
        {
            _regionManager.Regions["CalendarRegion"].NavigationService.Journal.GoBack();
        }

        /// <inheritdoc />
        /// <summary>
        /// Implementation of the IsNavigationTarget method.
        /// </summary>
        /// <param name="navigationContext">Navigation context that contains information for the navigation request.</param>
        /// <returns>True</returns>
        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        /// <inheritdoc />
        /// <summary>
        /// Implementation of the OnNavigatedFrom method.
        /// Defines what happens when the view is navigated away from the EventModification view.
        /// </summary>
        /// <param name="navigationContext">Navigation context that contains information for the navigation request.</param>
        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
           
        }
    }
}
