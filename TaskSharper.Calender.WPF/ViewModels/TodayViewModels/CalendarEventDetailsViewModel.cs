using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using TaskSharper.Calender.WPF.Events;
using TaskSharper.Calender.WPF.Events.Resources;
using TaskSharper.Calender.WPF.Properties;
using TaskSharper.DataAccessLayer.Google;
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
        private IEventAggregator _eventAggregator;
        private readonly IEventManager _calendarService;

        private Process _touchKeyboardProcess;
        private string touchKeyboardPath = @"C:\Program Files\Common Files\Microsoft Shared\Ink\TabTip.exe";
        private string _title;
        private Event _selectedEvent;
        private Event _editEvent;
        private bool _isNotInEditMode = true;
        private IEnumerable<Event.EventType> _eventTypes;
        private IEnumerable<Event.EventStatus> _eventStatuses;
        private double _taskOpacity = 0.5;
        private double _appointmentOpacity = 0.5;
        private double _confirmedOpacity = 0.5;
        private double _tentativeOpacity = 0.5;
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

        public IEnumerable<Event.EventType> EventTypes
        {
            get => _eventTypes;
            set => SetProperty(ref _eventTypes, value);
        }

        public IEnumerable<Event.EventStatus> EventStatuses
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
            SetType(Event.EventType.Task);
        }

        public void SetTypeAsAppointment()
        {
            SetType(Event.EventType.Appointment);
        }

        public void SetType(Event.EventType type)
        {
            TaskOpacity = 0.5;
            AppointmentOpacity = 0.5;
            switch (type)
            {
                case Event.EventType.Task:
                    EditEvent.Type = Event.EventType.Task;
                    TaskOpacity = 1;
                    break;
                case Event.EventType.Appointment:
                    EditEvent.Type = Event.EventType.Appointment;
                    AppointmentOpacity = 1;
                    break;
                case Event.EventType.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public void SetStatus(Event.EventStatus status)
        {
            ConfirmedOpacity = 0.5;
            TentativeOpacity = 0.5;
            switch (status)
            {
                case Event.EventStatus.Confirmed:
                    EditEvent.Status = Event.EventStatus.Confirmed;
                    ConfirmedOpacity = 1;
                    break;
                case Event.EventStatus.Tentative:
                    EditEvent.Status = Event.EventStatus.Tentative;
                    TentativeOpacity = 1;
                    break;
                case Event.EventStatus.Cancelled:
                    break;
                case Event.EventStatus.Completed:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(status), status, null);
            }
        }

        public CalendarEventDetailsViewModel(IRegionManager regionManager, IEventManager calendarService, IEventAggregator eventAggregator)
        {
            _regionManager = regionManager;
            _calendarService = calendarService;
            _eventAggregator = eventAggregator;

            BackCommand = new DelegateCommand(Back);
            SaveEventCommand = new DelegateCommand(SaveEvent);
            CancelCommand = new DelegateCommand(Cancel);
            KeyboardCommand = new DelegateCommand(ToggleKeyboard);
            SetTypeAsTaskCommand = new DelegateCommand(() => SetType(Event.EventType.Task));
            SetTypeAsAppointmentCommand = new DelegateCommand(() => SetType(Event.EventType.Appointment));
            SetStatusAsConfirmedCommand = new DelegateCommand(() => SetStatus(Event.EventStatus.Confirmed));
            SetStatusAsTentativeCommand = new DelegateCommand(() => SetStatus(Event.EventStatus.Tentative));

            EventTypes = Enum.GetValues(typeof(Event.EventType)).Cast<Event.EventType>();
            EventStatuses = Enum.GetValues(typeof(Event.EventStatus)).Cast<Event.EventStatus>().Except(new List<Event.EventStatus>{ Event.EventStatus.Cancelled });

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

        private void SaveEvent()
        {
            _eventAggregator.GetEvent<SpinnerEvent>().Publish(EventResources.SpinnerEnum.Show);

            if (EditEvent.Start > EditEvent.End || EditEvent.Title == "")
            {
                DateTimeErrorMessage = EditEvent.Start > EditEvent.End ? Resources.ErrorEndTimeIsEarlierThanStartTime : null;
                TitleErrorMessage = EditEvent.Title == "" ? Resources.ErrorTitleNotSet : null;
            }
            else
            {
                SelectedEvent = _calendarService.UpdateEvent(EditEvent);
                _regionManager.Regions["CalendarRegion"].NavigationService.Journal.GoBack();
            }
            
            _eventAggregator.GetEvent<SpinnerEvent>().Publish(EventResources.SpinnerEnum.Hide);
        }

        private void Cancel()
        {
            _regionManager.Regions["CalendarRegion"].NavigationService.Journal.GoBack();
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            var id = navigationContext.Parameters["id"].ToString();

            SelectedEvent = _calendarService.GetEvent(id);
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
