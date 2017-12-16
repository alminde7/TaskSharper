using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Serilog;
using TaskSharper.Domain.Calendar;
using TaskSharper.Domain.Models;
using TaskSharper.Domain.RestClient;
using TaskSharper.Shared.Exceptions;
using TaskSharper.WPF.Common.Events;
using TaskSharper.WPF.Common.Events.NotificationEvents;
using TaskSharper.WPF.Common.Events.Resources;
using TaskSharper.WPF.Common.Properties;

namespace TaskSharper.WPF.Common.Components.EventModification
{
    /// <inheritdoc cref="BindableBase" />
    /// <inheritdoc cref="INavigationAware"/>
    /// <summary>
    /// ViewModel for the EventModification component.
    /// </summary>
    public class EventModificationViewModel : BindableBase, INavigationAware
    {
        /// <summary>
        /// Enum for the type of modification. Possible values are:
        /// Create
        /// Edit
        /// </summary>
        enum ModificationType
        {
            Create,
            Edit
        }

        private ModificationType _modificationType;

        public DelegateCommand BackCommand { get; set; }
        public DelegateCommand SaveEventCommand { get; set; }
        public DelegateCommand CancelCommand { get; set; }
        public DelegateCommand SetTypeAsAppointmentCommand { get; set; }
        public DelegateCommand SetTypeAsTaskCommand { get; set; }
        public DelegateCommand SetStatusAsTentativeCommand { get; set; }
        public DelegateCommand SetStatusAsConfirmedCommand { get; set; }

        public ObservableCollection<CategoryViewModel> Categories { get; set; }

        private readonly IRegionManager _regionManager;
        private readonly IEventAggregator _eventAggregator;
        private readonly IEventRestClient _dataService;
        private readonly ILogger _logger;
        private string _region;
        
        private Event _event;
        private IEnumerable<EventType> _eventTypes;
        private IEnumerable<EventStatus> _eventStatuses;
        private double _taskOpacity = Settings.Default.HiddenOpacity;
        private double _appointmentOpacity = Settings.Default.HiddenOpacity;
        private double _confirmedOpacity = Settings.Default.NotSelectedOpacity;
        private double _tentativeOpacity = Settings.Default.NotSelectedOpacity;
        private string _dateTimeErrorMessage;
        private string _titleErrorMessage;
        private bool _isTaskTypeVisible;
        private bool _isAppointmentTypeVisible;
        
        /// <summary>
        /// Event that is used for view-binding.
        /// </summary>
        public Event Event
        {
            get => _event;
            set => SetProperty(ref _event, value);
        }

        /// <summary>
        /// Possible values of the EventType enum.
        /// </summary>
        public IEnumerable<EventType> EventTypes
        {
            get => _eventTypes;
            set => SetProperty(ref _eventTypes, value);
        }

        /// <summary>
        /// Possible values of the EventStatus enum.
        /// </summary>
        public IEnumerable<EventStatus> EventStatuses
        {
            get => _eventStatuses;
            set => SetProperty(ref _eventStatuses, value);
        }

        /// <summary>
        /// Binding value used for opacity of the task icon. Ranges between 0.0-1.0.
        /// Default value for when task is selected: 1.0
        /// Default value for when task is not selected: 0.5
        /// If this is 0, the task icon is hidden, but still takes up space in the view.
        /// </summary>
        public double TaskOpacity
        {
            get => _taskOpacity;
            set => SetProperty(ref _taskOpacity, value);
        }

        /// <summary>
        /// Binding value used for opacity of the appointment icon. Ranges between 0.0-1.0.
        /// Default value for when appointment is selected: 1.0
        /// Default value for when appointment is not selected: 0.5
        /// If this is 0, the appointment icon is hidden, but still takes up space in the view.
        /// </summary>
        public double AppointmentOpacity
        {
            get => _appointmentOpacity;
            set => SetProperty(ref _appointmentOpacity, value);
        }

        /// <summary>
        /// If this is false, the task icon is collapsed and not taking up space in the view.
        /// </summary>
        public bool IsTaskTypeVisible
        {
            get => _isTaskTypeVisible;
            set => SetProperty(ref _isTaskTypeVisible, value);
        }

        /// <summary>
        /// If this is false, the appointment icon is collapsed and not taking up space in the view.
        /// </summary>
        public bool IsAppointmentTypeVisible
        {
            get => _isAppointmentTypeVisible;
            set => SetProperty(ref _isAppointmentTypeVisible, value);
        }

