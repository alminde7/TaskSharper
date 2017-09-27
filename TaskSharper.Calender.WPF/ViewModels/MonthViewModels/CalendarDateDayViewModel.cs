using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Serilog;
using TaskSharper.Calender.WPF.Events;
using TaskSharper.Calender.WPF.Events.Resources;
using TaskSharper.Domain.BusinessLayer;

namespace TaskSharper.Calender.WPF.ViewModels.MonthViewModels
{
    public class CalendarDateDayViewModel : BindableBase
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly CalendarTypeEnum _dateType;
        private readonly ILogger _logger;
        private int _dayOfMonth;
        private DateTime _date;

        public ObservableCollection<CalendarDayEventViewModel> CalendarEvents { get; set; }    
        public IEventManager EventManager { get; set; }

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

        public CalendarDateDayViewModel(DateTime date, IEventAggregator eventAggregator, IEventManager eventManager, CalendarTypeEnum dateType, ILogger logger)
        {
            _eventAggregator = eventAggregator;
            _dateType = dateType;
            _logger = logger.ForContext<CalendarDateDayViewModel>();
            Date = date;
            EventManager = eventManager;
            CalendarEvents = new ObservableCollection<CalendarDayEventViewModel>();
            
            eventAggregator.GetEvent<MonthChangedEvent>().Subscribe(MonthChangedEventHandler);
            
            GetEvents();
        }

        private void MonthChangedEventHandler(DateChangedEnum state)
        {
            if (_dateType != CalendarTypeEnum.Month) return;
            switch (state)
            {
                case DateChangedEnum.Increase:
                    Date = Date.AddDays(28);
                    UpdateView();
                    break;
                case DateChangedEnum.Decrease:
                    Date = Date.AddDays(-28);
                    UpdateView();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        private void UpdateView()
        {
            CalendarEvents.Clear();
            GetEvents();
        }

        public void UpdateDate(DateTime date)
        {
            Date = date;
            UpdateView();
        }

        public async void GetEvents()
        {
            _eventAggregator.GetEvent<SpinnerEvent>().Publish(EventResources.SpinnerEnum.Show);

            try
            {
                var calendarEvents = await EventManager.GetEventsAsync(Date);

                foreach (var calendarEvent in calendarEvents)
                {
                    if (!calendarEvent.Start.HasValue || !calendarEvent.End.HasValue) continue;
                    CalendarEvents.Add(new CalendarDayEventViewModel() { Event = calendarEvent });
                    

                }
                _eventAggregator.GetEvent<SpinnerEvent>().Publish(EventResources.SpinnerEnum.Hide);
            }
            catch (Exception e)
            {
                _eventAggregator.GetEvent<SpinnerEvent>().Publish(EventResources.SpinnerEnum.Hide);
                _logger.Error(e, "Failed to update view");
            }
        }
    }
}
