using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using TaskSharper.Calender.WPF.Events;
using TaskSharper.Calender.WPF.Events.Resources;
using TaskSharper.Calender.WPF.Properties;
using TaskSharper.Domain.BusinessLayer;
using TaskSharper.Domain.Calendar;

namespace TaskSharper.Calender.WPF.ViewModels
{
    public class CalendarEventDetailsViewModel : BindableBase, INavigationAware
    {
        public DelegateCommand BackCommand { get; set; }
        public DelegateCommand SaveEventCommand { get; set; }
        public DelegateCommand CancelCommand { get; set; }
        public DelegateCommand KeyboardCommand { get; set; }
        public DelegateCommand SetTypeAsAppointmentCommand { get; set; }
        public DelegateCommand SetTypeAsTaskCommand { get; set; }
        public DelegateCommand SetStatusAsTentativeCommand { get; set; }
        public DelegateCommand SetStatusAsConfirmedCommand { get; set; }

        private readonly IRegionManager _regionManager;
        private readonly IEventAggregator _eventAggregator;
        private readonly IEventRestClient _dataService;

        private Process _touchKeyboardProcess;
        private string touchKeyboardPath = @"C:\Program Files\Common Files\Microsoft Shared\Ink\TabTip.exe";
        private string _title;
        private Event _selectedEvent;
        private Event _editEvent;
        private bool _isNotInEditMode = true;
        private IEnumerable<EventType> _eventTypes;
        private IEnumerable<EventStatus> _eventStatuses;
        private double _taskOpacity = Settings.Default.NotSelectedOpacity;
        private double _appointmentOpacity = Settings.Default.NotSelectedOpacity;
        private double _confirmedOpacity = Settings.Default.NotSelectedOpacity;
        private double _tentativeOpacity = Settings.Default.NotSelectedOpacity;
        private string _dateTimeErrorMessage;
        private string _titleErrorMessage;

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

        public Event SelectedEvent
        {
            get => _selectedEvent;
            set => SetProperty(ref _selectedEvent, value);
        }

        public Event EditEvent
        {
            get => _editEvent;
            set => SetProperty(ref _editEvent, value);
        }

        public bool IsNotInEditMode
        {
            get => _isNotInEditMode;
            set => SetProperty(ref _isNotInEditMode, value);
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
                    EditEvent.Type = EventType.Task;
                    TaskOpacity = Settings.Default.SelectedOpacity;
                    break;
                case EventType.Appointment:
                    EditEvent.Type = EventType.Appointment;
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
                    EditEvent.Status = EventStatus.Confirmed;
                    ConfirmedOpacity = Settings.Default.SelectedOpacity;
                    break;
                case EventStatus.Tentative:
                    EditEvent.Status = EventStatus.Tentative;
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

        public CalendarEventDetailsViewModel(IRegionManager regionManager, IEventRestClient dataService, IEventAggregator eventAggregator)
        {
            _regionManager = regionManager;
            _dataService = dataService;
            _eventAggregator = eventAggregator;

            BackCommand = new DelegateCommand(Back);
            SaveEventCommand = new DelegateCommand(SaveEvent);
            CancelCommand = new DelegateCommand(Cancel);
            KeyboardCommand = new DelegateCommand(ToggleKeyboard);
            SetTypeAsTaskCommand = new DelegateCommand(() => SetType(EventType.Task));
            SetTypeAsAppointmentCommand = new DelegateCommand(() => SetType(EventType.Appointment));
            SetStatusAsConfirmedCommand = new DelegateCommand(() => SetStatus(EventStatus.Confirmed));
            SetStatusAsTentativeCommand = new DelegateCommand(() => SetStatus(EventStatus.Tentative));

            EventTypes = Enum.GetValues(typeof(EventType)).Cast<EventType>();
            EventStatuses = Enum.GetValues(typeof(EventStatus)).Cast<EventStatus>().Except(new List<EventStatus>{ EventStatus.Cancelled });
            EditEvent = new Event()
            {
                Start = DateTime.Today,
                End = DateTime.Today
            }; // Temporarily assign an empty event so DateTimePicker can bind to a non-null object (this will be properly set in the OnNavigatedTo method)
            _eventAggregator.GetEvent<CultureChangedEvent>().Subscribe(CultureChanged);
        }

        private void CultureChanged()
        {
            if (TitleErrorMessage != null) TitleErrorMessage = Resources.ErrorTitleNotSet;
            if (DateTimeErrorMessage != null) DateTimeErrorMessage = Resources.ErrorEndTimeIsEarlierThanStartTime;
        }

        private void ToggleKeyboard()
        {      
            TouchKeyboardProcess = TouchKeyboardProcess.HasExited || TouchKeyboardProcess == null ? Process.Start(touchKeyboardPath) : null;
        }

        private async void SaveEvent()
        {
            if (EditEvent.Start > EditEvent.End || EditEvent.Title == "")
            {
                DateTimeErrorMessage = EditEvent.Start > EditEvent.End ? Resources.ErrorEndTimeIsEarlierThanStartTime : null;
                TitleErrorMessage = EditEvent.Title == "" ? Resources.ErrorTitleNotSet : null;
            }
            else
            {
                SelectedEvent = await _dataService.UpdateAsync(EditEvent);
                _regionManager.Regions["CalendarRegion"].NavigationService.Journal.GoBack();
            }
        }

        private void Cancel()
        {
            _regionManager.Regions["CalendarRegion"].NavigationService.Journal.GoBack();
        }

        public async void OnNavigatedTo(NavigationContext navigationContext)
        {
            var id = navigationContext.Parameters["id"].ToString();

            SelectedEvent = _dataService.Get(id);
            EditEvent = CopySelectedEvent();
            SetType(EditEvent.Type);
            SetStatus(EditEvent.Status);
        }

        private Event CopySelectedEvent()
        {
            return new Event
            {
                Id = SelectedEvent.Id,
                Title = SelectedEvent.Title,
                Description = SelectedEvent.Description,
                Start = SelectedEvent.Start,
                End = SelectedEvent.End,
                AllDayEvent = SelectedEvent.AllDayEvent,
                Status = SelectedEvent.Status,
                Type = SelectedEvent.Type,
                Created = SelectedEvent.Created,
                Updated = SelectedEvent.Updated
            };
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            EditEvent = CopySelectedEvent();
            TitleErrorMessage = null;
            DateTimeErrorMessage = null;
        }

        private void Back()
        {
            _regionManager.Regions["CalendarRegion"].NavigationService.Journal.GoBack();
        }
    }
}
