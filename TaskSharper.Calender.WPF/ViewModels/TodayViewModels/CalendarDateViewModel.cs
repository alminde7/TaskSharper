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
    /// <inheritdoc />
    /// <summary>
    /// ViewModel for the CalendarDate user control.
    /// </summary>
    public class CalendarDateViewModel : BindableBase
    {
        private readonly CalendarTypeEnum _dateType;
        private readonly ILogger _logger;

        private string _dayOfWeek;
        private DateTime _currentDate;

        /// <summary>
        /// Day of week. Eg. "Monday"
        /// </summary>
        public string DayOfWeek
        {
            get => _dayOfWeek;
            set => SetProperty(ref _dayOfWeek, value);
        }

        /// <summary>
        /// DateTime object used for data binding in the view.
        /// </summary>
        public DateTime CurrentDate
        {
            get => _currentDate;
            set => SetProperty(ref _currentDate, value);
        }

        /// <summary>
        /// Information about the DateTime format.
        /// </summary>
        public DateTimeFormatInfo DateCultureInfo { get; set; }

        /// <summary>
        /// Information about the culture.
        /// </summary>
        public CultureInfo CurrentCulture { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="date">Date that is to be set</param>
        /// <param name="eventAggregator">Event aggregator for subscribing to and publishing events</param>
        /// <param name="dateType">Type of calendar. Possible values are: Day, Week, Month</param>
        /// <param name="logger">Logger for logging</param>
        public CalendarDateViewModel(DateTime date, IEventAggregator eventAggregator, CalendarTypeEnum dateType, ILogger logger)
        {
            _dateType = dateType;
            _logger = logger;

            // Initialization
            SetDate(date);

            // Event subscription
            eventAggregator.GetEvent<DayChangedEvent>().Subscribe(DayChangedEventHandler);
            eventAggregator.GetEvent<WeekChangedEvent>().Subscribe(WeekChangedEventHandler);
            eventAggregator.GetEvent<MonthChangedEvent>().Subscribe(MonthChangedEventHandler);
            eventAggregator.GetEvent<CultureChangedEvent>().Subscribe(UpdateCultureHandler);
        }

        /// <summary>
        /// Handler for when culture is changed.
        /// </summary>
        private void UpdateCultureHandler()
        {
            SetDate(CurrentDate);
        }

        /// <summary>
        /// Setting the day and date in the correct language/format.
        /// </summary>
        /// <param name="date">Date that is to be set</param>
        public void SetDate(DateTime date)
        {
            CurrentCulture = CultureInfo.CurrentCulture;
            DateCultureInfo = DateTimeFormatInfo.CurrentInfo;
            DayOfWeek = CurrentCulture.TextInfo.ToTitleCase(DateCultureInfo.GetDayName(date.DayOfWeek));
            CurrentDate = date;
        }

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
                    SetDate(CurrentDate.AddMonths(1));
                    break;
                case DateChangedEnum.Decrease:
                    SetDate(CurrentDate.AddMonths(-1));
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
                    SetDate(CurrentDate.AddDays(7));
                    break;
                case DateChangedEnum.Decrease:
                    SetDate(CurrentDate.AddDays(-7));
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
                    SetDate(CurrentDate.AddDays(1));
                    break;
                case DateChangedEnum.Decrease:
                    SetDate(CurrentDate.AddDays(-1));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }
    }
}
