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
            Date = date;

            CurrentCulture = CultureInfo.CurrentCulture;
            DateCultureInfo = DateTimeFormatInfo.CurrentInfo;

            Year = date.Year;
            Month = CurrentCulture.TextInfo.ToTitleCase(DateCultureInfo.GetMonthName(date.Month));
            WeekNumber = DateCultureInfo.Calendar.GetWeekOfYear(date, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday);
        }

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
