using System;
using System.Collections.ObjectModel;

namespace TaskSharper.Calender.WPF.ViewModels
{
    public class CalendarEventsViewModel
    {
        private const int HOURS_IN_A_DAY = 24;

        public ObservableCollection<CalendarEventViewModel> CalendarEvents { get; set; }

        public CalendarEventsViewModel()
        {
            CalendarEvents = new ObservableCollection<CalendarEventViewModel>();
            InitializeView();
        }

        private void InitializeView()
        {

            var start = DateTime.Now;
            var end = DateTime.Now.AddHours(2);
            for (int i = 0; i < 24; i++)
            {
                CalendarEvents.Add(new CalendarEventViewModel(i));
            }

            CalendarEvents[15].Start = start;
            CalendarEvents[15].End = end;
            CalendarEvents[15].Title = "Haløj";
            CalendarEvents[15].Description = "dfsfd";

            CalendarEvents[16].Start = start;
            CalendarEvents[16].End = end;

            CalendarEvents[17].Start = start;
            CalendarEvents[17].End = end;
        }
    }
}
