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

            //eventAggregator.GetEvent<DateChangedEvent>().Subscribe(WeekChangedEventHandler);

            InitializeView();

            GetEvents();
        }

        private void InitializeView()
        {
             
        }

        private void GetEvents()
        {
            _eventAggregator.GetEvent<SpinnerEvent>().Publish(EventResources.SpinnerEnum.Show);

            try
            {
                var calendarEvents = EventManager.GetEvents(Date);

                foreach (var calendarEvent in calendarEvents)
                {
                    if (!calendarEvent.Start.HasValue || !calendarEvent.End.HasValue) continue;

                    var viewmodel = new CalendarDayEventViewModel();
                    viewmodel.Event = calendarEvent;
                    CalendarEvents.Add(viewmodel);            
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
