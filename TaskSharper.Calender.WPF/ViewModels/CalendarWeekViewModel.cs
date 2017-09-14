using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using TaskSharper.Calender.WPF.Events;
using TaskSharper.Calender.WPF.Events.Resources;

namespace TaskSharper.Calender.WPF.ViewModels
{
    public class CalendarWeekViewModel : BindableBase
    {
        private readonly CalendarEventsService _service;
        private const int DAYS_IN_WEEK = 7;
        private IEventAggregator _eventAggregator;
        private readonly IRegionManager _regionManager;

        public ObservableCollection<CalendarDateViewModel> DateHeaders { get; set; }
        public ObservableCollection<CalendarEventsViewModel> EventContainers { get; set; }

        public CalendarWeekViewModel(CalendarEventsService service, IEventAggregator eventAggregator, IRegionManager regionManager)
        {
            _service = service;
            _eventAggregator = eventAggregator;
            _regionManager = regionManager;
            DateHeaders = new ObservableCollection<CalendarDateViewModel>();
            EventContainers = new ObservableCollection<CalendarEventsViewModel>();

            _eventAggregator.GetEvent<SpinnerEvent>().Publish(EventResources.SpinnerEnum.Show);
            InitializeViews();

            GetCalelndarEvents();
            _eventAggregator.GetEvent<SpinnerEvent>().Publish(EventResources.SpinnerEnum.Hide);
        }


        private void InitializeViews()
        {
            for (int i = 1; i <= DAYS_IN_WEEK; i++)
            {
                var date = CalculateDate(i);
                DateHeaders.Add(new CalendarDateViewModel(date));
                EventContainers.Add(new CalendarEventsViewModel(date, _regionManager));
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
            var events = _service.GetEvents();

            //TODO:: Refactor this to make use of observable dictionary
            foreach (var calendarEvent in events)
            {
                var container = EventContainers.FirstOrDefault(x => x.Date.Day == calendarEvent.Start.Day);
                var index = EventContainers.IndexOf(container);
                
                EventContainers[index].AddEvent(calendarEvent);
            }
            
            return Task.CompletedTask;
        }
    }

    public class CalendarEventsService
    {
        public List<Events> GetEvents()
        {
            var list = new List<Events>()
            {
                new Events()
                {
                    Title = "The title", 
                    Description = "A description",
                    Start = DateTime.Now,
                    End = DateTime.Now.AddHours(3)
                },
                new Events()
                {
                    Title = "The new title",
                    Description = "A new description",
                    Start = DateTime.Now.AddDays(3),
                    End = DateTime.Now.AddDays(3).AddHours(3)
                },
                new Events()
                {
                    Title = "Cool title",
                    Description = "crappy description",
                    Start = DateTime.Now.AddDays(3).AddHours(-4),
                    End = DateTime.Now.AddDays(3).AddHours(-2)
                }
            };

            return list;
        }
    }

    public class Events
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
}
