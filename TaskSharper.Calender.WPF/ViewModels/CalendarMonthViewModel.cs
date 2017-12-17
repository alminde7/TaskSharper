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
using TaskSharper.Domain.Models;
using TaskSharper.Domain.RestClient;
using TaskSharper.Shared.Exceptions;
using TaskSharper.Shared.Extensions;
using TaskSharper.WPF.Common.Events;
using TaskSharper.WPF.Common.Events.NotificationEvents;
using TaskSharper.WPF.Common.Events.Resources;

namespace TaskSharper.Calender.WPF.ViewModels
{
    /// <summary>
    /// Is the ViewModel for the CalendarMonthView.
    /// 
    /// It purpose is to showcase the structure of a normal looking calendar. 
    /// </summary>
    public class CalendarMonthViewModel : BindableBase, INavigationAware
    {
        private const int DaysInWeek = 7;
        private DateTime _previousMonday;
        private string _currentMonthAndYear;
        private DateTime _currentDate;
        private int _numberOfWeeks;
        private readonly IEventRestClient _eventRestClient;
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

        /// <summary>
        /// Constructor 
        /// 
        /// Contains list of the following ViewModels CalendarDateDayViewModel, CalendarWeekNumberViewModel
        /// CalendarWeekDayViewModel.
        /// 
        /// It is also subscribes to the event of culture change.
        /// </summary>
        /// <param name="eventRestClient">Dependency injection of eventRestClient</param>
        /// <param name="eventAggregator">Dependency injection of the eventManager</param>
        /// <param name="logger">Dependency injection of the logger</param>
        /// <param name="regionManager">Dependency injection of the regionManager</param>
        public CalendarMonthViewModel(IEventRestClient eventRestClient, IEventAggregator eventAggregator, ILogger logger, IRegionManager regionManager)
        {
            // Initialize objects
            _eventRestClient = eventRestClient;
            _eventAggregator = eventAggregator;
            _regionManager = regionManager;
            _logger = logger.ForContext<CalendarMonthViewModel>();

            // Create Event commands
            NextCommand = new DelegateCommand(NextMonth);
            PrevCommand = new DelegateCommand(PrevMonth);
            CreateEventCommand = new DelegateCommand(NavigateToCreateEventView);

            // Event Subscriptions
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

        /// <summary>
        /// Navigates to the create event view.
        /// </summary>
        private void NavigateToCreateEventView()
        {
            var navigationParameters = new NavigationParameters();
            navigationParameters.Add("Type", EventType.None);
            navigationParameters.Add("Region", ViewConstants.REGION_Calendar);
            _regionManager.RequestNavigate(ViewConstants.REGION_Calendar, ViewConstants.VIEW_CalendarEventDetails, navigationParameters);
        }

        /// <summary>
        /// Sets the culture for the Month and Year property by the current culture 
        /// </summary>
        private void SetMonthAndYearCulture()
        {
            CurrentMonthAndYear = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(CurrentDatetime.ToString("yyyy MMMM", CultureInfo.CurrentCulture));
        }

        #region ClickHandlers

        /// <summary>
        /// When the Prev button is pressed the delegate command will call this function
        /// </summary>
        private async void PrevMonth()
        {
            CurrentDatetime = CurrentDatetime.AddMonths(-1);
            SetMonthAndYearCulture();
            UpdateDates();
            await UpdateViewsWithData();
        }

        /// <summary>
        /// When the Prev button is pressed the delegate command will call this function
        /// </summary>
        private async void NextMonth()
        {
            CurrentDatetime = CurrentDatetime.AddMonths(1);
            SetMonthAndYearCulture();
            UpdateDates();
            await UpdateViewsWithData();
        }
        #endregion

        /// <summary>
        /// Updates the current culture and call properties that need refreshing.  
        /// </summary>
        /// <param name="date"></param>
        private void UpdateCulture(DateTime date)
        {
            CurrentCulture = CultureInfo.CurrentCulture;
            CurrentDatetime = date;
            SetMonthAndYearCulture();
        }

        /// <summary>
        /// Sets the property in CalendarDateDayViewModel depending on the current month in the CalendarMonthViewModel.
        /// This is done, so it possible to grey out elements that is not part of the current month.
        /// </summary>
        private void UpdateIsWithinSelectedMonth()
        {
            foreach (var day in DateDays)
            {
                day.IsWithinSelectedMonth = day.Date.Month == CurrentDatetime.Month;
            }
        }

        /// <summary>
        /// The BootstrapView method is a setup method that is called from the constructor
        /// It instantiate the CalendarWeekDayViewModels for every day in the week. 
        /// It also instantiate the 42 month elements, these elements are the CalendarDateDayViewModel's.  
        /// </summary>
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

                if (i % DaysInWeek == 0)
                {
                    WeekNumbers.Add(new CalendarWeekNumberViewModel(prevMonday));
                }

                DateDays.Add(new CalendarDateDayViewModel(prevMonday, _eventAggregator, _eventRestClient, CalendarTypeEnum.Month, _logger, _regionManager));

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

        /// <summary>
        /// Search method that finds the previous monday, this is used to fix the 42 elements in the view,
        /// so it is always monday that is the first element.
        /// </summary>
        /// <param name="firstDayOfMonth"></param>
        private void FindPreviousMonday(DateTime firstDayOfMonth)
        {
            if(firstDayOfMonth.DayOfWeek == DayOfWeek.Monday)
            {
                _previousMonday = firstDayOfMonth;
                return;
            }
            FindPreviousMonday(firstDayOfMonth.AddDays(-1));
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="day"></param>
        /// <param name="currentDateTime"></param>
        /// <returns></returns>
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
                        DateDays.Add(new CalendarDateDayViewModel(prevMonday, _eventAggregator, _eventRestClient, CalendarTypeEnum.Month, _logger, _regionManager));
                    continue;
                }
                if (DateTime.DaysInMonth(CurrentDatetime.Year, CurrentDatetime.Month) == 30 && firstDayOfMonth.DayOfWeek == DayOfWeek.Sunday)
                {
                    if (i < 41)
                        DateDays.Add(new CalendarDateDayViewModel(prevMonday, _eventAggregator, _eventRestClient, CalendarTypeEnum.Month, _logger, _regionManager));
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
                var monthEvents = await _eventRestClient.GetAsync(start, end);
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
