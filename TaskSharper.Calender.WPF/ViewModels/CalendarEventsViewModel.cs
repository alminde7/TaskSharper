using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace TaskSharper.Calender.WPF.ViewModels
{
    public class CalendarEventsViewModel 
    {
        public ObservableCollection<CalendarEventViewModel> CalendarEvents { get; set; }

        public CalendarEventsViewModel()
        {
            CalendarEvents = new ObservableCollection<CalendarEventViewModel>();
            
            CalendarEvents.Add(new CalendarEventViewModel()
            {
                Title = "sdf",
                Description = "sdfsd"
            });

            CalendarEvents.Add(new CalendarEventViewModel()
            {
                Title = "Mads",
                Description = "Er til mænd"
            });
        }
    }
}
