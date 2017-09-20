using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using TaskSharper.Calender.WPF.Events;
using TaskSharper.Calender.WPF.Events.Resources;
using TaskSharper.DataAccessLayer.Google;
using TaskSharper.Domain.Calendar;

namespace TaskSharper.Calender.WPF.ViewModels.MonthViewModels
{
    public class CalendarDateDayViewModel : BindableBase
    {
        private readonly IEventAggregator _eventAggregator;
        private int _dayOfMonth;
        private DateTime _date;

        public ObservableCollection<CalendarDayEventViewModel> CalendarEvents { get; set; }    
        public ICalendarService Service { get; set; }

        public DateTime Date
        {
            get => _date;
            set
            {
                DayOfMonth = value.Day;
                _date = value;
            }
        }

        public int DayOfMonth
        {
            get => _dayOfMonth;
            set => SetProperty(ref _dayOfMonth, value);
        }

        public CalendarDateDayViewModel(DateTime date, IEventAggregator eventAggregator, ICalendarService service)
        {
            _eventAggregator = eventAggregator;
            Date = date;
            Service = service;
            CalendarEvents = new ObservableCollection<CalendarDayEventViewModel>();

            //eventAggregator.GetEvent<DateChangedEvent>().Subscribe(WeekChangedEventHandler);

            InitializeView();

            Task.Run(GetEvents);
        }

        private void InitializeView()
        {
             CalendarEvents.Add(new CalendarDayEventViewModel());
        }

        private Task GetEvents()
        {
            _eventAggregator.GetEvent<SpinnerEvent>().Publish(EventResources.SpinnerEnum.Show);

            try
            {
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
