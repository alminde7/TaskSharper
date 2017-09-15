using System;
using Prism.Events;
using Prism.Mvvm;
using TaskSharper.Domain.Calendar;

namespace TaskSharper.Calender.WPF.ViewModels
{
    public class CalendarTodayViewModel : BindableBase
    {
        private CalendarEventsViewModel _eventsViewModel;
        private CalendarDateViewModel _dateViewModel;
        public IEventAggregator EventAggregator { get; }
        public ICalendarService CalendarService { get; }

        public CalendarEventsViewModel EventsViewModel
        {
            get => _eventsViewModel;
            set => SetProperty(ref _eventsViewModel, value);
        }

        public CalendarDateViewModel DateViewModel
        {
            get => _dateViewModel;
            set => SetProperty(ref _dateViewModel, value);
        }

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
