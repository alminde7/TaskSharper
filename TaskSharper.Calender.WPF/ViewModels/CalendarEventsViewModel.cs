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

            for (int i = 0; i < 24; i++)
            {
                CalendarEvents.Add(new CalendarEventViewModel()
                {
                    Title = "Hej",
                    Description = "Jeppe"
                });
            }
            
            //CalendarEvents.Add(new CalendarEventViewModel()
            //{
            //    Title = "sdf",
            //    Description = "sdfsd"
            //});

            //CalendarEvents.Add(new CalendarEventViewModel()
            //{
            //    Title = "Mads",
            //    Description = "Er til mænd"
            //});

            CalendarEvents[12].Title = "Jeppe";
            CalendarEvents[12].Description = "Er smuk";
        }


    }
}
