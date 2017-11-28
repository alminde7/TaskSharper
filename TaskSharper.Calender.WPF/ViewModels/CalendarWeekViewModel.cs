using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using MoreLinq;
using Prism.Events;
using Prism.Mvvm;
using Prism.Commands;
using Prism.Regions;
using Serilog;
using TaskSharper.Calender.WPF.Config;
using TaskSharper.Domain.Calendar;
using TaskSharper.Shared.Exceptions;
using TaskSharper.Shared.Extensions;
using TaskSharper.WPF.Common.Events;
using TaskSharper.WPF.Common.Events.DateChangedEvents;
using TaskSharper.WPF.Common.Events.NotificationEvents;
using TaskSharper.WPF.Common.Events.Resources;

namespace TaskSharper.Calender.WPF.ViewModels
{
    public class CalendarWeekViewModel : BindableBase, INavigationAware
    {
        private const int DaysInWeek = 7;

        private readonly IEventRestClient _dataService;
        private readonly IEventAggregator _eventAggregator;
        private readonly IRegionManager _regionManager;
        private readonly ILogger _logger;

        public ObservableCollection<CalendarDateViewModel> DateHeaders { get; set; }
        public ObservableCollection<CalendarAllDayEventContainerViewModel> AllDayEventContainers { get; set; }
        public ObservableCollection<CalendarEventsViewModel> EventContainers { get; set; }
        public CalendarYearHeaderViewModel DateYearHeader { get; set; }
        public DateTime CurrentWeek { get; set; }

        public DelegateCommand NextCommand { get; set; }
        public DelegateCommand PrevCommand { get; set; }
        public DelegateCommand CreateEventCommand { get; set; }


        public CalendarWeekViewModel(IEventRestClient dataService, IEventAggregator eventAggregator, IRegionManager regionManager, ILogger logger)
        {
            _dataService = dataService;
            _eventAggregator = eventAggregator;
            _regionManager = regionManager;
            _logger = logger.ForContext<CalendarWeekViewModel>();

            NextCommand = new DelegateCommand(NextWeek);
            PrevCommand = new DelegateCommand(PreviousWeek);
            CreateEventCommand = new DelegateCommand(NavigateToCreateEventView);

            DateHeaders = new ObservableCollection<CalendarDateViewModel>();
            AllDayEventContainers = new ObservableCollection<CalendarAllDayEventContainerViewModel>();
            EventContainers = new ObservableCollection<CalendarEventsViewModel>();
            DateYearHeader = new CalendarYearHeaderViewModel(_eventAggregator, CalendarTypeEnum.Week, _logger);

            CurrentWeek = DateTime.Now;
            InitializeViews();
        }

        private void NavigateToCreateEventView()
        {
            var navigationParameters = new NavigationParameters();
            navigationParameters.Add("Type", EventType.None);
            navigationParameters.Add("Region", ViewConstants.REGION_Calendar);
            _regionManager.RequestNavigate(ViewConstants.REGION_Calendar, ViewConstants.VIEW_CalendarEventDetails, navigationParameters);
        }

        #region Commands

        private async void NextWeek()
        {
            CurrentWeek = CurrentWeek.Date.AddDays(7);
            _eventAggregator.GetEvent<WeekChangedEvent>().Publish(DateChangedEnum.Increase);
            await UpdateViews();
            _logger.ForContext("Click", typeof(WeekChangedEvent)).Information("NextWeek has been clicked");
        }

        private async void PreviousWeek()
        {
            CurrentWeek = CurrentWeek.Date.AddDays(-7);
            _eventAggregator.GetEvent<WeekChangedEvent>().Publish(DateChangedEnum.Decrease);
            await UpdateViews();
            _logger.ForContext("Click", typeof(WeekChangedEvent)).Information("PreviousWeek has been clicked");
        }

        #endregion

        #region Bootstrap Views

        private void InitializeViews()
        {
            for (int i = 1; i <= DaysInWeek; i++)
            {
                var date = CalculateDate(i);

                EventContainers.Add(new CalendarEventsViewModel(date, _eventAggregator, _regionManager,
                    _dataService, CalendarTypeEnum.Week, _logger));
                DateHeaders.Add(new CalendarDateViewModel(date, _eventAggregator, CalendarTypeEnum.Week, _logger));
                AllDayEventContainers.Add(new CalendarAllDayEventContainerViewModel(date, _regionManager, _eventAggregator, _logger));
            }
        }

