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

        public CalendarYearHeaderViewModel(IEventAggregator eventAggregator)
        {
            // Initialization
            CurrentCulture = CultureInfo.CurrentCulture;
            DateCultureInfo = DateTimeFormatInfo.CurrentInfo;
            Date = DateTime.Now;

            // Event subscriptions
            eventAggregator.GetEvent<WeekChangedEvent>().Subscribe(WeekChangedEventHandler);
        }

        private void WeekChangedEventHandler(DateChangeEnum state)
        {
            switch (state)
            {
                case DateChangeEnum.Increase_Week:
                    Date = Date.AddDays(7);
                    break;
                case DateChangeEnum.Decrease_Week:
                    Date = Date.AddDays(-7);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }
    }
}
