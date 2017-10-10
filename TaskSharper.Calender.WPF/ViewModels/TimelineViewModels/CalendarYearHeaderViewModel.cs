using System;
using System.Globalization;
using Prism.Events;
using Prism.Mvvm;
using Serilog;
using TaskSharper.Calender.WPF.Events;
using TaskSharper.Calender.WPF.Events.Resources;

namespace TaskSharper.Calender.WPF.ViewModels
{
    public class CalendarYearHeaderViewModel : BindableBase
    {
        private readonly CalendarTypeEnum _dateType;
        private readonly ILogger _logger;
        private DateTime _date;
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

        public DateTime Date    
        {
            get => _date;
            set
            {
                Year = value.Year;
                Month = CurrentCulture.TextInfo.ToTitleCase(DateCultureInfo.GetMonthName(value.Month));
                WeekNumber = DateCultureInfo.Calendar.GetWeekOfYear(value, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday);
                _date = value;
            }
        }

        public DateTimeFormatInfo DateCultureInfo { get; set; }

        public CultureInfo CurrentCulture { get; set; }

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

        private void UpdateCultureHandler()
        {
            SetDate(Date);
        }

        private void SetDate(DateTime date)
        {
            CurrentCulture = CultureInfo.CurrentCulture;
            DateCultureInfo = DateTimeFormatInfo.CurrentInfo;
            Date = date;
        }

        private void MonthChangedEventHandler(DateChangedEnum state)
        {
            if (_dateType != CalendarTypeEnum.Month) return;
            switch (state)
            {
                case DateChangedEnum.Increase:
                    Date = Date.AddMonths(1);
                    break;
                case DateChangedEnum.Decrease:
                    Date = Date.AddMonths(-1);
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
                    break;
                case DateChangedEnum.Decrease:
                    Date = Date.AddDays(-7);
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
                    break;
                case DateChangedEnum.Decrease:
                    Date = Date.AddDays(-1);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }
    }
}
