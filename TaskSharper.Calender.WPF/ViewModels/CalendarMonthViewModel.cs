using Microsoft.Practices.ObjectBuilder2;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using TaskSharper.Calender.WPF.Events;
using TaskSharper.Calender.WPF.Events.Resources;
using TaskSharper.Calender.WPF.ViewModels.MonthViewModels;
using TaskSharper.Domain.BusinessLayer;
using TaskSharper.Domain.Calendar;

namespace TaskSharper.Calender.WPF.ViewModels
{
    public class CalendarMonthViewModel : BindableBase
    {
        private const int DaysInWeek = 7;
        private DateTime DateNow = DateTime.Now;
        private DateTime PrevFriday;

        private readonly IEventManager _eventManager;
        private IEventAggregator _eventAggregator;

        public ObservableCollection<CalendarDateDayViewModel> DateDays { get; set; }
        public ObservableCollection<CalendarWeekNumberViewModel> WeekNumbers { get; set; }

        public ObservableCollection<CalendarWeekDayViewModel> WeekDays { get; set; }

        public DateTime CurrentDatetime { get; set; }

        public DelegateCommand NextCommand { get; set; }
        public DelegateCommand PrevCommand { get; set; }

        public CalendarMonthViewModel(IEventManager eventManager, IEventAggregator eventAggregator)
        {
            _eventManager = eventManager;
            _eventAggregator = eventAggregator;

            NextCommand = new DelegateCommand(NextMonth);
            PrevCommand = new DelegateCommand(PrevMonth);

            DateDays = new ObservableCollection<CalendarDateDayViewModel>();
            WeekNumbers = new ObservableCollection<CalendarWeekNumberViewModel>();
            WeekDays = new ObservableCollection<CalendarWeekDayViewModel>();

            CurrentDatetime = DateTime.Now;

            InitializeViews();
        }

        private void PrevMonth()
        {
            CurrentDatetime = CurrentDatetime.AddMonths(-1); 
            _eventAggregator.GetEvent<DateChangedEvent>().Publish(DateChangeEnum.DecreaseMonth);
        }

        private void NextMonth()
        {
            CurrentDatetime = CurrentDatetime.AddMonths(1);
            _eventAggregator.GetEvent<DateChangedEvent>().Publish(DateChangeEnum.IncreaseMonth);
        }

        private void InitializeViews()
        {
            for (int i = 1; i <= 7; i++)
            {
                WeekDays.Add(new CalendarWeekDayViewModel(i));
            }
            var nextFriday = CalculateDate(1, CurrentDatetime);
            FindMonday(nextFriday);

            for (int i =  0; i < 35; i++)
            {
                var PrevMonday = PrevFriday.AddDays(i);
                DateDays.Add(new CalendarDateDayViewModel(PrevMonday, _eventAggregator, _eventManager));
            }
        }
        private void UpdateViews()
        {
            var nextFriday = CalculateDate(1, CurrentDatetime);
            
        }
        private void FindMonday(DateTime nextmonth)
        {
            if(nextmonth.DayOfWeek == DayOfWeek.Monday)
            {
                PrevFriday = nextmonth;
                return;
            }
            FindMonday(nextmonth.AddDays(-1));

        }
        
        private DateTime CalculateDate(int day, DateTime currentDateTime)
        {
            var dayOffset = day - (int)currentDateTime.Day;

            var dateTime = currentDateTime.AddDays(dayOffset);

            return dateTime;
        }

    }
}