        /// <summary>
        /// Binding value used for opacity of the confirmed icon. Ranges between 0.0-1.0.
        /// Default value for when confirmed is selected: 1.0
        /// Default value for when confirmed is not selected: 0.5
        /// If this is 0, the confirmed icon is hidden, but still takes up space in the view.
        /// </summary>
        public double ConfirmedOpacity
        {
            get => _confirmedOpacity;
            set => SetProperty(ref _confirmedOpacity, value);
        }

        /// <summary>
        /// Binding value used for opacity of the tentative icon. Ranges between 0.0-1.0.
        /// Default value for when tentative is selected: 1.0
        /// Default value for when tentative is not selected: 0.5
        /// If this is 0, the tentative icon is hidden, but still takes up space in the view.
        /// </summary>
        public double TentativeOpacity
        {
            get => _tentativeOpacity;
            set => SetProperty(ref _tentativeOpacity, value);
        }

        /// <summary>
        /// Binding value for title error messages.
        /// </summary>
        public string TitleErrorMessage
        {
            get => _titleErrorMessage;
            set => SetProperty(ref _titleErrorMessage, value);
        }

        /// <summary>
        /// Binding value for date time picker error messages.
        /// </summary>
        public string DateTimeErrorMessage
        {
            get => _dateTimeErrorMessage;
            set => SetProperty(ref _dateTimeErrorMessage, value);
        }

