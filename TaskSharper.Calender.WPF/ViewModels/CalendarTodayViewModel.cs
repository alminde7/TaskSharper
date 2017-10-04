using System;
using System.Windows.Input;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Serilog;
using TaskSharper.BusinessLayer;
using TaskSharper.Calender.WPF.Events;
using TaskSharper.Calender.WPF.Events.Resources;
using TaskSharper.Domain.BusinessLayer;
using TaskSharper.Domain.Calendar;

namespace TaskSharper.Calender.WPF.ViewModels
{
    public class CalendarTodayViewModel : BindableBase, INavigationAware
    {
        private readonly IRegionManager _regionManager;
        public IEventAggregator EventAggregator { get; }
        public IEventManager CalendarService { get; }
        public ILogger Logger { get; }

        public CalendarEventsViewModel EventsViewModel { get; set; }
        public CalendarDateViewModel DateViewModel { get; set; }
        public CalendarYearHeaderViewModel DateYearHeader { get; set; }
        public DateTime CurrentDay { get; set; }

        public ICommand NextCommand { get; set; }
        public ICommand PrevCommand { get; set; }

        public CalendarTodayViewModel(IEventAggregator eventAggregator, IEventManager calendarService, IRegionManager regionManager, ILogger logger)
        {
            _regionManager = regionManager;
            EventAggregator = eventAggregator;
            CalendarService = calendarService;
            Logger = logger.ForContext<CalendarTodayViewModel>();
            CurrentDay = DateTime.Now;

            // Initialize views
            EventsViewModel = new CalendarEventsViewModel(CurrentDay, eventAggregator, _regionManager, calendarService, CalendarTypeEnum.Day, Logger);
            DateViewModel = new CalendarDateViewModel(CurrentDay, eventAggregator, CalendarTypeEnum.Day, Logger);
            DateYearHeader = new CalendarYearHeaderViewModel(EventAggregator, CalendarTypeEnum.Day, Logger);

            // Initialize commands
            NextCommand = new DelegateCommand(NextDayCommandHandler);
            PrevCommand = new DelegateCommand(PreviousDayCommandHandler);
        }

        public void NextDayCommandHandler()
        {
            CurrentDay = CurrentDay.AddDays(1);
            EventAggregator.GetEvent<DayChangedEvent>().Publish(DateChangedEnum.Increase);
            Logger.ForContext("Click", typeof(DayChangedEvent)).Information("NextDay has been clicked");
        }

        public void PreviousDayCommandHandler()
        {
            CurrentDay = CurrentDay.AddDays(-1);
            EventAggregator.GetEvent<DayChangedEvent>().Publish(DateChangedEnum.Decrease);
            Logger.ForContext("Click", typeof(DayChangedEvent)).Information("PreviousDay has been clicked");
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {

            // OBS: This is called on every navigation, however [date] parameter is only set on 
            // navigation from CalendarMonthView. When navigation is not from CalendarMonthView
            // it will throw an exception when trying to access parameter [date] - which is 
            // the reason for the empty catch block :)
            try
            {
                if (DateTime.TryParse(navigationContext.Parameters["date"].ToString(), out var day))
                {
                    EventsViewModel.Date = day;
                    DateViewModel.CurrentDate = day;
                    DateYearHeader.Date = day;
                }
            }
            catch
            {
                // ignored
            }
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
