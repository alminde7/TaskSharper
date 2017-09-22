using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using Prism.Events;
using Prism.Mvvm;
using TaskSharper.Calender.WPF.Events;
using TaskSharper.Calender.WPF.Events.Resources;

namespace TaskSharper.Calender.WPF.ViewModels
{
    public class CalendarDateViewModel : BindableBase
    {
        private string _dayOfWeek;
        private int _dayOfMonth;
        private DateTime _currentDate;

        public string DayOfWeek
        {
            get => _dayOfWeek;
            set => SetProperty(ref _dayOfWeek, value);
        }

        public int DayOfMonth
        {
            get => _dayOfMonth;
            set => SetProperty(ref _dayOfMonth, value);
        }

        public DateTime CurrentDate
        {
            get => _currentDate;
            set
            {
                DayOfWeek = CurrentCulture.TextInfo.ToTitleCase(DateCultureInfo.GetDayName(value.DayOfWeek));
                DayOfMonth = value.Day;
                _currentDate = value;
            }
        }

        public DateTimeFormatInfo DateCultureInfo { get; set; }
        public CultureInfo CurrentCulture { get; set; }

        public CalendarDateViewModel(DateTime date, IEventAggregator eventAggregator)
        {
            // Initialization
            CurrentCulture = CultureInfo.CurrentCulture;
            DateCultureInfo = DateTimeFormatInfo.CurrentInfo;
            CurrentDate = date;

            // Event subscription
            eventAggregator.GetEvent<DateChangedEvent>().Subscribe(WeekChangedEvent);
        }

        private void WeekChangedEvent(DateChangeEnum newDate)
        {
            switch (newDate)
            {
                case DateChangeEnum.IncreaseWeek:
                    CurrentDate = CurrentDate.AddDays(7);
                    break;
                case DateChangeEnum.DecreaseWeek:
                    CurrentDate = CurrentDate.AddDays(-7);
                    break;
                case DateChangeEnum.IncreaseDay:
                    CurrentDate = CurrentDate.AddDays(1);
                    break;
                case DateChangeEnum.DecreaseDay:
                    CurrentDate = CurrentDate.AddDays(-1);
                    break;
                case DateChangeEnum.IncreaseMonth:
                    CurrentDate = CurrentDate.AddMonths(1);
                    break;
                case DateChangeEnum.DecreaseMonth:
                    CurrentDate = CurrentDate.AddMonths(-1);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(newDate), newDate, null);
            }
        }
    }
}
