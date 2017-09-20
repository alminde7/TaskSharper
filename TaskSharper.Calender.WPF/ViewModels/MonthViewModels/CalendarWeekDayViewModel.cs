using Prism.Events;
using Prism.Mvvm;
using System;
using System.Globalization;
using TaskSharper.Calender.WPF.Events.Resources;

namespace TaskSharper.Calender.WPF.ViewModels.MonthViewModels
{
    public class CalendarWeekDayViewModel : BindableBase
    {
        private string _dayOfWeek;
        private DateTime _currentDate;

        public string DayOfWeek
        {
            get => _dayOfWeek;
            set => SetProperty(ref _dayOfWeek, value);
        }

        public DateTime CurrentDate
        {
            get => _currentDate;
            set
            {
                DayOfWeek = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(DateTimeFormatInfo.CurrentInfo.GetDayName(value.DayOfWeek));
                _currentDate = value;
            }
        }

        public CalendarWeekDayViewModel(DateTime date)
        {
            CurrentDate = date;
        }
    }
}
