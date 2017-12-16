using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Media;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Serilog;
using TaskSharper.Calender.WPF.Helpers.EventLocation;
using TaskSharper.Domain.Calendar;
using TaskSharper.Domain.Models;
using TaskSharper.Domain.RestClient;
using TaskSharper.Shared.Constants;
using TaskSharper.WPF.Common.Events;
using TaskSharper.WPF.Common.Events.DateChangedEvents;
using TaskSharper.WPF.Common.Events.Resources;
using Settings = TaskSharper.WPF.Common.Properties.Settings;

namespace TaskSharper.Calender.WPF.ViewModels
{
    /// <inheritdoc />
    /// <summary>
    /// ViewModel for the calendar events container.
    /// </summary>
    public class CalendarEventsViewModel : BindableBase
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IRegionManager _regionManager;
        private readonly CalendarTypeEnum _dateType;
        private readonly ILogger _logger;
        private readonly IEventRestClient _dataService;
        private CalendarEventsCurrentTimeLine _timeLine;
        private List<string> _columnAlreadyUpdatedList;
        
        /// <summary>
        /// Date that this container has events for.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Collection of events that can be bound to in the view.
        /// </summary>
        public ObservableCollection<CalendarEventViewModel> CalendarEvents { get; set; }

        /// <summary>
        /// Collection of event backgrounds that can be bound to in the view.
        /// </summary>
        public ObservableCollection<CalendarEventsBackground> Backgrounds { get; set; }

