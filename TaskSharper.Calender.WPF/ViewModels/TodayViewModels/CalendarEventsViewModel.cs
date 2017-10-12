using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Timers;
using System.Windows;
using System.Windows.Media;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Serilog;
using TaskSharper.Calender.WPF.Events;
using TaskSharper.Calender.WPF.Events.Resources;
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
        private readonly IEventManager _evenrManager;
        private CalendarEventsCurrentTimeLine _timeLine;
        private DateTime _date;

        // Setting Date automatically results in view data being updated
        public DateTime Date
        {
            get => _date;
            set
            {
                _date = value;
                UpdateView();
            }
        }
        
        public ObservableCollection<CalendarEventViewModel> CalendarEvents { get; set; }
        public ObservableCollection<CalendarEventsBackground> Backgrounds { get; set; }

        public CalendarEventsCurrentTimeLine TimeLine
        {
            get => _timeLine;
            set => SetProperty(ref _timeLine, value);
        }

        public CalendarEventsViewModel(DateTime date, IEventAggregator eventAggregator, IRegionManager regionManager, IEventManager eventManager, CalendarTypeEnum dateType, ILogger logger)
        {
            // Initialze object
            _eventAggregator = eventAggregator;
            _regionManager = regionManager;
            _dateType = dateType;
            _evenrManager = eventManager;
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

            // Set date => Automatically update data in view
            Date = date;
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

        private void EventChangedEventHandler(Event obj)
        {
            if (Date.Date == obj.Start.Value.Date)
            {
                _evenrManager.UpdateEvent(obj);
                UpdateView();
            }
        }
        #endregion

        private void InitializeView()
        {
            for (int i = 1; i < Time.HoursInADay; i = i + 2)
            {
                Backgrounds.Add(new CalendarEventsBackground
                {
                    Height = 50,
                    LocX = 0,
                    LocY = i * 50
                });
            }

            var now = DateTime.Now;

            TimeLine = new CalendarEventsCurrentTimeLine
            {
                Height = 1,
                LocX = 0,
                LocY = 1200 / Time.HoursInADay * (now.Hour + now.Minute / Time.MinutesInAnHour),
                StrokeDashArray = DateTime.Today == Date.Date ? new DoubleCollection { 4, 0 } : new DoubleCollection { 2, 4 }
            };

            var timer = new Timer();
            timer.Elapsed += UpdateTimeLine;
            // Set the Interval to 1 minute.
            timer.Interval = (double) 60 * 1000;
            timer.Enabled = true;
        }

        private void UpdateTimeLine(object source, ElapsedEventArgs e)
        {
            TimeLine.LocY = 1200 / Time.HoursInADay * (DateTime.Now.Hour + DateTime.Now.Minute / Time.MinutesInAnHour);
            
            TimeLine.StrokeDashArray = DateTime.Today == Date.Date ?
                Application.Current.Dispatcher.Invoke(() => TimeLine.StrokeDashArray = new DoubleCollection { 4, 0 }) :
                Application.Current.Dispatcher.Invoke(() => TimeLine.StrokeDashArray = new DoubleCollection { 2, 4 });
        }

        public void UpdateView()
        {
            CalendarEvents?.Clear();
            GetEvents();
            UpdateTimeLine(null, null);
        }

        private async void GetEvents()
        {
            _eventAggregator.GetEvent<SpinnerEvent>().Publish(EventResources.SpinnerEnum.Show);

            try
            {
                var calendarEvents = await _evenrManager.GetEventsAsync(Date.Date);

                foreach (var calendarEvent in calendarEvents)
                {
                    if (!calendarEvent.Start.HasValue || !calendarEvent.End.HasValue) continue;
                    var viewModel = new CalendarEventViewModel(_regionManager, _eventAggregator, _logger)
                    {
                        LocY = calendarEvent.Start.Value.Hour / Time.HoursInADay * 1200 +
                               calendarEvent.Start.Value.Minute / Time.MinutesInAnHour / Time.HoursInADay * 1200, // TODO: Declare 1200 as a constant somewhere
                        Height = (calendarEvent.End.Value - calendarEvent.Start.Value).TotalMinutes / Time.MinutesInAnHour / Time.HoursInADay * 1200,
                        Event = calendarEvent
                    };
                    
                    CalendarEvents.Add(viewModel);
                }

                _eventAggregator.GetEvent<SpinnerEvent>().Publish(EventResources.SpinnerEnum.Hide);
            }
            catch (Exception e)
            {
                _eventAggregator.GetEvent<SpinnerEvent>().Publish(EventResources.SpinnerEnum.Hide);
                _logger.Error(e, "Error orcurred while getting event data");

            }
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
