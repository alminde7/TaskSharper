﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using TaskSharper.Calender.WPF.Events;
using TaskSharper.Calender.WPF.Events.Resources;
using Prism.Commands;
using TaskSharper.DataAccessLayer.Google;
using TaskSharper.Domain.Calendar;

namespace TaskSharper.Calender.WPF.ViewModels
{
    public class CalendarWeekViewModel : BindableBase 
    {
        private const int DaysInWeek = 7;

        private readonly ICalendarService _service;
        private IEventAggregator _eventAggregator;
        private readonly IRegionManager _regionManager;


        public ObservableCollection<CalendarDateViewModel> DateHeaders { get; set; }
        public ObservableCollection<CalendarEventsViewModel> EventContainers { get; set; }
        public DateTime CurrentWeek { get; set; }

        public DelegateCommand NextCommand { get; set; }
        public DelegateCommand PrevCommand { get; set; }


        public CalendarWeekViewModel(ICalendarService service, IEventAggregator eventAggregator, IRegionManager regionManager)
        {
            _service = service;
            _eventAggregator = eventAggregator;

            NextCommand = new DelegateCommand(NextWeek);
            PrevCommand = new DelegateCommand(PreviousWeek);
            
            _regionManager = regionManager;
            DateHeaders = new ObservableCollection<CalendarDateViewModel>();
            EventContainers = new ObservableCollection<CalendarEventsViewModel>();

            CurrentWeek = DateTime.Now;

            InitializeViews();
        }

        #region Commands
        private void NextWeek()
        {
            CurrentWeek = CurrentWeek.Date.AddDays(7);
            _eventAggregator.GetEvent<DateChangedEvent>().Publish(DateChangeEnum.IncreaseWeek);
        }

        private void PreviousWeek()
        {
            CurrentWeek = CurrentWeek.Date.AddDays(-7);
            _eventAggregator.GetEvent<DateChangedEvent>().Publish(DateChangeEnum.DecreaseWeek);
        }
        #endregion

        #region Bootstrap Views
        private void InitializeViews()
        {
            for (int i = 1; i <= DaysInWeek; i++)
            {
                var date = CalculateDate(i);
                DateHeaders.Add(new CalendarDateViewModel(date, _eventAggregator));
                EventContainers.Add(new CalendarEventsViewModel(date, _eventAggregator, _service, _regionManager));
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