        /// <summary>
        /// In the view, this is the line that indicates the current time of day.
        /// </summary>
        public CalendarEventsCurrentTimeLine TimeLine
        {
            get => _timeLine;
            set => SetProperty(ref _timeLine, value);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="date">Date for the event container</param>
        /// <param name="eventAggregator">Event aggregator for subscribing to and publishing events</param>
        /// <param name="regionManager">Region manager for navigation</param>
        /// <param name="dataService">Data service for data management</param>
        /// <param name="dateType">Type of calendar. Possible values are: Day, Week, Month</param>
        /// <param name="logger">Logger for logging</param>
        public CalendarEventsViewModel(DateTime date, IEventAggregator eventAggregator, IRegionManager regionManager, IEventRestClient dataService, CalendarTypeEnum dateType, ILogger logger)
        {
            // Initialze object
            Date = date;
            _columnAlreadyUpdatedList = new List<string>();

            _eventAggregator = eventAggregator;
            _regionManager = regionManager;
            _dateType = dateType;
            _dataService = dataService;
            _logger = logger.ForContext<CalendarEventsViewModel>();

            // Initialize containers
            CalendarEvents = new ObservableCollection<CalendarEventViewModel>();
            Backgrounds = new ObservableCollection<CalendarEventsBackground>();

            // Subscribe to events
            _eventAggregator.GetEvent<DayChangedEvent>().Subscribe(DayChangedEventHandler);
            _eventAggregator.GetEvent<WeekChangedEvent>().Subscribe(WeekChangedEventHandler);
            _eventAggregator.GetEvent<MonthChangedEvent>().Subscribe(MonthChangedEventHandler);
            eventAggregator.GetEvent<EventChangedEvent>().Subscribe(EventChangedEventHandler);

            // Initialize view
            InitializeView();
        }

        #region EventHandlers

        /// <summary>
        /// Handler for when month is changed.
        /// </summary>
        /// <param name="state">Increase or Decrease</param>
        private void MonthChangedEventHandler(DateChangedEnum state)
        {
            if (_dateType != CalendarTypeEnum.Month) return;
            switch (state)
            {
                case DateChangedEnum.Increase:
                    Date = Date.AddMonths(1);
                    UpdateView();
                    break;
                case DateChangedEnum.Decrease:
                    Date = Date.AddMonths(-1);
                    UpdateView();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        /// <summary>
        /// Handler for when week is changed.
        /// </summary>
        /// <param name="state">Increase or Decrease</param>
        private void WeekChangedEventHandler(DateChangedEnum state)
        {
            if (_dateType != CalendarTypeEnum.Week) return;
            switch (state)
            {
                case DateChangedEnum.Increase:
                    Date = Date.AddDays(7);
                    UpdateView();
                    break;
                case DateChangedEnum.Decrease:
                    Date = Date.AddDays(-7);
                    UpdateView();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        /// <summary>
        /// Handler for when day is changed.
        /// </summary>
        /// <param name="state">Increase or Decrease</param>
        private void DayChangedEventHandler(DateChangedEnum state)
        {
            if (_dateType != CalendarTypeEnum.Day) return;
            switch (state)
            {
                case DateChangedEnum.Increase:
                    Date = Date.AddDays(1);
                    UpdateView();
                    break;
                case DateChangedEnum.Decrease:
                    Date = Date.AddDays(-1);
                    UpdateView();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        /// <summary>
        /// Handler for when an event has changed.
        /// </summary>
        /// <param name="obj">An event</param>
        private async void EventChangedEventHandler(Event obj)
        {
            if (Date.Date == obj.Start.Value.Date)
            {
                await _dataService.UpdateAsync(obj);
                UpdateView();
            }
        }
        #endregion

        /// <summary>
        /// Updates the view with a list of events.
        /// </summary>
        /// <param name="events">List of events</param>
        public void UpdateView(IList<Event> events = null)
        {
            if (events == null)
            {
                CalendarEvents?.Clear();
                UpdateTimeLine(null, null);
            }
            else
            {
                CalendarEvents?.Clear();
                UpdateEvents(events);
                UpdateTimeLine(null, null);
            }
        }

        /// <summary>
        /// Initiates the CalendarEventViewModels with correct layout values.
        /// </summary>
        /// <param name="events">List of events</param>
        private void UpdateEvents(IList<Event> events)
        {
            _columnAlreadyUpdatedList?.Clear();
            var eventLocations = EventLocation.FindLayout(events.ToList());
            foreach (var column in eventLocations)
            {
                foreach (var calendarEvent in column)
                {
                    if (!calendarEvent.Event.Start.HasValue || !calendarEvent.Event.End.HasValue) continue;
                    var viewModel = new CalendarEventViewModel(_regionManager, _eventAggregator, _logger)
                    {
                        LocY = calendarEvent.PosY,
                        TotalColumns = calendarEvent.TotalColumns,
                        Column = calendarEvent.Column,
                        Height = calendarEvent.Height,
                        Event = calendarEvent.Event,
                        ColumnSpan = calendarEvent.ColumnSpan
                        // Width and LocX are set in the OnLoaded and OnSizeChanged method
                    };

                    CalendarEvents.Add(viewModel);
                }
            }
        }

        /// <summary>
        /// Initializes the view.
        /// Sets the background, the timeline and starts a timer that updates the timeline every minute.
        /// </summary>
        private void InitializeView()
        {
            for (int i = 1; i < TimeConstants.HoursInADay; i = i + 2)
            {
                Backgrounds.Add(new CalendarEventsBackground
                {
                    Height = Settings.Default.CalendarStructure_Height_1200 / TimeConstants.HoursInADay,
                    LocX = 0,
                    LocY = i * Settings.Default.CalendarStructure_Height_1200 / TimeConstants.HoursInADay
                });
            }

            var now = DateTime.Now;

            TimeLine = new CalendarEventsCurrentTimeLine
            {
                Height = 1,
                LocX = 0,
                LocY = 1200 / TimeConstants.HoursInADay * (now.Hour + now.Minute / TimeConstants.MinutesInAnHour),
                StrokeDashArray = DateTime.Today == Date.Date ? new DoubleCollection { 4, 0 } : new DoubleCollection { 2, 4 }
            };

            var timer = new Timer();
            timer.Elapsed += UpdateTimeLine;
            // Set the Interval to 1 minute.
            timer.Interval = (double) 60 * 1000;
            timer.Enabled = true;
        }

        /// <summary>
        /// Handler for when TimeLine should be updated (eg. once per minute).
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void UpdateTimeLine(object source, ElapsedEventArgs e)
        {
            TimeLine.LocY = Settings.Default.CalendarStructure_Height_1200 / TimeConstants.HoursInADay * (DateTime.Now.Hour + DateTime.Now.Minute / TimeConstants.MinutesInAnHour);
            
            TimeLine.StrokeDashArray = DateTime.Today == Date.Date ?
                Application.Current.Dispatcher.Invoke(() => TimeLine.StrokeDashArray = new DoubleCollection { 4, 0 }) :
                Application.Current.Dispatcher.Invoke(() => TimeLine.StrokeDashArray = new DoubleCollection { 2, 4 });
        }

    }

    /// <inheritdoc />
    /// <summary>
    /// Contains information about a background's location on the canvas in the view.
    /// Is used for data binding.
    /// </summary>
    public class CalendarEventsBackground : BindableBase
    {
        private double _height;
        private double _locX;
        private double _locY;

        /// <summary>
        /// Height in pixels for the background.
        /// </summary>
        public double Height
        {
            get => _height;
            set => SetProperty(ref _height, value);
        }

        /// <summary>
        /// X-position for the background on the canvas.
        /// </summary>
        public double LocX
        {
            get => _locX;
            set => SetProperty(ref _locX, value);
        }

        /// <summary>
        /// Y-position for the background on the canvas.
        /// </summary>
        public double LocY
        {
            get => _locY;
            set => SetProperty(ref _locY, value);
        }
    }

    /// <inheritdoc />
    /// <summary>
    /// Contains information about the timeline's location in the view.
    /// Is used for data binding.
    /// </summary>
    public class CalendarEventsCurrentTimeLine : BindableBase
    {
        private double _height;
        private double _locX;
        private double _locY;
        private DoubleCollection _strokeDashArray;

        /// <summary>
        /// Height in pixels for the timeline.
        /// </summary>
        public double Height
        {
            get => _height;
            set => SetProperty(ref _height, value);
        }

        /// <summary>
        /// X-position for the timeline on the canvas.
        /// </summary>
        public double LocX
        {
            get => _locX;
            set => SetProperty(ref _locX, value);
        }
        
        /// <summary>
        /// Y-position for the timeline on the canvas.
        /// </summary>
        public double LocY
        {
            get => _locY;
            set => SetProperty(ref _locY, value);
        }

        /// <summary>
        /// How the timeline is presented. Eg. if it is dashed or a full line.
        /// </summary>
        public DoubleCollection StrokeDashArray
        {
            get => _strokeDashArray;
            set => SetProperty(ref _strokeDashArray, value);
        }
    }
    
}
