using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using Microsoft.Practices.Unity;
using Prism.Mvvm;
using Prism.Regions;
using TaskSharper.Domain.Calendar;

namespace TaskSharper.Calender.WPF.ViewModels
{
    public class CalendarEventsViewModel
    {
        private readonly IRegionManager _regionManager;
        public DateTime Date { get; }
        private const int HOURS_IN_A_DAY = 24;

        public ObservableCollection<CalendarEventViewModel> CalendarEvents { get; set; }
        

        public CalendarEventsViewModel(DateTime date, IRegionManager regionManager)
        {
            Date = date;
            _regionManager = regionManager;
            CalendarEvents = new ObservableCollection<CalendarEventViewModel>();
            InitializeView();
        }

        private void InitializeView()
        {
            for (int i = 0; i < HOURS_IN_A_DAY; i++)
            {
                CalendarEvents.Add(new CalendarEventViewModel(i, _regionManager));
            }
        }

        public void AddEvent(Event calendarEvent)
        {
            //TODO:: Refactor this to make use of observable dictionary
            
            var eventViewModel = CalendarEvents.FirstOrDefault(x => x.TimeOfDay == calendarEvent.Start.Value.Hour);
            var index = CalendarEvents.IndexOf(eventViewModel);

            
            var timespan = calendarEvent.End.Value.Hour - calendarEvent.Start.Value.Hour;

            for (int i = index; i < index + timespan; i++)
            {
                CalendarEvents[i].Id = calendarEvent.Id;
                CalendarEvents[i].Title = calendarEvent.Title;
                CalendarEvents[i].Description = calendarEvent.Description;
                CalendarEvents[i].Start = calendarEvent.Start.Value;
                CalendarEvents[i].End = calendarEvent.End.Value;
                CalendarEvents[i].Type = calendarEvent.Type;
            }
        }
    }
}
