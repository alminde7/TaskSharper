using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using TaskSharper.Calender.WPF.Events;
using TaskSharper.Calender.WPF.Events.Resources;
using TaskSharper.DataAccessLayer.Google;
using TaskSharper.Domain.Calendar;

namespace TaskSharper.Calender.WPF.ViewModels
{
    public class CalendarWeekViewModel : BindableBase
    {
        private const int DAYS_IN_WEEK = 7;

        private readonly ICalendarService _service;
        private IEventAggregator _eventAggregator;

        public ObservableCollection<CalendarDateViewModel> DateHeaders { get; set; }
        public ObservableCollection<CalendarEventsViewModel> EventContainers { get; set; }

        public CalendarWeekViewModel(ICalendarService service, IEventAggregator eventAggregator)
        {
            _service = service;
            _eventAggregator = eventAggregator;

            DateHeaders = new ObservableCollection<CalendarDateViewModel>();
            EventContainers = new ObservableCollection<CalendarEventsViewModel>();

            _eventAggregator.GetEvent<SpinnerEvent>().Publish(EventResources.SpinnerEnum.Show);

            InitializeViews();

            Task.Run(GetCalelndarEvents);

            _eventAggregator.GetEvent<SpinnerEvent>().Publish(EventResources.SpinnerEnum.Hide);
        }

        private void InitializeViews()
        {
            for (int i = 1; i <= DAYS_IN_WEEK; i++)
            {
                var date = CalculateDate(i);
                DateHeaders.Add(new CalendarDateViewModel(date));
                EventContainers.Add(new CalendarEventsViewModel(date));
            }
        }

        private DateTime CalculateDate(int day)
        {
            var dayOffset = day - (int)DateTime.Now.DayOfWeek;
            
            if (day == 0)
                dayOffset += 7;

            var dateTime = DateTime.Now.AddDays(dayOffset);

            return dateTime;
        }

        public Task GetCalelndarEvents()
        {
            var events = _service.GetEvents(DateHeaders.First().Date, DateHeaders.Last().Date, Constants.DefaultGoogleCalendarId);

            //TODO:: Refactor this to make use of observable dictionary
            foreach (var calendarEvent in events)
            {
                // Do not allow events without start and end time (small hack)
                if (!calendarEvent.Start.HasValue || !calendarEvent.End.HasValue) continue;

                // TODO:: Remove this when add functionality is complete
                calendarEvent.Type = Event.EventType.Appointment;
                
                var container = EventContainers.FirstOrDefault(x => x.Date.Day == calendarEvent.Start.Value.Day);
                var index = EventContainers.IndexOf(container);
                
                EventContainers[index].AddEvent(calendarEvent);
            }
            
            return Task.CompletedTask;
        }
    }
}
