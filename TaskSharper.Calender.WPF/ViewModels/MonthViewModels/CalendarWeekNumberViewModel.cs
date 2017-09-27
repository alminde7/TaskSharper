using System;
using System.Globalization;
using Prism.Mvvm;

namespace TaskSharper.Calender.WPF.ViewModels
{
    public class CalendarWeekNumberViewModel : BindableBase
    {
        private DateTime _date;
        private int _week;

        public DateTime Date
        {
            get { return _date; }
            set
            {
                Week = DateTimeFormatInfo.CurrentInfo.Calendar.GetWeekOfYear(value, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday);
                SetProperty(ref _date, value);
            }
        }

        public int Week
        {
            get { return _week; }
            set { SetProperty(ref _week, value); }
        }

        public CalendarWeekNumberViewModel(DateTime date)
        {
            Date = date;
        }
    }
}
