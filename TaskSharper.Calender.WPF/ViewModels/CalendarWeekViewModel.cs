using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using TaskSharper.Calender.WPF.Events;
using TaskSharper.Calender.WPF.Events.Resources;
using Prism.Commands;
using TaskSharper.DataAccessLayer.Google;
using TaskSharper.Domain.Calendar;

namespace TaskSharper.Calender.WPF.ViewModels
{
    public class CalendarWeekViewModel : BindableBase 
    {
        private const int DAYS_IN_WEEK = 7;

        private readonly ICalendarService _service;
        private IEventAggregator _eventAggregator;
        private readonly IRegionManager _regionManager;

        public DelegateCommand NextCommand { get; set; }

        public DelegateCommand PrevCommand { get; set; }

        public ObservableCollection<CalendarDateViewModel> DateHeaders { get; set; }
        public ObservableCollection<CalendarEventsViewModel> EventContainers { get; set; }

        
        public int WeeklyOffset { get; set; } = 0;


        public CalendarWeekViewModel(ICalendarService service, IEventAggregator eventAggregator, IRegionManager regionManager)
        {


            NextCommand = new DelegateCommand(NextWeek);
            PrevCommand = new DelegateCommand(PrevWeek);
            _service = service;
            _eventAggregator = eventAggregator;
            _regionManager = regionManager;
            DateHeaders = new ObservableCollection<CalendarDateViewModel>();
            EventContainers = new ObservableCollection<CalendarEventsViewModel>();

            _eventAggregator.GetEvent<SpinnerEvent>().Publish(EventResources.SpinnerEnum.Show);

            InitializeViews();
            Task.Run(GetCalendarEvents);
            
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
            Task.Run(GetCalendarEvents);
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
                EventContainers.Add(new CalendarEventsViewModel(date, _regionManager));
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
            var events = _service.GetEvents(DateHeaders.First().Date, DateHeaders.Last().Date, Constants.DefaultGoogleCalendarId);

            //TODO:: Refactor this to make use of observable dictionary
            foreach (var calendarEvent in events)
            {
                // Do not allow events without start and end time (small hack)
                if (!calendarEvent.Start.HasValue || !calendarEvent.End.HasValue) continue;

                // TODO:: Remove this when add functionality is complete
                calendarEvent.Type = Event.EventType.Appointment;

                var container = EventContainers.FirstOrDefault(x => x.Date.Day == calendarEvent.Start.Value.Day && x.Date.Month == calendarEvent.Start.Value.Month && x.Date.Year == calendarEvent.Start.Value.Date.Year);
                if (container != null)
                {
                    var index = EventContainers.IndexOf(container);

                    EventContainers[index].AddEvent(calendarEvent);
                }
            }

            _eventAggregator.GetEvent<SpinnerEvent>().Publish(EventResources.SpinnerEnum.Hide);
            
            return Task.CompletedTask;
        }
    }
}
