using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using MoreLinq;
using Prism.Regions;
using Serilog;
using TaskSharper.Calender.WPF.Config;
using TaskSharper.Calender.WPF.ViewModels.MonthViewModels;
using TaskSharper.Domain.Calendar;
using TaskSharper.Shared.Exceptions;
using TaskSharper.Shared.Extensions;
using TaskSharper.WPF.Common.Events;
using TaskSharper.WPF.Common.Events.NotificationEvents;
using TaskSharper.WPF.Common.Events.Resources;

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
        public DelegateCommand CreateEventCommand { get; set; }
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
            CreateEventCommand = new DelegateCommand(NavigateToCreateEventView);

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

        private void NavigateToCreateEventView()
        {
            var navigationParameters = new NavigationParameters();
            navigationParameters.Add("Type", EventType.None);
            navigationParameters.Add("Region", ViewConstants.REGION_Calendar);
            _regionManager.RequestNavigate(ViewConstants.REGION_Calendar, ViewConstants.VIEW_CalendarEventDetails, navigationParameters);
        }

        private void SetMonthAndYearCulture()
        {
            CurrentMonthAndYear = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(CurrentDatetime.ToString("yyyy MMMM", CultureInfo.CurrentCulture));
        }

        #region ClickHandlers
        private async void PrevMonth()
        {
            CurrentDatetime = CurrentDatetime.AddMonths(-1);
            SetMonthAndYearCulture();
            UpdateDates();
            await UpdateViewsWithData();
        }

        private async void NextMonth()
        {
            CurrentDatetime = CurrentDatetime.AddMonths(1);
            SetMonthAndYearCulture();
            UpdateDates();
            await UpdateViewsWithData();
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
            if (@events != null)
            {
                ApplicationStatus.InternetConnection = true;

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
        }

        private async Task<IDictionary<DateTime, IList<Event>>> GetEvents(DateTime start, DateTime end)
        {
            try
            {
                var days = new Dictionary<DateTime, IList<Event>>();
                var monthEvents = await _dataService.GetAsync(start, end);
                if (monthEvents == null) return days;

                monthEvents = monthEvents.OrderBy(o => o.Start).ThenBy(o => o.End);
                

                var uniqueEvents = monthEvents.DistinctBy(x => x.Id).ToList();

                foreach (var weekEvent in uniqueEvents)
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
            catch (ConnectionException)
            {
                _eventAggregator.GetEvent<NotificationEvent>().Publish(new ConnectionErrorNotification());
                return null;
            }
            catch (UnauthorizedAccessException)
            {
                _eventAggregator.GetEvent<NotificationEvent>().Publish(new UnauthorizedErrorNotification());
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public async void OnNavigatedTo(NavigationContext navigationContext)
        {
            await UpdateViewsWithData();
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
