using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using TaskSharper.Calender.WPF.Events;
using TaskSharper.Calender.WPF.Events.Resources;
using TaskSharper.DataAccessLayer.Google;
using TaskSharper.Domain.BusinessLayer;
using TaskSharper.Domain.Calendar;

namespace TaskSharper.Calender.WPF.ViewModels
{
    public class CalendarEventDetailsViewModel : BindableBase, INavigationAware
    {
        public DelegateCommand BackCommand { get; set; }
        public DelegateCommand EnableEditModeCommand { get; set; }
        public DelegateCommand DisableEditModeCommand { get; set; }
        public DelegateCommand SaveEventCommand { get; set; }
        public DelegateCommand CancelCommand { get; set; }
        public DelegateCommand KeyboardCommand { get; set; }     

        private readonly IRegionManager _regionManager;
        private IEventAggregator _eventAggregator;
        private readonly IEventManager _calendarService;

        private Process _touchKeyboardProcess;
        private string touchKeyboardPath = @"C:\Program Files\Common Files\Microsoft Shared\Ink\TabTip.exe";
        private string _title;
        private Event _selectedEvent;
        private Event _editEvent;
        private bool _isInEditMode;
        private bool _isNotInEditMode = true;
        private IEnumerable<Event.EventType> _eventTypes;
        private IEnumerable<Event.EventStatus> _eventStatuses;

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

        public bool IsInEditMode
        {
            get => _isInEditMode;
            set
            {
                IsNotInEditMode = !value;
                SetProperty(ref _isInEditMode, value);
            }
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

        public void EnableEditMode()
        {
            IsInEditMode = true;
            TouchKeyboardProcess = Process.Start(touchKeyboardPath);
        }

        public void DisableEditMode()
        {
            IsInEditMode = false;
            TouchKeyboardProcess?.WaitForExit(200);
        }

        public CalendarEventDetailsViewModel(IRegionManager regionManager, IEventManager calendarService, IEventAggregator eventAggregator)
        {
            _regionManager = regionManager;
            _calendarService = calendarService;
            _eventAggregator = eventAggregator;

            BackCommand = new DelegateCommand(Back);
            EnableEditModeCommand = new DelegateCommand(EnableEditMode);
            DisableEditModeCommand = new DelegateCommand(DisableEditMode);
            SaveEventCommand = new DelegateCommand(SaveEvent);
            CancelCommand = new DelegateCommand(Cancel);
            KeyboardCommand = new DelegateCommand(ToggleKeyboard);

            EventTypes = Enum.GetValues(typeof(Event.EventType)).Cast<Event.EventType>();
            EventStatuses = Enum.GetValues(typeof(Event.EventStatus)).Cast<Event.EventStatus>().Except(new List<Event.EventStatus>{ Event.EventStatus.Cancelled });
        }

        private void ToggleKeyboard()
        {      
            TouchKeyboardProcess = TouchKeyboardProcess.HasExited || TouchKeyboardProcess == null ? Process.Start(touchKeyboardPath) : null;
        }

        private void SaveEvent()
        {
            _eventAggregator.GetEvent<SpinnerEvent>().Publish(EventResources.SpinnerEnum.Show);
            _eventAggregator.GetEvent<EventChangedEvent>().Publish(EditEvent);
            SelectedEvent = _calendarService.GetEvent(EditEvent.Id, EditEvent.Start.Value);
            DisableEditMode();
            _eventAggregator.GetEvent<SpinnerEvent>().Publish(EventResources.SpinnerEnum.Hide);
        }

        private void Cancel()
        {
            EditEvent = CopySelectedEvent();
            DisableEditMode();
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            var id = navigationContext.Parameters["id"].ToString();

            SelectedEvent = _calendarService.GetEvent(id);
            EditEvent = CopySelectedEvent();
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
        }

        private void Back()
        {
            DisableEditMode();
            _regionManager.Regions["CalendarRegion"].NavigationService.Journal.GoBack();
        }
    }
}
