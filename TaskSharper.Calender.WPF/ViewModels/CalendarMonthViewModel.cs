using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using Prism.Regions;
using Serilog;
using TaskSharper.Calender.WPF.Events;
using TaskSharper.Calender.WPF.ViewModels.MonthViewModels;
using TaskSharper.Domain.BusinessLayer;

namespace TaskSharper.Calender.WPF.ViewModels
{
    public class CalendarMonthViewModel : BindableBase
    {
        private const int DaysInWeek = 7;
        private DateTime PreviousMonday;

        private readonly IEventManager _eventManager;
        private readonly IEventAggregator _eventAggregator;
        private readonly IRegionManager _regionManager;
        private readonly ILogger _logger;

        public ObservableCollection<CalendarDateDayViewModel> DateDays { get; set; }
        public ObservableCollection<CalendarWeekNumberViewModel> WeekNumbers { get; set; }
        public ObservableCollection<CalendarWeekDayViewModel> WeekDays { get; set; }

        public DateTime CurrentDatetime { get; set; }

        public DelegateCommand NextCommand { get; set; }
        public DelegateCommand PrevCommand { get; set; }

        public CalendarMonthViewModel(IEventManager eventManager, IEventAggregator eventAggregator, ILogger logger, IRegionManager regionManager)
        {
            // Initialize objects
            _eventManager = eventManager;
            _eventAggregator = eventAggregator;
            _regionManager = regionManager;
            _logger = logger.ForContext<CalendarMonthViewModel>();

            // Create Event commands
            NextCommand = new DelegateCommand(NextMonth);
            PrevCommand = new DelegateCommand(PrevMonth);

            // Initialize view containers
            DateDays = new ObservableCollection<CalendarDateDayViewModel>();
            WeekNumbers = new ObservableCollection<CalendarWeekNumberViewModel>();
            WeekDays = new ObservableCollection<CalendarWeekDayViewModel>();

            CurrentDatetime = DateTime.Now;

            // Create view
            BootstrapView();
        }

        private void PrevMonth()
        {
            CurrentDatetime = CurrentDatetime.AddMonths(-1); 
            UpdateDates();
        }

        private void NextMonth()
        {
            CurrentDatetime = CurrentDatetime.AddMonths(1);
            UpdateDates();
        }

        private void BootstrapView()
        {
            for (int i = 1; i <= DaysInWeek; i++)
            {
                WeekDays.Add(new CalendarWeekDayViewModel(i));
            }
            var firstDayOfMonth = CalculateDate(1, CurrentDatetime);
            FindPreviousMonday(firstDayOfMonth);

            for (int i =  0; i < 35; i++)
            {
                var prevMonday = PreviousMonday.AddDays(i);

                if (i % 7 == 0)
                {
                    WeekNumbers.Add(new CalendarWeekNumberViewModel(prevMonday));
                }

                DateDays.Add(new CalendarDateDayViewModel(prevMonday, _eventAggregator, _eventManager, CalendarTypeEnum.Month, _logger, _regionManager));
            }
        }


        private void FindPreviousMonday(DateTime firstDayOfMonth)
        {
            if(firstDayOfMonth.DayOfWeek == DayOfWeek.Monday)
            {
                PreviousMonday = firstDayOfMonth;
                return;
            }
            FindPreviousMonday(firstDayOfMonth.AddDays(-1));

        }
        
        private DateTime CalculateDate(int day, DateTime currentDateTime)
        {
            var dayOffset = day - (int)currentDateTime.Day;

            var dateTime = currentDateTime.AddDays(dayOffset);

            return dateTime;
        }

        public void UpdateDates()
        {
            var firstDayOfMonth = CalculateDate(1, CurrentDatetime);
            FindPreviousMonday(firstDayOfMonth);

            int weekCount = 0;
            for (int i = 0; i < 35; i++)
            {
                var prevMonday = PreviousMonday.AddDays(i);

                if (i % 7 == 0)
                    WeekNumbers[weekCount++].Date = prevMonday;
                    
                DateDays[i].UpdateDate(prevMonday);
            }
        }

    }
}
