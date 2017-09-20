using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Practices.ObjectBuilder2;
using Prism.Events;
using TaskSharper.Calender.WPF.Events;
using TaskSharper.Calender.WPF.Events.Resources;
using TaskSharper.Domain.BusinessLayer;

namespace TaskSharper.Calender.WPF.ViewModels
{
    public class CalendarEventsViewModel
    {
        private const int HoursInADay = 24;

        private readonly IEventAggregator _eventAggregator;
        private readonly CalendarTypeEnum _dateType;

        public DateTime Date { get; set; }
        public IEventManager Service { get; set; }

        public ObservableCollection<CalendarEventViewModel> CalendarEvents { get; set; }

        public CalendarEventsViewModel(DateTime date, IEventAggregator eventAggregator, IEventManager service, CalendarTypeEnum dateType)
        {
            _eventAggregator = eventAggregator;
            _dateType = dateType;
            Date = date;
            Service = service;
            CalendarEvents = new ObservableCollection<CalendarEventViewModel>();

            eventAggregator.GetEvent<DateChangedEvent>().Subscribe(WeekChangedEventHandler);

            InitializeView();

            Task.Run(GetEvents);
        }

        private void InitializeView()
        {
            for (int i = 0; i < HoursInADay; i++)
            {
                CalendarEvents.Add(new CalendarEventViewModel(i));
            }
        }

        private void WeekChangedEventHandler(DateChangeEnum state)
        {
            switch (_dateType)
            {
                case CalendarTypeEnum.Day:
                    switch (state)
                    {
                        case DateChangeEnum.IncreaseDay:
                            Date = Date.AddDays(1);
                            UpdateView();
                            break;
                        case DateChangeEnum.DecreaseDay:
                            Date = Date.AddDays(-1);
                            UpdateView();
                            break;
                        default:
                            break;
                    }
                    break;
                case CalendarTypeEnum.Week:
                    switch (state)
                    {
                        case DateChangeEnum.IncreaseWeek:
                            Date = Date.AddDays(7);
                            UpdateView();
                            break;
                        case DateChangeEnum.DecreaseWeek:
                            Date = Date.AddDays(-7);
                            UpdateView();
                            break;
                        default:
                            break;
                    }

                    break;
                case CalendarTypeEnum.Month:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void UpdateView()
        {
            CalendarEvents.ForEach(x => x.Event = null);
            Task.Run(GetEvents);
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
                // TODO:: Log exception:Handle exception:Show message to user(maybe)

            }
            return Task.CompletedTask;
        }

    }
}
