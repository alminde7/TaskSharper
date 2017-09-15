using System;
using Prism.Events;
using Prism.Mvvm;
using TaskSharper.Domain.Calendar;

namespace TaskSharper.Calender.WPF.ViewModels
{
    public class CalendarTodayViewModel : BindableBase
    {
        public IEventAggregator EventAggregator { get; }
        public ICalendarService CalendarService { get; }

        public CalendarEventsViewModel EventsViewModel { get; set; }
        public CalendarDateViewModel DateViewModel { get; set; }
        public DateTime CurrentDate { get; set; }

        public CalendarTodayViewModel(IEventAggregator eventAggregator, ICalendarService calendarService)
        {
            EventAggregator = eventAggregator;
            CalendarService = calendarService;
            CurrentDate = DateTime.Now;

            // Initialize views
            EventsViewModel = new CalendarEventsViewModel(CurrentDate, eventAggregator, calendarService);
            DateViewModel = new CalendarDateViewModel(CurrentDate, eventAggregator);
        }
    }
}
