using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Practices.ObjectBuilder2;
using Prism.Events;
using Prism.Regions;
using Serilog;
using TaskSharper.BusinessLayer;
using TaskSharper.Calender.WPF.Events;
using TaskSharper.Calender.WPF.Events.Resources;
using TaskSharper.Domain.BusinessLayer;
using TaskSharper.Domain.Calendar;

namespace TaskSharper.Calender.WPF.ViewModels
{
    public class CalendarEventsViewModel
    {
        private const int HoursInADay = 24;

        private readonly IEventAggregator _eventAggregator;
        private readonly IRegionManager _regionManager;
        private readonly CalendarTypeEnum _dateType;
        private readonly ILogger _logger;

        public DateTime Date { get; set; }
        public IEventManager Service { get; set; }

        public ObservableCollection<CalendarEventViewModel> CalendarEvents { get; set; }

        public CalendarEventsViewModel(DateTime date, IEventAggregator eventAggregator, IRegionManager regionManager, IEventManager service, CalendarTypeEnum dateType, ILogger logger)
        {
            _eventAggregator = eventAggregator;
            _regionManager = regionManager;
            _dateType = dateType;
            _logger = logger.ForContext<CalendarEventsViewModel>();
            Date = date;
            Service = service;
            CalendarEvents = new ObservableCollection<CalendarEventViewModel>();


            _eventAggregator.GetEvent<DayChangedEvent>().Subscribe(DayChangedEventHandler);
            _eventAggregator.GetEvent<WeekChangedEvent>().Subscribe(WeekChangedEventHandler);
            _eventAggregator.GetEvent<MonthChangedEvent>().Subscribe(MonthChangedEventHandler);
            eventAggregator.GetEvent<EventChangedEvent>().Subscribe(EventChangedEventHandler);

            InitializeView();

            Task.Run(GetEvents);
        }

        #region EventHandlers
        private void MonthChangedEventHandler(DateChangedEnum state)
        {
            if (_dateType != CalendarTypeEnum.Month) return;
            switch (state)
            {
                case DateChangedEnum.Increase:
                    Date = Date.AddMonths(1);
                    UpdateView();
                    break;
                case DateChangedEnum.Decrease:
                    Date = Date.AddMonths(-1);
                    UpdateView();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        private void WeekChangedEventHandler(DateChangedEnum state)
        {
            if (_dateType != CalendarTypeEnum.Week) return;
            switch (state)
            {
                case DateChangedEnum.Increase:
                    Date = Date.AddDays(7);
                    UpdateView();
                    break;
                case DateChangedEnum.Decrease:
                    Date = Date.AddDays(-7);
                    UpdateView();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        private void DayChangedEventHandler(DateChangedEnum state)
        {
            if (_dateType != CalendarTypeEnum.Day) return;
            switch (state)
            {
                case DateChangedEnum.Increase:
                    Date = Date.AddDays(1);
                    UpdateView();
                    break;
                case DateChangedEnum.Decrease:
                    Date = Date.AddDays(-1);
                    UpdateView();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        private void EventChangedEventHandler(Event obj)
        {
            if (Date.Date == obj.Start.Value.Date)
            {
                Service.UpdateEvent(obj);
                UpdateView();
            }
        }
        #endregion

        private void UpdateView()
        {
            CalendarEvents.ForEach(x => x.Event = null);
            Task.Run(GetEvents);
        }

        private void InitializeView()
        {
            for (int i = 0; i < HoursInADay; i++)
            {
                CalendarEvents.Add(new CalendarEventViewModel(i, _regionManager, _logger));
            }
        }

        private Task GetEvents()
        {
            _eventAggregator.GetEvent<SpinnerEvent>().Publish(EventResources.SpinnerEnum.Show);

            try
            {
                var calendarEvents = Service.GetEvents(Date.Date);

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
            }
            catch (Exception e)
            {
                _eventAggregator.GetEvent<SpinnerEvent>().Publish(EventResources.SpinnerEnum.Hide);
                _logger.Error(e, "Error orcurred while getting event data");

            }
            return Task.CompletedTask;
        }

    }
}
