using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Web;
using Prism.Events;
using Prism.Mvvm;
using TaskSharper.Calender.WPF.Events;
using TaskSharper.Calender.WPF.Events.Resources;
using Prism.Commands;
using Prism.Regions;
using Serilog;
using TaskSharper.Calender.WPF.Config;
using TaskSharper.Calender.WPF.Events.NotificationEvents;
using TaskSharper.Domain.Calendar;
using TaskSharper.Shared.Exceptions;
using TaskSharper.Shared.Extensions;

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
        public ObservableCollection<CalendarEventsViewModel> EventContainers { get; set; }
        public CalendarYearHeaderViewModel DateYearHeader { get; set; }
        public DateTime CurrentWeek { get; set; }

        public DelegateCommand NextCommand { get; set; }
        public DelegateCommand PrevCommand { get; set; }


        public CalendarWeekViewModel(IEventRestClient dataService, IEventAggregator eventAggregator, IRegionManager regionManager, ILogger logger)
        {
            _dataService = dataService;
            _eventAggregator = eventAggregator;
            _regionManager = regionManager;
            _logger = logger.ForContext<CalendarWeekViewModel>();

            NextCommand = new DelegateCommand(NextWeek);
            PrevCommand = new DelegateCommand(PreviousWeek);

            DateHeaders = new ObservableCollection<CalendarDateViewModel>();
            EventContainers = new ObservableCollection<CalendarEventsViewModel>();
            DateYearHeader = new CalendarYearHeaderViewModel(_eventAggregator, CalendarTypeEnum.Week, _logger);

            CurrentWeek = DateTime.Now;
            InitializeViews();
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
                    _dataService, CalendarTypeEnum.Week, _logger, new List<Event>()));
                DateHeaders.Add(new CalendarDateViewModel(date, _eventAggregator, CalendarTypeEnum.Week, _logger));
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
            if (@events != null)
            {
                ApplicationStatus.InternetConnection = true;

                foreach (var eventContainer in EventContainers)
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

        private async Task<IDictionary<DateTime, IList<Event>>> GetEvents(DateTime week)
        {
            var start = week.StartOfWeek().StartOfDay();
            var end = week.EndOfWeek().EndOfDay();

            try
            {
                var weekEvents = await _dataService.GetAsync(start, end);

                var days = new Dictionary<DateTime, IList<Event>>();

                if (weekEvents == null) return days;

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
                                days.Add(date.AddDays(i), new List<Event>() {weekEvent});
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
                            days.Add(date, new List<Event>() {weekEvent});
                        }
                    }
                }

                return days;
            }
            catch (ConnectionException e)
            {
                _eventAggregator.GetEvent<NotificationEvent>().Publish(new ConnectionErrorNotification());
                return null;
            }
            catch (ArgumentException e)
            {
                // Client error exception
                return null;
            }
            catch (HttpException e)
            {
                // Internal server error
                return null;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
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