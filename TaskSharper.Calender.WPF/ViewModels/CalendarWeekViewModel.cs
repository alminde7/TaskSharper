﻿using System;
using System.Collections.ObjectModel;
using Prism.Events;
using Prism.Mvvm;
using TaskSharper.Calender.WPF.Events;
using TaskSharper.Calender.WPF.Events.Resources;
using Prism.Commands;
using Prism.Regions;
using Serilog;
using TaskSharper.Domain.BusinessLayer;

namespace TaskSharper.Calender.WPF.ViewModels
{
    public class CalendarWeekViewModel : BindableBase 
    {
        private const int DaysInWeek = 7;

        private readonly IEventManager _service;
        private IEventAggregator _eventAggregator;
        private readonly IRegionManager _regionManager;
        private readonly ILogger _logger;

        public ObservableCollection<CalendarDateViewModel> DateHeaders { get; set; }
        public ObservableCollection<CalendarEventsViewModel> EventContainers { get; set; }
        public CalendarYearHeaderViewModel DateYearHeader { get; set; }
        public DateTime CurrentWeek { get; set; }

        public DelegateCommand NextCommand { get; set; }
        public DelegateCommand PrevCommand { get; set; }


        public CalendarWeekViewModel(IEventManager service, IEventAggregator eventAggregator, IRegionManager regionManager, ILogger logger)
        {
            _service = service;
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
        private void NextWeek()
        {
            CurrentWeek = CurrentWeek.Date.AddDays(7);
            _eventAggregator.GetEvent<WeekChangedEvent>().Publish(DateChangedEnum.Increase);
            _logger.ForContext("Click", typeof(WeekChangedEvent)).Information("NextWeek has been clicked");
        }

        private void PreviousWeek()
        {
            CurrentWeek = CurrentWeek.Date.AddDays(-7);
            _eventAggregator.GetEvent<WeekChangedEvent>().Publish(DateChangedEnum.Decrease);
            _logger.ForContext("Click", typeof(WeekChangedEvent)).Information("PreviousWeek has been clicked");
        }
        #endregion

        #region Bootstrap Views
        private void InitializeViews()
        {
            for (int i = 1; i <= DaysInWeek; i++)
            {
                var date = CalculateDate(i);
                DateHeaders.Add(new CalendarDateViewModel(date, _eventAggregator, CalendarTypeEnum.Week, _logger));
                EventContainers.Add(new CalendarEventsViewModel(date, _eventAggregator, _regionManager, _service, CalendarTypeEnum.Week, _logger));
            }
        }

        private DateTime CalculateDate(int day)
        {
            var dayOffset = day - (int)DateTime.Now.DayOfWeek;

            var dateTime = DateTime.Now.AddDays(dayOffset);

            return dateTime;
        }
        #endregion

    }
}
