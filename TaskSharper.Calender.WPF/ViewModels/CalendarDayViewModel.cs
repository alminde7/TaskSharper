using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Input;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Serilog;
using TaskSharper.Calender.WPF.Config;

using TaskSharper.Domain.Calendar;
using TaskSharper.Shared.Exceptions;
using TaskSharper.WPF.Common.Events;
using TaskSharper.WPF.Common.Events.DateChangedEvents;
using TaskSharper.WPF.Common.Events.NotificationEvents;
using TaskSharper.WPF.Common.Events.Resources;

namespace TaskSharper.Calender.WPF.ViewModels
{
    public class CalendarDayViewModel : BindableBase, INavigationAware
    {
        private readonly IRegionManager _regionManager;
        public IEventAggregator EventAggregator { get; }
        public IEventRestClient DataService { get; }
        public ILogger Logger { get; }

        public CalendarEventsViewModel EventsViewModel { get; set; }
        public CalendarAllDayEventContainerViewModel AllDayEventContainer { get; set; }
        public CalendarDateViewModel DateViewModel { get; set; }
        public CalendarYearHeaderViewModel DateYearHeader { get; set; }
        public DateTime CurrentDay { get; set; }

        public ICommand NextCommand { get; set; }
        public ICommand PrevCommand { get; set; }

        public CalendarDayViewModel(IEventAggregator eventAggregator, IEventRestClient dataService, IRegionManager regionManager, ILogger logger)
        {
            _regionManager = regionManager;
            EventAggregator = eventAggregator;
            DataService = dataService;
            Logger = logger.ForContext<CalendarDayViewModel>();
            CurrentDay = DateTime.Now;

            // Initialize views
            EventsViewModel = new CalendarEventsViewModel(CurrentDay, eventAggregator, _regionManager, dataService,
                CalendarTypeEnum.Day, Logger);
            AllDayEventContainer = new CalendarAllDayEventContainerViewModel(CurrentDay, regionManager, eventAggregator, logger);
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
            var data = await GetEvents(CurrentDay);
            if (data.normalEvents != null)
            {
                ApplicationStatus.InternetConnection = true;
                EventsViewModel.UpdateView(data.normalEvents);
            }
            if (data.allDayEvents != null)
            {
                ApplicationStatus.InternetConnection = true;
                AllDayEventContainer.SetAllDayEvents(data.allDayEvents.ToList());
            }
        }

        public async Task<(IList<Event> normalEvents, IList<Event> allDayEvents)> GetEvents(DateTime date)
        {
            try
            {
                var data = await DataService.GetAsync(date);
                var normalEvents = data.Where(e => !e.AllDayEvent.HasValue);
                var allDayEvents = data.Where(e => e.AllDayEvent.HasValue);

                return (normalEvents.ToList(), allDayEvents.ToList());
            }
            catch (ConnectionException e)
            {
                EventAggregator.GetEvent<NotificationEvent>().Publish(new ConnectionErrorNotification());
                return (null,null);
            }
            catch (ArgumentException e)
            {
                // Client error exception
                return (null, null);
            }
            catch (HttpException e)
            {
                // Internal server error
                return (null, null);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return (null, null);
            }
        }

        #region NavigationAware implementation

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            EventAggregator.GetEvent<ScrollButtonsEvent>().Publish(EventResources.ScrollButtonsEnum.Show);
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
            EventAggregator.GetEvent<ScrollButtonsEvent>().Publish(EventResources.ScrollButtonsEnum.Hide);
        }

        #endregion
    }
}