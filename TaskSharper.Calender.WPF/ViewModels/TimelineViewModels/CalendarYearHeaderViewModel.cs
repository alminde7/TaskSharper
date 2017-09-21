using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Events;
using Prism.Mvvm;
using TaskSharper.Calender.WPF.Events;
using TaskSharper.Calender.WPF.Events.Resources;

namespace TaskSharper.Calender.WPF.ViewModels
{
    public class CalendarYearHeaderViewModel : BindableBase
    {
        private readonly CalendarTypeEnum _dateType;
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

        public CalendarYearHeaderViewModel(IEventAggregator eventAggregator, CalendarTypeEnum dateType)
        {
            _dateType = dateType;
            // Initialization
            CurrentCulture = CultureInfo.CurrentCulture;
            DateCultureInfo = DateTimeFormatInfo.CurrentInfo;
            Date = DateTime.Now;

            // Event subscriptions
            // Event subscription
            eventAggregator.GetEvent<DayChangedEvent>().Subscribe(DayChangedEventHandler);
            eventAggregator.GetEvent<WeekChangedEvent>().Subscribe(WeekChangedEventHandler);
            eventAggregator.GetEvent<MonthChangedEvent>().Subscribe(MonthChangedEventHandler);
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
