using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Serilog;
using TaskSharper.Calender.WPF.Events;
using TaskSharper.Calender.WPF.Events.Resources;
using TaskSharper.Domain.Calendar;

namespace TaskSharper.Calender.WPF.ViewModels
{
    public class CalendarDayViewModel : BindableBase, INavigationAware
    {
        private readonly IRegionManager _regionManager;
        public IEventAggregator EventAggregator { get; }
        public IEventRestClient DataService { get; }
        public ILogger Logger { get; }

        public CalendarEventsViewModel EventsViewModel { get; set; }
        public CalendarDateViewModel DateViewModel { get; set; }
        public CalendarYearHeaderViewModel DateYearHeader { get; set; }
        public DateTime CurrentDay { get; set; }

        public ICommand NextCommand { get; set; }
        public ICommand PrevCommand { get; set; }

        public CalendarDayViewModel(IEventAggregator eventAggregator, IEventRestClient dataService,
            IRegionManager regionManager, ILogger logger)
        {
            _regionManager = regionManager;
            EventAggregator = eventAggregator;
            DataService = dataService;
            Logger = logger.ForContext<CalendarDayViewModel>();
            CurrentDay = DateTime.Now;

            // Initialize views
            EventsViewModel = new CalendarEventsViewModel(CurrentDay, eventAggregator, _regionManager, dataService,
                CalendarTypeEnum.Day, Logger, new List<Event>());
            DateViewModel = new CalendarDateViewModel(CurrentDay, eventAggregator, CalendarTypeEnum.Day, Logger);
            DateYearHeader = new CalendarYearHeaderViewModel(EventAggregator, CalendarTypeEnum.Day, Logger);

            // Initialize commands
            NextCommand = new DelegateCommand(NextDayCommandHandler);
            PrevCommand = new DelegateCommand(PreviousDayCommandHandler);
        }

        #region EventHandlers

        public void NextDayCommandHandler()
        {
            CurrentDay = CurrentDay.AddDays(1);
            EventAggregator.GetEvent<DayChangedEvent>().Publish(DateChangedEnum.Increase);
            UpdateView();
            Logger.ForContext("Click", typeof(DayChangedEvent)).Information("NextDay has been clicked");
        }

        public void PreviousDayCommandHandler()
        {
            CurrentDay = CurrentDay.AddDays(-1);
            EventAggregator.GetEvent<DayChangedEvent>().Publish(DateChangedEnum.Decrease);
            UpdateView();
            Logger.ForContext("Click", typeof(DayChangedEvent)).Information("PreviousDay has been clicked");
        }

        #endregion

        private async void UpdateView()
        {
            var date = await GetEvents(CurrentDay);
            EventsViewModel.UpdateView(date);
        }

        public async Task<IList<Event>> GetEvents(DateTime date)
        {
            var data = await DataService.GetAsync(date);
            return data.ToList();
        }

        #region NavigationAware implementation

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            if (navigationContext == null || navigationContext.Parameters.ToList().Count <= 0)
            {
                UpdateView();
            }
            else
            {
                if (DateTime.TryParse(navigationContext.Parameters["date"].ToString(), out var day))
                {
                    CurrentDay = day;
                    EventsViewModel.Date = day;
                    UpdateView();
                    DateViewModel.SetDate(day);
                    DateYearHeader.SetDate(day);
                }
            }
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        #endregion
    }
}