        /// <summary>
        /// Sets type and updates the opacity of type icons.
        /// </summary>
        /// <param name="type">Type of the event</param>
        public void SetType(EventType type)
        {
            TaskOpacity = Settings.Default.NotSelectedOpacity;
            AppointmentOpacity = Settings.Default.NotSelectedOpacity;
            switch (type)
            {
                case EventType.Task:
                    Event.Type = EventType.Task;
                    TaskOpacity = Settings.Default.SelectedOpacity;
                    break;
                case EventType.Appointment:
                    Event.Type = EventType.Appointment;
                    AppointmentOpacity = Settings.Default.SelectedOpacity;
                    break;
                case EventType.None:
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Sets status and updates the opacity of status icons.
        /// </summary>
        /// <param name="status">Status of the event</param>
        public void SetStatus(EventStatus status)
        {
            ConfirmedOpacity = Settings.Default.NotSelectedOpacity;
            TentativeOpacity = Settings.Default.NotSelectedOpacity;
            switch (status)
            {
                case EventStatus.Confirmed:
                    Event.Status = EventStatus.Confirmed;
                    ConfirmedOpacity = Settings.Default.SelectedOpacity;
                    break;
                case EventStatus.Tentative:
                    Event.Status = EventStatus.Tentative;
                    TentativeOpacity = Settings.Default.SelectedOpacity;
                    break;
                case EventStatus.Cancelled:
                    break;
                case EventStatus.Completed:
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="regionManager">Regionmanager used for navigation</param>
        /// <param name="dataService">Data service for data management</param>
        /// <param name="eventAggregator">Event aggregator for subscribing to and publishing events</param>
        /// <param name="logger">Logger for logging</param>
        public EventModificationViewModel(IRegionManager regionManager, IEventRestClient dataService, IEventAggregator eventAggregator, ILogger logger)
        {
            _regionManager = regionManager;
            _dataService = dataService;
            _eventAggregator = eventAggregator;
            _logger = logger.ForContext<EventModificationViewModel>();

            Categories = new ObservableCollection<CategoryViewModel>();

            BackCommand = new DelegateCommand(Back);
            SaveEventCommand = new DelegateCommand(SaveEvent);
            CancelCommand = new DelegateCommand(Cancel);
            SetTypeAsTaskCommand = new DelegateCommand(() => SetType(EventType.Task));
            SetTypeAsAppointmentCommand = new DelegateCommand(() => SetType(EventType.Appointment));
            SetStatusAsConfirmedCommand = new DelegateCommand(() => SetStatus(EventStatus.Confirmed));
            SetStatusAsTentativeCommand = new DelegateCommand(() => SetStatus(EventStatus.Tentative));

            EventTypes = Enum.GetValues(typeof(EventType)).Cast<EventType>();
            EventStatuses = Enum.GetValues(typeof(EventStatus)).Cast<EventStatus>().Except(new List<EventStatus> { EventStatus.Cancelled });
            Event = new Event
            {
                Start = DateTime.Today,
                End = DateTime.Today
            }; // Temporarily assign an empty event so DateTimePicker can bind to a non-null object (this will be properly set in the OnNavigatedTo method)
            _eventAggregator.GetEvent<CultureChangedEvent>().Subscribe(CultureChanged);
            _eventAggregator.GetEvent<CategoryClickedEvent>().Subscribe(CategoryClicked);
        }

        /// <summary>
        /// Handler for clicking a category.
        /// </summary>
        /// <param name="category">Category for the event</param>
        private void CategoryClicked(EventCategory category)
        {
            Event.Category = category;
        }

        /// <summary>
        /// Handler for when language changes.
        /// </summary>
        private void CultureChanged()
        {
            ValidateEvent(Event);
        }

        /// <summary>
        /// Handler for saving the event.
        /// </summary>
        public async void SaveEvent()
        {
            if (!ValidateEvent(Event))
            {
                try
                {
                    switch (_modificationType)
                    {
                        case ModificationType.Create:
                            Event = await _dataService.CreateAsync(Event);
                            break;
                        case ModificationType.Edit:
                            Event = await _dataService.UpdateAsync(Event);
                            break;
                    }
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
                    _logger.Error(e, "Error while modifying an event.");
                }

                _regionManager.Regions[_region].NavigationService.Journal.GoBack();
            }
        }

        public bool ValidateEvent(Event eventObj)
        {
            var hasError = false;
            if (string.IsNullOrEmpty(eventObj.Title))
            {
                TitleErrorMessage = Resources.ErrorTitleNotSet;
                hasError = true;
            }
            if (eventObj.Start > eventObj.End)
            {
                DateTimeErrorMessage = Resources.ErrorEndTimeIsEarlierThanStartTime;
                hasError = true;
            }
            if (eventObj.Start < DateTime.Today)
            {
                DateTimeErrorMessage = Resources.ErrorStartTimeIsBeforeTodaysDate;
                hasError = true;
            }
            if (eventObj.End?.Date > eventObj.Start?.Date)
            {
                DateTimeErrorMessage = Resources.ErrorEventSpansAccrossMultipleDays;
                hasError = true;
            }
            return hasError;
        }

        /// <summary>
        /// Handler for the cancel button.
        /// </summary>
        private void Cancel()
        {
            _regionManager.Regions[_region].NavigationService.Journal.GoBack();
        }

        /// <inheritdoc />
        /// <summary>
        /// Implementation of the OnNavigatedTo method.
        /// Defines what happens when view is navigated to the EventModification view.
        /// </summary>
        /// <param name="navigationContext">Navigation context that contains information for the navigation request.</param>
        public async void OnNavigatedTo(NavigationContext navigationContext)
        {
            var id = navigationContext?.Parameters["Id"]?.ToString();
            _region = navigationContext?.Parameters["Region"]?.ToString();
            
            if (id != null)
            {
                Event = _dataService.Get(id);
                SetType(Event.Type);
                SetStatus(Event.Status);
                _modificationType = ModificationType.Edit;
            }
            else
            {
                _modificationType = ModificationType.Create;
                Event = new Event
                {
                    Start = DateTime.Today.AddHours(DateTime.Now.Hour + 1),
                    End = DateTime.Today.AddHours(DateTime.Now.Hour + 2)
                };
            }

            Event.Type = navigationContext?.Parameters["Type"] is EventType eventType ? eventType : EventType.None;
            SetType(Event.Type);

            switch (Event.Type)
            {
                case EventType.None:
                    IsAppointmentTypeVisible = true;
                    IsTaskTypeVisible = true;
                    break;
                case EventType.Appointment:
                    IsAppointmentTypeVisible = true;
                    break;
                case EventType.Task:
                    IsTaskTypeVisible = true;
                    break;
            }

            var categories = new List<EventCategory>();
            try
            {
                var result = await _dataService.GetAsync();
                categories = result.ToList();
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
                _logger.Error(e, "Error while getting categories.");
            }

            Categories?.Clear();
            foreach (var eventCategory in categories)
            {
                var categoryViewModel = new CategoryViewModel(_regionManager, _eventAggregator, _dataService)
                {
                    Id = eventCategory.Id,
                    Type = Event.Type,
                    Category = eventCategory.Name
                };
                
                if (_modificationType == ModificationType.Edit && Event.Category.Id == eventCategory.Id)
                {
                    categoryViewModel.CategoryOpacity = Settings.Default.SelectedOpacity;
                }

                Categories?.Add(categoryViewModel);
            }
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
            TitleErrorMessage = null;
            DateTimeErrorMessage = null;
        }

        /// <summary>
        /// Handler for clicking the back button.
        /// </summary>
        private void Back()
        {
            _regionManager.Regions[_region].NavigationService.Journal.GoBack();
        }
    }
}
