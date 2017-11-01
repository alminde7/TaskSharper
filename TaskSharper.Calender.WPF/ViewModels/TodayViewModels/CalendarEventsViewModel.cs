using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Media;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Serilog;
using TaskSharper.Calender.WPF.Events;
using TaskSharper.Calender.WPF.Events.Resources;
using TaskSharper.Calender.WPF.Properties;
using TaskSharper.Domain.BusinessLayer;
using TaskSharper.Domain.Calendar;
using TaskSharper.Shared.Constants;

namespace TaskSharper.Calender.WPF.ViewModels
{
    public class CalendarEventsViewModel : BindableBase
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IRegionManager _regionManager;
        private readonly CalendarTypeEnum _dateType;
        private readonly ILogger _logger;
        private readonly IEventRestClient _dataService;
        private CalendarEventsCurrentTimeLine _timeLine;
        
        public DateTime Date { get; set; }

        public ObservableCollection<CalendarEventViewModel> CalendarEvents { get; set; }
        public ObservableCollection<CalendarEventsBackground> Backgrounds { get; set; }

        public CalendarEventsCurrentTimeLine TimeLine
        {
            get => _timeLine;
            set => SetProperty(ref _timeLine, value);
        }

        public CalendarEventsViewModel(DateTime date, IEventAggregator eventAggregator, IRegionManager regionManager, IEventRestClient dataService, CalendarTypeEnum dateType, ILogger logger)
        {
            // Initialze object
            Date = date;

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

        private async void EventChangedEventHandler(Event obj)
        {
            if (Date.Date == obj.Start.Value.Date)
            {
                await _dataService.UpdateAsync(obj);
                UpdateView();
            }
        }
        #endregion

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

        private void UpdateEvents(IList<Event> events)
        {
            foreach (var calendarEvent in events)
            {
                if (!calendarEvent.Start.HasValue || !calendarEvent.End.HasValue) continue;
                var simultaneousEvents = SimultaneousEvents(calendarEvent, events);
                var viewModel = new CalendarEventViewModel(_regionManager, _eventAggregator, _logger)
                {
                    LocY = calendarEvent.Start.Value.Hour / TimeConstants.HoursInADay * Settings.Default.CalendarStructure_Height_1200 +
                           calendarEvent.Start.Value.Minute / TimeConstants.MinutesInAnHour / TimeConstants.HoursInADay * Settings.Default.CalendarStructure_Height_1200,
                    SimultaneousEvents = simultaneousEvents.simultaneousEvents,
                    Column = simultaneousEvents.column,
                    Height = (calendarEvent.End.Value - calendarEvent.Start.Value).TotalMinutes / TimeConstants.MinutesInAnHour / TimeConstants.HoursInADay * Settings.Default.CalendarStructure_Height_1200,
                    Event = calendarEvent
                    // Width and LocX are set in the OnLoaded and OnSizeChanged method
                };

                CalendarEvents.Add(viewModel);
            }
        }

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

        private (double simultaneousEvents, double column) SimultaneousEvents(Event eventObj, IList<Event> events)
        {
            var columnIndex = 0;

            foreach (var @event in events)
            {
                if (@event.Start < eventObj.End && eventObj.Start < @event.End) // If this is true, there is some kind of overlapping
                {
                    if (@event.Start < eventObj.Start)
                    {
                        columnIndex++;
                    }
                    if (@event.Start < eventObj.Start && eventObj.End < @event.End)
                    {
                        columnIndex++;
                    }
                    if (@event.Start == eventObj.Start && @event.End < eventObj.End)
                    {
                        columnIndex++;
                    }
                    if (!eventObj.Id.Equals(@event.Id) && @event.Start == eventObj.Start && @event.End == eventObj.End)
                    {
                        //columnIndex++;
                        // TODO: Figure out what to do in this case...
                    }
                }
            }

            return (events.Count(i => i.Start < eventObj.End && eventObj.Start < i.End), columnIndex);
        }

        private void UpdateTimeLine(object source, ElapsedEventArgs e)
        {
            TimeLine.LocY = Settings.Default.CalendarStructure_Height_1200 / TimeConstants.HoursInADay * (DateTime.Now.Hour + DateTime.Now.Minute / TimeConstants.MinutesInAnHour);
            
            TimeLine.StrokeDashArray = DateTime.Today == Date.Date ?
                Application.Current.Dispatcher.Invoke(() => TimeLine.StrokeDashArray = new DoubleCollection { 4, 0 }) :
                Application.Current.Dispatcher.Invoke(() => TimeLine.StrokeDashArray = new DoubleCollection { 2, 4 });
        }

    }

    public class CalendarEventsBackground : BindableBase
    {
        private double _height;
        private double _locX;
        private double _locY;

        public double Height
        {
            get => _height;
            set => SetProperty(ref _height, value);
        }

        public double LocX
        {
            get => _locX;
            set => SetProperty(ref _locX, value);
        }

        public double LocY
        {
            get => _locY;
            set => SetProperty(ref _locY, value);
        }
    }

    public class CalendarEventsCurrentTimeLine : BindableBase
    {
        private double _height;
        private double _locX;
        private double _locY;
        private DoubleCollection _strokeDashArray;

        public double Height
        {
            get => _height;
            set => SetProperty(ref _height, value);
        }

        public double LocX
        {
            get => _locX;
            set => SetProperty(ref _locX, value);
        }

        public double LocY
        {
            get => _locY;
            set => SetProperty(ref _locY, value);
        }

        public DoubleCollection StrokeDashArray
        {
            get => _strokeDashArray;
            set => SetProperty(ref _strokeDashArray, value);
        }
    }
}
