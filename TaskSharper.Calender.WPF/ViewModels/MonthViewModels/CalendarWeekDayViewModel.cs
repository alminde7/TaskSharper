using Prism.Events;
using Prism.Mvvm;
using System;
using System.Globalization;
using TaskSharper.Calender.WPF.Events.Resources;

namespace TaskSharper.Calender.WPF.ViewModels.MonthViewModels
{
    public class CalendarWeekDayViewModel : BindableBase
    {
        private string day;

        public string Day
        {
            get => day;
            set => SetProperty(ref day, value);
        }

        public CalendarWeekDayViewModel(int dayofweek)
        {
            if (dayofweek == 7)
                dayofweek = 0;

            Day = ((DayOfWeek) Enum.ToObject(typeof(DayOfWeek), dayofweek)).ToString();
        }
    }
}
