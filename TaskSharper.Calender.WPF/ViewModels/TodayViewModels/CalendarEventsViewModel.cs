using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Practices.ObjectBuilder2;
using Prism.Events;
using TaskSharper.Calender.WPF.Events;
using TaskSharper.Calender.WPF.Events.Resources;
using TaskSharper.DataAccessLayer.Google;
using TaskSharper.Domain.Calendar;

namespace TaskSharper.Calender.WPF.ViewModels
{
    public class CalendarEventsViewModel
    {
        private const int HOURS_IN_A_DAY = 24;

        private readonly IEventAggregator _eventAggregator;

        public DateTime Date { get; set; }
        public ICalendarService Service { get; }

        public ObservableCollection<CalendarEventViewModel> CalendarEvents { get; set; }

        public CalendarEventsViewModel(DateTime date, IEventAggregator eventAggregator, ICalendarService service)
        {
            _eventAggregator = eventAggregator;
            Date = date;
            Service = service;
            CalendarEvents = new ObservableCollection<CalendarEventViewModel>();

            eventAggregator.GetEvent<WeekChangedEvent>().Subscribe(WeekChangedEventHandler);
            
            InitializeView();

            Task.Run(GetEvents);
        }
        
        private void InitializeView()
        {
            for (int i = 0; i < HOURS_IN_A_DAY; i++)
            {
                CalendarEvents.Add(new CalendarEventViewModel(i));
            }
        }

        private void WeekChangedEventHandler(WeekChangedEnum state)
        {
            switch (state)
            {
                case WeekChangedEnum.Increase:
                    Date = Date.AddDays(7);
                    CalendarEvents.ForEach(x => x.Event = null);

                    Task.Run(GetEvents);
                    break;
                case WeekChangedEnum.Decrease:
                    Date = Date.AddDays(-7);
                    CalendarEvents.ForEach(x => x.Event = null);
                    Task.Run(GetEvents);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        private Task GetEvents()
        {
            _eventAggregator.GetEvent<SpinnerEvent>().Publish(EventResources.SpinnerEnum.Show);

            var calendarEvents = Service.GetEvents(Date.Date, Date.Date.AddDays(1).AddTicks(-1), Constants.DefaultGoogleCalendarId);

            foreach (var calendarEvent in calendarEvents)
            {
                if (!calendarEvent.Start.HasValue || !calendarEvent.End.HasValue) continue;

                var eventTimespan = calendarEvent.End.Value.Hour - calendarEvent.Start.Value.Hour;
                var startIndex = calendarEvent.Start.Value.Hour;

                for (int i = startIndex; i < startIndex + eventTimespan; i++)
                {
                    CalendarEvents[i].Event = calendarEvent;
                }
            }
            _eventAggregator.GetEvent<SpinnerEvent>().Publish(EventResources.SpinnerEnum.Hide);
            return Task.CompletedTask;
        }

    }
}
