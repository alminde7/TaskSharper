using System;
using Prism.Mvvm;

namespace TaskSharper.Calender.WPF.ViewModels
{
    public class CalendarDateViewModel : BindableBase
    {

        public string DayOfWeek { get; set; } = DateTime.Now.DayOfWeek.ToString();
        public int DayOfMonth { get; set; } = DateTime.Now.Day;
    }
}
