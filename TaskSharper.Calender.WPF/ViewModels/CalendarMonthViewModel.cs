﻿using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Prism.Regions;
using Serilog;
using TaskSharper.Calender.WPF.Events;
using TaskSharper.Calender.WPF.Events.Resources;
using TaskSharper.Calender.WPF.ViewModels.MonthViewModels;
using TaskSharper.Domain.Calendar;
using TaskSharper.Shared.Extensions;

namespace TaskSharper.Calender.WPF.ViewModels
{
    public class CalendarMonthViewModel : BindableBase, INavigationAware
    {
        private const int DaysInWeek = 7;
        private DateTime _previousMonday;
        private string _currentMonthAndYear;
        private DateTime _currentDate;
        private int _numberOfWeeks;
        private readonly IEventRestClient _dataService;
        private readonly IEventAggregator _eventAggregator;
        private readonly IRegionManager _regionManager;
        private readonly ILogger _logger;
        

        public ObservableCollection<CalendarDateDayViewModel> DateDays { get; set; }
        public ObservableCollection<CalendarWeekNumberViewModel> WeekNumbers { get; set; }
        public ObservableCollection<CalendarWeekDayViewModel> WeekDays { get; set; }

        public DelegateCommand NextCommand { get; set; }
        public DelegateCommand PrevCommand { get; set; }
        public CultureInfo CurrentCulture { get; set; }

        public DateTime CurrentDatetime
        {
            get => _currentDate;
            set => SetProperty(ref _currentDate, value);
        }
        public string CurrentMonthAndYear
        {
            get => _currentMonthAndYear;
            set => SetProperty(ref _currentMonthAndYear, value);
        }

        public int NumberOfWeeks
        {
            get => _numberOfWeeks;
            set => SetProperty(ref _numberOfWeeks, value);
        }
        
        public CalendarMonthViewModel(IEventRestClient dataService, IEventAggregator eventAggregator, ILogger logger, IRegionManager regionManager)
        {
            // Initialize objects
            _dataService = dataService;
            _eventAggregator = eventAggregator;
            _regionManager = regionManager;
            _logger = logger.ForContext<CalendarMonthViewModel>();

            // Create Event commands
            NextCommand = new DelegateCommand(NextMonth);
            PrevCommand = new DelegateCommand(PrevMonth);

            //Event Subscriptions 
            eventAggregator.GetEvent<CultureChangedEvent>().Subscribe(()=> UpdateCulture(CurrentDatetime));

            // Initialize view containers
            DateDays = new ObservableCollection<CalendarDateDayViewModel>();
            WeekNumbers = new ObservableCollection<CalendarWeekNumberViewModel>();
            WeekDays = new ObservableCollection<CalendarWeekDayViewModel>();

            CurrentDatetime = DateTime.Now;
            SetMonthAndYearCulture();

            // Create view
            BootstrapView();
        }

        private void SetMonthAndYearCulture()
        {
            CurrentMonthAndYear = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(CurrentDatetime.ToString("yyyy MMMM", CultureInfo.CurrentCulture));
        }

        #region ClickHandlers
        private async void PrevMonth()
        {
            _eventAggregator.GetEvent<SpinnerEvent>().Publish(EventResources.SpinnerEnum.Show);
            CurrentDatetime = CurrentDatetime.AddMonths(-1);
            SetMonthAndYearCulture();
            UpdateDates();
            await UpdateViewsWithData();
            _eventAggregator.GetEvent<SpinnerEvent>().Publish(EventResources.SpinnerEnum.Hide);
        }

        private async void NextMonth()
        {
            _eventAggregator.GetEvent<SpinnerEvent>().Publish(EventResources.SpinnerEnum.Show);
            CurrentDatetime = CurrentDatetime.AddMonths(1);
            SetMonthAndYearCulture();
            UpdateDates();
            await UpdateViewsWithData();
            _eventAggregator.GetEvent<SpinnerEvent>().Publish(EventResources.SpinnerEnum.Hide);
        }
        #endregion

        private void UpdateCulture(DateTime date)
        {
            CurrentCulture = CultureInfo.CurrentCulture;
            CurrentDatetime = date;
            SetMonthAndYearCulture();
        }

        private void UpdateIsWithinSelectedMonth()
        {
            foreach (var day in DateDays)
            {
                day.IsWithinSelectedMonth = day.Date.Month == CurrentDatetime.Month;
            }
        }

