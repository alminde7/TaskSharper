using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Serilog;
using TaskSharper.Domain.Calendar;
using TaskSharper.Shared.Exceptions;
using TaskSharper.WPF.Common.Events;
using TaskSharper.WPF.Common.Events.NotificationEvents;
using TaskSharper.WPF.Common.Events.Resources;
using TaskSharper.WPF.Common.Properties;

namespace TaskSharper.WPF.Common.Components.EventModification
{
    public class EventModificationViewModel : BindableBase, INavigationAware
    {
        enum ModificationType
        {
            Create,
            Edit
        }

        private ModificationType _modificationType;

        public DelegateCommand BackCommand { get; set; }
        public DelegateCommand SaveEventCommand { get; set; }
        public DelegateCommand CancelCommand { get; set; }
        public DelegateCommand KeyboardCommand { get; set; }
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

        private Process _touchKeyboardProcess;
        private string touchKeyboardPath = @"C:\Program Files\Common Files\Microsoft Shared\Ink\TabTip.exe";
        private string _title;
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

        public Process TouchKeyboardProcess
        {
            get => _touchKeyboardProcess;
            set => SetProperty(ref _touchKeyboardProcess, value);
        }

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public Event Event
        {
            get => _event;
            set => SetProperty(ref _event, value);
        }

        public IEnumerable<EventType> EventTypes
        {
            get => _eventTypes;
            set => SetProperty(ref _eventTypes, value);
        }

        public IEnumerable<EventStatus> EventStatuses
        {
            get => _eventStatuses;
            set => SetProperty(ref _eventStatuses, value);
        }

        public double TaskOpacity
        {
            get => _taskOpacity;
            set => SetProperty(ref _taskOpacity, value);
        }

        public double AppointmentOpacity
        {
            get => _appointmentOpacity;
            set => SetProperty(ref _appointmentOpacity, value);
        }

        public bool IsTaskTypeVisible
        {
            get => _isTaskTypeVisible;
            set => SetProperty(ref _isTaskTypeVisible, value);
        }

        public bool IsAppointmentTypeVisible
        {
            get => _isAppointmentTypeVisible;
            set => SetProperty(ref _isAppointmentTypeVisible, value);
        }

        public double ConfirmedOpacity
        {
            get => _confirmedOpacity;
            set => SetProperty(ref _confirmedOpacity, value);
        }

        public double TentativeOpacity
        {
            get => _tentativeOpacity;
            set => SetProperty(ref _tentativeOpacity, value);
        }

        public string TitleErrorMessage
        {
            get => _titleErrorMessage;
            set => SetProperty(ref _titleErrorMessage, value);
        }

        public string DateTimeErrorMessage
        {
            get => _dateTimeErrorMessage;
            set => SetProperty(ref _dateTimeErrorMessage, value);
        }

        public void SetTypeAsTask()
        {
            SetType(EventType.Task);
        }

        public void SetTypeAsAppointment()
        {
            SetType(EventType.Appointment);
        }

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
            KeyboardCommand = new DelegateCommand(ToggleKeyboard);
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

        private void CategoryClicked(EventCategory category)
        {
            Event.Category = category;
        }

        private void CultureChanged()
        {
            if (Event.Start > Event.End || string.IsNullOrEmpty(Event.Title))
            {
                DateTimeErrorMessage = Event.Start > Event.End ? Resources.ErrorEndTimeIsEarlierThanStartTime : null;
                TitleErrorMessage = string.IsNullOrEmpty(Event.Title) ? Resources.ErrorTitleNotSet : null;
            }
            if (Event.Start < DateTime.Today)
            {
                DateTimeErrorMessage = Resources.ErrorStartTimeIsBeforeTodaysDate;
            }
            if (Event.End?.Date > Event.Start?.Date)
            {
                DateTimeErrorMessage = Resources.ErrorEventSpansAccrossMultipleDays;
            }
        }

        private void ToggleKeyboard()
        {
            TouchKeyboardProcess = TouchKeyboardProcess.HasExited || TouchKeyboardProcess == null ? Process.Start(touchKeyboardPath) : null;
        }

        public async void SaveEvent()
        {
            bool error = false;
            if (string.IsNullOrEmpty(Event.Title))
            {
                TitleErrorMessage = Resources.ErrorTitleNotSet;
                error = true;
            }
            if (Event.Start > Event.End)
            {
                DateTimeErrorMessage = Resources.ErrorEndTimeIsEarlierThanStartTime;
                error = true;
            }
            if (Event.Start < DateTime.Today)
            {
                DateTimeErrorMessage = Resources.ErrorStartTimeIsBeforeTodaysDate;
                error = true;
            }
            if (Event.End?.Date > Event.Start?.Date)
            {
                DateTimeErrorMessage = Resources.ErrorEventSpansAccrossMultipleDays;
                error = true;
            }
            if (!error)
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
                catch (ConnectionException e)
                {
                    _eventAggregator.GetEvent<NotificationEvent>().Publish(new ConnectionErrorNotification());
                }
                catch (UnauthorizedAccessException e)
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

        private void Cancel()
        {
            _regionManager.Regions[_region].NavigationService.Journal.GoBack();
        }

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
                    Start = DateTime.Today,
                    End = DateTime.Today
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
            catch (ConnectionException e)
            {
                _eventAggregator.GetEvent<NotificationEvent>().Publish(new ConnectionErrorNotification());
            }
            catch (UnauthorizedAccessException e)
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

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            TitleErrorMessage = null;
            DateTimeErrorMessage = null;
        }

        private void Back()
        {
            _regionManager.Regions[_region].NavigationService.Journal.GoBack();
        }
    }
}
