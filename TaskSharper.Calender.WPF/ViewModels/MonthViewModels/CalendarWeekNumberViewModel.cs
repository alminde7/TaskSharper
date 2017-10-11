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
            get => _date;
            set => SetProperty(ref _date, value);
        }

        public int Week
        {
            get => _week;
            set => SetProperty(ref _week, value);
        }

        public CalendarWeekNumberViewModel(DateTime date)
        {
            SetDate(date);
        }

        public void SetDate(DateTime date)
        {
            Week = DateTimeFormatInfo.CurrentInfo.Calendar.GetWeekOfYear(date, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday);
            Date = date;
        }
    }
}
