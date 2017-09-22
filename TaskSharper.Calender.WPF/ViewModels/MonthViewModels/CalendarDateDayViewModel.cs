using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using TaskSharper.Calender.WPF.Events;
using TaskSharper.Calender.WPF.Events.Resources;
using TaskSharper.DataAccessLayer.Google;
using TaskSharper.Domain.BusinessLayer;
using TaskSharper.Domain.Calendar;
using TaskSharper.Calender.WPF.Events;
using TaskSharper.Calender.WPF.Events.Resources;
using Microsoft.Practices.ObjectBuilder2;

namespace TaskSharper.Calender.WPF.ViewModels.MonthViewModels
{
    public class CalendarDateDayViewModel : BindableBase
    {
        private readonly IEventAggregator _eventAggregator;
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

        public CalendarDateDayViewModel(DateTime date, IEventAggregator eventAggregator, IEventManager eventManager)
        {
            _eventAggregator = eventAggregator;
            Date = date;
            EventManager = eventManager;
            CalendarEvents = new ObservableCollection<CalendarDayEventViewModel>();

            eventAggregator.GetEvent<DateChangedEvent>().Subscribe(MonthChangedEventHandler);

            InitializeView();

            GetEvents();
        }

        private void InitializeView()
        {
            
        }

        private void MonthChangedEventHandler(DateChangeEnum state)
        {
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
                case DateChangeEnum.IncreaseDay:
                    Date = Date.AddDays(1);
                    UpdateView();
                    break;
                case DateChangeEnum.DecreaseDay:
                    Date = Date.AddDays(-1);
                    UpdateView();
                    break;
                case DateChangeEnum.IncreaseMonth:
                    Date = Date.AddDays(28);
                    UpdateView();
                    break;
                case DateChangeEnum.DecreaseMonth:
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

        public void GetEvents()
        {
            _eventAggregator.GetEvent<SpinnerEvent>().Publish(EventResources.SpinnerEnum.Show);

            try
            {
                var calendarEvents = EventManager.GetEvents(Date);

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
                // TODO:: Log exception:Handle exception:Show message to user(maybe)

            }
        }
    }
}