        private void BootstrapView()
        {
            for (int i = 1; i <= DaysInWeek; i++)
            {
                WeekDays.Add(new CalendarWeekDayViewModel(i, _eventAggregator));
            }
            var firstDayOfMonth = CalculateDate(1, CurrentDatetime);
            FindPreviousMonday(firstDayOfMonth);

            for (int i =  0; i < 42; i++)
            {
                var prevMonday = _previousMonday.AddDays(i);

                if (i % 7 == 0)
                {
                    WeekNumbers.Add(new CalendarWeekNumberViewModel(prevMonday));
                }

                DateDays.Add(new CalendarDateDayViewModel(prevMonday, _eventAggregator, _dataService, CalendarTypeEnum.Month, _logger, _regionManager));

                if (i < 34) continue;
                if (DateTime.DaysInMonth(CurrentDatetime.Year, CurrentDatetime.Month) == 31 && (firstDayOfMonth.DayOfWeek == DayOfWeek.Saturday || firstDayOfMonth.DayOfWeek == DayOfWeek.Sunday))
                {
                    continue;
                }
                if (DateTime.DaysInMonth(CurrentDatetime.Year, CurrentDatetime.Month) == 30 && firstDayOfMonth.DayOfWeek == DayOfWeek.Sunday)
                {
                    continue;
                }
                break;
            }

            NumberOfWeeks = WeekNumbers.Count;
            UpdateIsWithinSelectedMonth();
        }

        private void FindPreviousMonday(DateTime firstDayOfMonth)
        {
            if(firstDayOfMonth.DayOfWeek == DayOfWeek.Monday)
            {
                _previousMonday = firstDayOfMonth;
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
            WeekNumbers.Clear();
            for (int i = 0; i < 42; i++)
            {
                var prevMonday = _previousMonday.AddDays(i);

                if (i % 7 == 0)
                {
                    weekCount++;
                    WeekNumbers.Add(new CalendarWeekNumberViewModel(prevMonday));
                }
                    
                DateDays[i].UpdateDate(prevMonday);

                if (i < 34) continue;
                if (DateTime.DaysInMonth(CurrentDatetime.Year, CurrentDatetime.Month) == 31 && (firstDayOfMonth.DayOfWeek == DayOfWeek.Saturday || firstDayOfMonth.DayOfWeek == DayOfWeek.Sunday))
                {
                    if (i < 41)
                        DateDays.Add(new CalendarDateDayViewModel(prevMonday, _eventAggregator, _dataService, CalendarTypeEnum.Month, _logger, _regionManager));
                    continue;
                }
                if (DateTime.DaysInMonth(CurrentDatetime.Year, CurrentDatetime.Month) == 30 && firstDayOfMonth.DayOfWeek == DayOfWeek.Sunday)
                {
                    if (i < 41)
                        DateDays.Add(new CalendarDateDayViewModel(prevMonday, _eventAggregator, _dataService, CalendarTypeEnum.Month, _logger, _regionManager));
                    continue;
                }
                if (DateDays.Count >= 42)
                {
                    for (int j = DateDays.Count - 1; j > i; j--)
                    {
                        DateDays.RemoveAt(j);
                    }
                }
                break;
            }

            NumberOfWeeks = WeekNumbers.Count;
            UpdateIsWithinSelectedMonth();
        }

        private async Task UpdateViewsWithData()
        {
            var @events = await GetEvents(DateDays.First().Date, DateDays.Last().Date);
            foreach (var eventContainer in DateDays)
            {
                var date = eventContainer.Date.StartOfDay();
                if (@events.ContainsKey(date))
                {
                    eventContainer.UpdateView(@events[date]);
                }
                else
                {
                    eventContainer.UpdateView();
                }
            }
        }

        private async Task<IDictionary<DateTime, IList<Event>>> GetEvents(DateTime start, DateTime end)
        {
            var startdate = start.StartOfDay();
            var enddate = end.EndOfDay();

            var weekEvents = await _dataService.GetAsync(start, end);

            var days = new Dictionary<DateTime, IList<Event>>();

            foreach (var weekEvent in weekEvents)
            {
                var date = weekEvent.Start.Value.Date.StartOfDay();

                var diff = (weekEvent.End.Value.Date - weekEvent.Start.Value.Date).Days;

                if (diff > 0)
                {
                    for (int i = 0; i < diff; i++)
                    {
                        if (days.ContainsKey(date.AddDays(i)))
                        {
                            days[weekEvent.Start.Value.StartOfDay()].Add(weekEvent);
                        }
                        else
                        {
                            days.Add(date.AddDays(i), new List<Event>() { weekEvent });
                        }
                    }
                }
                else
                {
                    if (days.ContainsKey(date))
                    {
                        days[weekEvent.Start.Value.StartOfDay()].Add(weekEvent);
                    }
                    else
                    {
                        days.Add(date, new List<Event>() { weekEvent });
                    }
                }
            }

            return days;
        }

        public async void OnNavigatedTo(NavigationContext navigationContext)
        {
            await UpdateViewsWithData();
            _eventAggregator.GetEvent<SpinnerEvent>().Publish(EventResources.SpinnerEnum.Hide);
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }
    }
}
