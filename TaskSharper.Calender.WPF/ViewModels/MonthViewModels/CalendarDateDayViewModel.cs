using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Prism.Commands;
using Prism.Regions;
using Serilog;
using TaskSharper.Calender.WPF.Config;
using TaskSharper.Domain.Calendar;
using TaskSharper.Domain.Models;
using TaskSharper.Domain.RestClient;
using TaskSharper.Shared.Extensions;
using TaskSharper.WPF.Common.Events.DateChangedEvents;
using TaskSharper.WPF.Common.Events.Resources;

namespace TaskSharper.Calender.WPF.ViewModels.MonthViewModels
{
    public class CalendarDateDayViewModel : BindableBase
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly CalendarTypeEnum _dateType;
        private readonly IRegionManager _regionManager;
        private readonly ILogger _logger;

        private DateTime _date;
        private bool _isCurrentDay;
        private bool _isWithinSelectedMonth;

        public DelegateCommand GoToDayViewCommand { get; set; }

        public ObservableCollection<CalendarDayEventViewModel> CalendarEvents { get; set; }    
        public IEventRestClient EventManager { get; set; }

        public DateTime Date
        {
            get => _date;
            set =>  SetProperty(ref _date, value);
        }

        public bool IsCurrentDay
        {
            get => _isCurrentDay;
            set => SetProperty(ref _isCurrentDay, value);
        }

        public bool IsWithinSelectedMonth
        {
            get => _isWithinSelectedMonth;
            set => SetProperty(ref _isWithinSelectedMonth, value);
        }
        
        public CalendarDateDayViewModel(DateTime date, IEventAggregator eventAggregator, IEventRestClient eventManager, CalendarTypeEnum dateType, ILogger logger, IRegionManager regionManager)
        {
            IsCurrentDay = false;
            IsWithinSelectedMonth = true;

            // Initialize objects
            _eventAggregator = eventAggregator;
            _dateType = dateType;
            _regionManager = regionManager;
            EventManager = eventManager;
            _logger = logger.ForContext<CalendarDateDayViewModel>();
            
            // Initialize view containers
            CalendarEvents = new ObservableCollection<CalendarDayEventViewModel>();
            
            // Subscribe to events
            eventAggregator.GetEvent<MonthChangedEvent>().Subscribe(MonthChangedEventHandler);

            // Initialize event commands
            GoToDayViewCommand = new DelegateCommand(GoToDayView);

            // Set date => Will automatically call UpdateView()
            UpdateDate(date);
        }

        private void MonthChangedEventHandler(DateChangedEnum state)
        {
            if (_dateType != CalendarTypeEnum.Month) return;
            switch (state)
            {
                case DateChangedEnum.Increase:
                    Date = Date.AddDays(28);
                    UpdateView();
                    break;
                case DateChangedEnum.Decrease:
                    Date = Date.AddDays(-28);
                    UpdateView();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        public void UpdateDate(DateTime date)
        {
            Date = date;
            IsCurrentDay = Date.Date == DateTime.Now.Date;
        }

        public void UpdateView(IList<Event> events = null)
        {
            if (events == null)
            {
                CalendarEvents.Clear();
            }
            else
            {
                CalendarEvents.Clear();
                GetEvents(events);
            }

        }

        private void GetEvents(IList<Event> calendarEvents)
        {
            foreach (var calendarEvent in calendarEvents)
            {
                if (!calendarEvent.Start.HasValue || !calendarEvent.End.HasValue) continue;
                CalendarEvents.Add(new CalendarDayEventViewModel() { Event = calendarEvent });
            }
        }

        public void GoToDayView()
        {
            _logger.Information("Navigating from MonthView to DayView");
            _regionManager.RequestNavigate(ViewConstants.REGION_Calendar, $"{ViewConstants.VIEW_CalendarDay}?date={Date.StartOfDay()}");
        }
    }
}
