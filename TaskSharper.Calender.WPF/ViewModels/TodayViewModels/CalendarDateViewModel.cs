using System;
using Prism.Mvvm;

namespace TaskSharper.Calender.WPF.ViewModels
{
    public class CalendarDateViewModel : BindableBase
    {

        public string DayOfWeek { get; set; } = DateTime.Now.DayOfWeek.ToString();
        public int DayOfMonth { get; set; } = DateTime.Now.Day;

        public CalendarDateViewModel()
        {
            
        }

        public CalendarDateViewModel(int dayOfMonth = 0)
        {
            SetDate(dayOfMonth);
        }

        private void SetDate(int today)
        {
            var dayOffset = today - (int)DateTime.Now.DayOfWeek;

            var dateTime = DateTime.Now.AddDays(dayOffset);

            DayOfMonth = dateTime.Day;
            DayOfWeek = dateTime.DayOfWeek.ToString();
        }
    }
}
