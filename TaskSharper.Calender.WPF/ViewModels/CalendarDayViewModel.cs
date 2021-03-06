﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Serilog;
using TaskSharper.Calender.WPF.Config;

using TaskSharper.Domain.Calendar;
using TaskSharper.Domain.Models;
using TaskSharper.Domain.RestClient;
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

        public DelegateCommand NextCommand { get; set; }
        public DelegateCommand PrevCommand { get; set; }
        public DelegateCommand CreateEventCommand { get; set; }

        /// <summary>
        /// Constructor for the CalendarDayViewModel
        /// </summary>
        /// <param name="eventAggregator">Dependency injection of the eventManager</param>
        /// <param name="dataService">Dependency injection of the eventManager</param>
        /// <param name="regionManager">Dependency injection of the regionManager</param>
        /// <param name="logger">Dependency injection of the logger</param>
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
            CreateEventCommand = new DelegateCommand(NavigateToCreateEventView);
        }

        /// <summary>
        /// Method to call the region manager and request a navigation to the CreateEventView()
        /// </summary>
        private void NavigateToCreateEventView()
        {
            var navigationParameters = new NavigationParameters();
            navigationParameters.Add("Type", EventType.None);
            navigationParameters.Add("Region", ViewConstants.REGION_Calendar);
            _regionManager.RequestNavigate(ViewConstants.REGION_Calendar, ViewConstants.VIEW_CalendarEventDetails, navigationParameters);
        }

        #region EventHandlers

        /// <summary>
        /// Method to publish an increase of day date. 
        /// </summary>
        public void NextDayCommandHandler()
        {
            CurrentDay = CurrentDay.AddDays(1);
            EventAggregator.GetEvent<DayChangedEvent>().Publish(DateChangedEnum.Increase);
            UpdateView();
            Logger.ForContext("Click", typeof(DayChangedEvent)).Information("NextDay has been clicked");
        }

        /// <summary>
        /// Method to publish an decrease of day date. 
        /// </summary>
        public void PreviousDayCommandHandler()
        {
            CurrentDay = CurrentDay.AddDays(-1);
            EventAggregator.GetEvent<DayChangedEvent>().Publish(DateChangedEnum.Decrease);
            UpdateView();
            Logger.ForContext("Click", typeof(DayChangedEvent)).Information("PreviousDay has been clicked");
        }

        #endregion

        /// <summary>
        /// Meathod to update all child view models. 
        /// </summary>
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

        /// <summary>
        /// Gets events by the date of the day
        /// </summary>
        /// <param name="date">date of the day</param>
        /// <returns>list of events </returns>
        public async Task<(IList<Event> normalEvents, IList<Event> allDayEvents)> GetEvents(DateTime date)
        {
            try
            {
                var data = await DataService.GetAsync(date);
                var normalEvents = data.Where(e => !e.AllDayEvent.HasValue);
                var allDayEvents = data.Where(e => e.AllDayEvent.HasValue);

                return (normalEvents.ToList(), allDayEvents.ToList());
            }
            catch (ConnectionException)
            {
                EventAggregator.GetEvent<NotificationEvent>().Publish(new ConnectionErrorNotification());
                return (null,null);
            }
            catch (UnauthorizedAccessException)
            {
                EventAggregator.GetEvent<NotificationEvent>().Publish(new UnauthorizedErrorNotification());
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