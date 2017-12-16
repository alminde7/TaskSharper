using System;
using System.Globalization;
using Prism.Events;
using Prism.Mvvm;
using Serilog;
using TaskSharper.WPF.Common.Events;
using TaskSharper.WPF.Common.Events.DateChangedEvents;
using TaskSharper.WPF.Common.Events.Resources;


namespace TaskSharper.Calender.WPF.ViewModels
{
    /// <summary>
    /// This is the ViewModel for the CalendarYearHeaderView.
    /// 
    /// Its purpose is to show the year month and week number in the side of the calendar. 
    /// This will give a clear overview of where the user is in time when using the calendar. 
    /// </summary>
    public class CalendarYearHeaderViewModel : BindableBase
    {
        private readonly CalendarTypeEnum _dateType;
        private readonly ILogger _logger;

        private int _year;
        private string _month;
        private int _weekNumber;

        public int Year
        {
            get => _year;
            set => SetProperty(ref _year, value);
        }

        public string Month
        {
            get => _month;
            set => SetProperty(ref _month, value);
        }

        public int WeekNumber
        {
            get => _weekNumber;
            set => SetProperty(ref _weekNumber, value);
        }

        public DateTime Date { get; set; }

        public DateTimeFormatInfo DateCultureInfo { get; set; }
        public CultureInfo CurrentCulture { get; set; }

        /// <summary>
        /// Constructor that subscribes to all the different date event changes. 
        /// These events are throw from the week month and calendar Views. 
        /// </summary>
        /// <param name="eventAggregator">Dependency injection of the eventAggregator</param>
        /// <param name="dateType">Class that indicates if the datatype is of day, week or month</param>
        /// <param name="logger">Dependency injection of the logger</param>
        public CalendarYearHeaderViewModel(IEventAggregator eventAggregator, CalendarTypeEnum dateType, ILogger logger)
        {
            _dateType = dateType;
            _logger = logger;

            // Initialization
            SetDate(DateTime.Now);

            // Event subscriptions
            eventAggregator.GetEvent<DayChangedEvent>().Subscribe(DayChangedEventHandler);
            eventAggregator.GetEvent<WeekChangedEvent>().Subscribe(WeekChangedEventHandler);
            eventAggregator.GetEvent<MonthChangedEvent>().Subscribe(MonthChangedEventHandler);
            eventAggregator.GetEvent<CultureChangedEvent>().Subscribe(UpdateCultureHandler);
        }

        /// <summary>
        /// Updates culture by recalling the SetDate method
        /// </summary>
        private void UpdateCultureHandler()
        {
            SetDate(Date);
        }

        /// <summary>
        /// SetDate sets the properties depending on the culture. 
        /// </summary>
        /// <param name="date">Datetime object that is Datetime.Now from the start, but changes when other events of change
        /// is received.</param>
        public void SetDate(DateTime date)
        {
            Date = date;

            CurrentCulture = CultureInfo.CurrentCulture;
            DateCultureInfo = DateTimeFormatInfo.CurrentInfo;

            Year = date.Year;
            Month = CurrentCulture.TextInfo.ToTitleCase(DateCultureInfo.GetMonthName(date.Month));
            WeekNumber = DateCultureInfo.Calendar.GetWeekOfYear(date, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday);
        }

        /// <summary>
        /// Handles the event of the month increasing or decreasing 
        /// </summary>
        /// <param name="state">State indicating either increase or decrease</param>
        private void MonthChangedEventHandler(DateChangedEnum state)
        {
            if (_dateType != CalendarTypeEnum.Month) return;
            switch (state)
            {
                case DateChangedEnum.Increase:
                    SetDate(Date.AddMonths(1));
                    break;
                case DateChangedEnum.Decrease:
                   SetDate(Date.AddMonths(-1));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }


        /// <summary>
        /// Handles the event of the week increasing or decreasing 
        /// </summary>
        /// <param name="state">State indicating either increase or decrease</param>
        private void WeekChangedEventHandler(DateChangedEnum state)
        {
            if (_dateType != CalendarTypeEnum.Week) return;
            switch (state)
            {
                case DateChangedEnum.Increase:
                    SetDate(Date.AddDays(7));
                    break;
                case DateChangedEnum.Decrease:
                    SetDate(Date.AddDays(-7));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        /// <summary>
        /// Handles the event of the day increasing or decreasing 
        /// </summary>
        /// <param name="state">State indicating either increase or decrease</param>
        private void DayChangedEventHandler(DateChangedEnum state)
        {
            if (_dateType != CalendarTypeEnum.Day) return;
            switch (state)
            {
                case DateChangedEnum.Increase:
                    SetDate(Date.AddDays(1));
                    break;
                case DateChangedEnum.Decrease:
                    SetDate(Date.AddDays(-1));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }
    }
}
