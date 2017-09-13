using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace TaskSharper.Calender.WPF.ViewModels
{
    public class CalendarEventsViewModel
    {
        public DateTime Date { get; }
        private const int HOURS_IN_A_DAY = 24;

        public ObservableCollection<CalendarEventViewModel> CalendarEvents { get; set; }

        public CalendarEventsViewModel(DateTime date)
        {
            Date = date;
            CalendarEvents = new ObservableCollection<CalendarEventViewModel>();
            InitializeView();
        }

        private void InitializeView()
        {
            for (int i = 0; i < 24; i++)
            {
                CalendarEvents.Add(new CalendarEventViewModel(i));
            }
        }

        public void AddEvent(Events calendarEvent)
        {
            //TODO:: Refactor this to make use of observable dictionary
            var eventViewModel = CalendarEvents.FirstOrDefault(x => x.TimeOfDay == calendarEvent.Start.Hour);
            var index = CalendarEvents.IndexOf(eventViewModel);

            var timespan = calendarEvent.End.Hour - calendarEvent.Start.Hour;

            for (int i = index; i < index + timespan; i++)
            {
                CalendarEvents[i].Title = calendarEvent.Title;
                CalendarEvents[i].Description = calendarEvent.Description;
                CalendarEvents[i].Start = calendarEvent.Start;
                CalendarEvents[i].End = calendarEvent.End;
            }
        }
    }
}
