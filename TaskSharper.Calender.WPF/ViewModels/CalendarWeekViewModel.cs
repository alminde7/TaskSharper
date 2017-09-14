using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using TaskSharper.Calender.WPF.Events;
using TaskSharper.Calender.WPF.Events.Resources;
using Prism.Commands;

namespace TaskSharper.Calender.WPF.ViewModels
{
    public class CalendarWeekViewModel : BindableBase 
    {
        private readonly CalendarEventsService _service;
        private const int DAYS_IN_WEEK = 7;
        private IEventAggregator _eventAggregator;

        public DelegateCommand NextCommand { get; set; }

        public DelegateCommand PrevCommand { get; set; }

        public ObservableCollection<CalendarDateViewModel> DateHeaders { get; set; }
        public ObservableCollection<CalendarEventsViewModel> EventContainers { get; set; }

        public int WeeklyOffset { get; set; } = 0;


        public CalendarWeekViewModel(CalendarEventsService service, IEventAggregator eventAggregator)
        {


            NextCommand = new DelegateCommand(NextWeek);
            PrevCommand = new DelegateCommand(PrevWeek);
            _service = service;
            _eventAggregator = eventAggregator;
            DateHeaders = new ObservableCollection<CalendarDateViewModel>();
            EventContainers = new ObservableCollection<CalendarEventsViewModel>();
            _eventAggregator.GetEvent<SpinnerEvent>().Publish(EventResources.SpinnerEnum.Show);
            InitializeViews();
            GetCalendarEvents();
            _eventAggregator.GetEvent<SpinnerEvent>().Publish(EventResources.SpinnerEnum.Hide);
        }

        private void NextWeek()
        {
            WeeklyOffset++;
            ReconstructView();
        }

        private void PrevWeek()
        {
            WeeklyOffset--;
            ReconstructView();
        }
        private void ReconstructView()
        {
            DateHeaders.Clear();
            EventContainers.Clear();
            InitializeViews();
            GetCalendarEvents();
        }

        private void InitializeViews()
        {
            SetupDates(WeeklyOffset);
        }
        public void SetupDates(int weeklyoffset)
        {
            int daysOffset = weeklyoffset * 7;
            for (int i = 1; i <= DAYS_IN_WEEK; i++)
            {
                var date = CalculateDate(i + daysOffset);
                DateHeaders.Add(new CalendarDateViewModel(date));
                EventContainers.Add(new CalendarEventsViewModel(date));
            }
        }

        private DateTime CalculateDate(int day)
        {
            var dayOffset = day - (int)DateTime.Now.DayOfWeek;

            var dateTime = DateTime.Now.AddDays(dayOffset);

            return dateTime;
        }

        public Task GetCalendarEvents()
        {
            _eventAggregator.GetEvent<SpinnerEvent>().Publish(EventResources.SpinnerEnum.Show);
            var events = _service.GetEvents();

            //TODO:: Refactor this to make use of observable dictionary
            foreach (var calendarEvent in events)
            {
                var container = EventContainers.FirstOrDefault(x => x.Date.Day == calendarEvent.Start.Day && x.Date.Month == calendarEvent.Start.Month && x.Date.Year == x.Date.Year);
                if(container != null)
                {
                    var index = EventContainers.IndexOf(container);

                    EventContainers[index].AddEvent(calendarEvent);
                }
            }

            _eventAggregator.GetEvent<SpinnerEvent>().Publish(EventResources.SpinnerEnum.Hide);
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
                    Start = DateTime.Now.AddDays(1),
                    End = DateTime.Now.AddDays(1).AddHours(3)
                },
                new Events()
                {
                    Title = "The new title",
                    Description = "A new description",
                    Start = DateTime.Now.AddDays(10),
                    End = DateTime.Now.AddDays(10).AddHours(3)
                },
                new Events()
                {
                    Title = "Cool title",
                    Description = "crappy description",
                    Start = DateTime.Now.AddDays(9).AddHours(-4),
                    End = DateTime.Now.AddDays(9).AddHours(-2)
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