        private DateTime CalculateDate(int day)
        {
            var dayOffset = day - (int) DateTime.Now.DayOfWeek;

            var dateTime = DateTime.Now.AddDays(dayOffset);

            return dateTime;
        }

        #endregion


        private async Task UpdateViews()
        {
            var @events = await GetEvents(CurrentWeek);
            if (@events.normalEvents != null)
            {
                ApplicationStatus.InternetConnection = true;

                foreach (var eventContainer in EventContainers)
                {
                    var date = eventContainer.Date.StartOfDay();
                    if (@events.normalEvents.ContainsKey(date))
                    {
                        eventContainer.UpdateView(@events.normalEvents[date]);
                    }
                    else
                    {
                        eventContainer.UpdateView();
                    }
                }
            }
            if (@events.allDayEvents != null)
            {
                ApplicationStatus.InternetConnection = true;

                foreach (var allDayEventContainer in AllDayEventContainers)
                {
                    var date = allDayEventContainer.Date.StartOfDay();
                    if (@events.allDayEvents.ContainsKey(date))
                    {
                        allDayEventContainer.SetAllDayEvents(@events.allDayEvents[date].ToList());
                    }
                    else
                    {
                        allDayEventContainer.SetAllDayEvents();
                    }
                }
            }
        }

        private async Task<(IDictionary<DateTime, IList<Event>> normalEvents, IDictionary<DateTime, IList<Event>> allDayEvents)> GetEvents(DateTime week)
        {
            var start = week.StartOfWeek().StartOfDay();
            var end = week.EndOfWeek().EndOfDay();

            try
            {
                var days = new Dictionary<DateTime, IList<Event>>();
                var allDayEventDays = new Dictionary<DateTime, IList<Event>>();
                var weekEvents = await _dataService.GetAsync(start, end);
                if (weekEvents == null) return (days, allDayEventDays);

                var uniqueEvents = weekEvents.DistinctBy(x => x.Id).ToList();

                foreach (var weekEvent in uniqueEvents)
                {
                    var date = weekEvent.Start.Value.Date.StartOfDay();

                    var diff = (weekEvent.End.Value.Date - weekEvent.Start.Value.Date).Days;

                    for (int i = 0; i <= diff; i++)
                    {
                        if (weekEvent.AllDayEvent.HasValue)
                        {
                            if (allDayEventDays.ContainsKey(date.AddDays(i)))
                            {
                                allDayEventDays[weekEvent.Start.Value.StartOfDay()].Add(weekEvent);
                            }
                            else
                            {
                                allDayEventDays.Add(date.AddDays(i), new List<Event>() {weekEvent});
                            }
                        }
                        else
                        {
                            if (days.ContainsKey(date.AddDays(i)))
                            {
                                days[weekEvent.Start.Value.StartOfDay()].Add(weekEvent);
                            }
                            else
                            {
                                days.Add(date.AddDays(i), new List<Event>() {weekEvent});
                            }
                        }

                    }
                }

                return (days, allDayEventDays);
            }
            catch (ConnectionException)
            {
                _eventAggregator.GetEvent<NotificationEvent>().Publish(new ConnectionErrorNotification());
                return (null, null);
            }
            catch (UnauthorizedAccessException)
            {
                _eventAggregator.GetEvent<NotificationEvent>().Publish(new UnauthorizedErrorNotification());
                return (null, null);
            }
            catch (Exception)
            {
                return (null, null);
            }
        }

        #region INavigationAware implementation

        public async void OnNavigatedTo(NavigationContext navigationContext)
        {
            _eventAggregator.GetEvent<ScrollButtonsEvent>().Publish(EventResources.ScrollButtonsEnum.Show);
            await UpdateViews();
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            _eventAggregator.GetEvent<ScrollButtonsEvent>().Publish(EventResources.ScrollButtonsEnum.Hide);
        }

        #endregion
    }
}