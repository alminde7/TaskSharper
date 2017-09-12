using System;
using System.Globalization;
using Prism.Mvvm;

namespace TaskSharper.Calender.WPF.ViewModels
{
    public class CalendarDateViewModel : BindableBase
    {

        public string DayOfWeek { get; set; }
        public int DayOfMonth { get; set; }

        public CalendarDateViewModel(DateTime date)
        {
            DayOfWeek = date.DayOfWeek.ToString();
            DayOfMonth = date.Day;
        }
    }
}
