using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Prism.Commands;
using Prism.Regions;
using Serilog;
using TaskSharper.Calender.WPF.Config;
using TaskSharper.Domain.Calendar;
using TaskSharper.Shared.Extensions;
using TaskSharper.WPF.Common.Events.DateChangedEvents;
using TaskSharper.WPF.Common.Events.Resources;

namespace TaskSharper.Calender.WPF.ViewModels.MonthViewModels
{
    /// <summary>
    /// This ViewModel is the connected with the view that is in every day of the month,
    /// When looking in the calendar. The View has multiple views DayEvent views inside it. 
    /// Clicking on the view will navigate you to the CalendarDayView. 
    /// </summary>
    public class CalendarDateDayViewModel : BindableBase
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly CalendarTypeEnum _dateType;
        private readonly IRegionManager _regionManager;
        private readonly ILogger _logger;

        private DateTime _date;
        private bool _isCurrentDay;
        private bool _isWithinSelectedMonth;

        public DelegateCommand GoToDayViewCommand { get; set; }

        public ObservableCollection<CalendarDayEventViewModel> CalendarEvents { get; set; }    
        public IEventRestClient EventManager { get; set; }

        public DateTime Date
        {
            get => _date;
            set =>  SetProperty(ref _date, value);
        }

        public bool IsCurrentDay
        {
            get => _isCurrentDay;
            set => SetProperty(ref _isCurrentDay, value);
        }

        /// <summary>
        /// Property that makes days that is not within the month greyed out in the view.
        /// </summary>
        public bool IsWithinSelectedMonth
        {
            get => _isWithinSelectedMonth;
            set => SetProperty(ref _isWithinSelectedMonth, value);
        }

        /// <summary>
        /// Constructor
        /// 
        /// Initialize the CalendarDayEventViewModel list. 
        /// 
        /// Subscribes to the MonthChangedEvent. 
        /// </summary>
        /// <param name="date"></param>
        /// <param name="eventAggregator">Dependency injection of the eventAggregator</param>
        /// <param name="eventManager">Dependency injection of the eventManager</param>
        /// <param name="dateType">Class that indicates if the datatype is of day, week or month</param>
        /// <param name="logger">Dependency injection of the logger</param>
        /// <param name="regionManager">Dependency injection of the regionManager</param>
        public CalendarDateDayViewModel(DateTime date, IEventAggregator eventAggregator, IEventRestClient eventManager, CalendarTypeEnum dateType, ILogger logger, IRegionManager regionManager)
        {
            IsCurrentDay = false;
            IsWithinSelectedMonth = true;

            // Initialize objects
            _eventAggregator = eventAggregator;
            _dateType = dateType;
            _regionManager = regionManager;
            EventManager = eventManager;
            _logger = logger.ForContext<CalendarDateDayViewModel>();
            
            // Initialize view containers
            CalendarEvents = new ObservableCollection<CalendarDayEventViewModel>();
            
            // Subscribe to events
            _eventAggregator.GetEvent<MonthChangedEvent>().Subscribe(MonthChangedEventHandler);

            // Initialize event commands
            GoToDayViewCommand = new DelegateCommand(GoToDayView);

            // Set date => Will automatically call UpdateView()
            UpdateDate(date);
        }

        /// <summary>
        /// When the event of the month have changed and the event have been received, this method will be called.
        /// Depending on the state the month will either increase or decrease by 28 days. 
        /// Could have used by month on the datetime object, but was found to be not consitant. 
        /// </summary>
        /// <param name="state"></param>
        private void MonthChangedEventHandler(DateChangedEnum state)
        {
            if (_dateType != CalendarTypeEnum.Month) return;
            switch (state)
            {
                case DateChangedEnum.Increase:
                    Date = Date.AddDays(28);
                    UpdateView();
                    break;
                case DateChangedEnum.Decrease:
                    Date = Date.AddDays(-28);
                    UpdateView();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        /// <summary>
        /// Is called from the constructor to update the date. 
        /// </summary>
        /// <param name="date">The date of the specific day</param>
        public void UpdateDate(DateTime date)
        {
            Date = date;
            IsCurrentDay = Date.Date == DateTime.Now.Date;
        }

        /// <summary>
        /// This will be called in the handler of month changed. 
        /// If the list is not empty GetEvents will be called. 
        /// </summary>
        /// <param name="events">new list of events from the cache.</param>
        public void UpdateView(IList<Event> events = null)
        {
            if (events == null)
            {
                CalendarEvents.Clear();
            }
            else
            {
                CalendarEvents.Clear();
                GetEvents(events);
            }

        }

        /// <summary>
        /// Adds event to a new CalendarDayEventViewModel list. 
        /// </summary>
        /// <param name="calendarEvents">new list of events from the cache.</param>
        private void GetEvents(IList<Event> calendarEvents)
        {
            foreach (var calendarEvent in calendarEvents)
            {
                if (!calendarEvent.Start.HasValue || !calendarEvent.End.HasValue) continue;
                CalendarEvents.Add(new CalendarDayEventViewModel() { Event = calendarEvent });
            }
        }

        /// <summary>
        /// If the day is clicked, it goes to the day view. 
        /// </summary>
        public void GoToDayView()
        {
            _logger.Information("Navigating from MonthView to DayView");
            _regionManager.RequestNavigate(ViewConstants.REGION_Calendar, $"{ViewConstants.VIEW_CalendarDay}?date={Date.StartOfDay()}");
        }
    }
}
