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
    public class CalendarTodayViewModel : BindableBase
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
            Logger = logger;
            CurrentDay = DateTime.Now;

            // Initialize views
            EventsViewModel = new CalendarEventsViewModel(CurrentDay, eventAggregator, _regionManager, calendarService, CalendarTypeEnum.Day, Logger);
            DateViewModel = new CalendarDateViewModel(CurrentDay, eventAggregator, CalendarTypeEnum.Day, Logger);
            DateYearHeader = new CalendarYearHeaderViewModel(EventAggregator, CalendarTypeEnum.Day);

            // Initialize commands
            NextCommand = new DelegateCommand(NextDayCommandHandler);
            PrevCommand = new DelegateCommand(PreviousDayCommandHandler);
        }

        public void NextDayCommandHandler()
        {
            CurrentDay = CurrentDay.AddDays(1);
            EventAggregator.GetEvent<DayChangedEvent>().Publish(DateChangedEnum.Increase);
        }

        public void PreviousDayCommandHandler()
        {
            CurrentDay = CurrentDay.AddDays(-1);
            EventAggregator.GetEvent<DayChangedEvent>().Publish(DateChangedEnum.Decrease);
        }
    }
}
