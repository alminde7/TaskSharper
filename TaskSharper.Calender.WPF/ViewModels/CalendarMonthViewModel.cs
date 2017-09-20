using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using TaskSharper.Calender.WPF.ViewModels.MonthViewModels;
using TaskSharper.Domain.BusinessLayer;
using TaskSharper.Domain.Calendar;

namespace TaskSharper.Calender.WPF.ViewModels
{
    public class CalendarMonthViewModel : BindableBase
    {
        private const int DaysInWeek = 7;
        private  DateTime DateNow = DateTime.Now;

        private readonly IEventManager _eventManager;
        private IEventAggregator _eventAggregator;

        public ObservableCollection<CalendarDateDayViewModel> DateDays { get; set; }
        public ObservableCollection<CalendarWeekNumberViewModel> WeekNumbers { get; set; }

        public ObservableCollection<CalendarWeekDayViewModel> WeekDays { get; set; }

        public DateTime CurrentWeek { get; set; }

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

            CurrentWeek = DateTime.Now;

            InitializeViews();
        }

        private void PrevMonth()
        {
            throw new NotImplementedException();
        }

        private void NextMonth()
        {
            throw new NotImplementedException();
        }

        private void InitializeViews()
        {
            for (int i = 1; i <= DaysInWeek; i++)
            {
                var date = CalculateDate(i);
                WeekDays.Add(new CalendarWeekDayViewModel(date));
               
            }
            for (int i = 1; i < DateTime.DaysInMonth(DateNow.Year, DateNow.Month) + 1; i++)
            {
                var date = CalculateDate(i);
                DateDays.Add(new CalendarDateDayViewModel(date, _eventAggregator, _eventManager));
            }
        }
        private DateTime CalculateDate(int day)
        {
            var dayOffset = day - (int)DateTime.Now.Day;

            var dateTime = DateTime.Now.AddDays(dayOffset);

            return dateTime;
        }

    }
}
