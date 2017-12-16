using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Serilog;
using TaskSharper.Domain.Calendar;
using TaskSharper.Domain.Models;

namespace TaskSharper.Calender.WPF.ViewModels
{
    /// <inheritdoc />
    /// <summary>
    /// ViewModel for the CalendarAllDayEvent container
    /// </summary>
    public class CalendarAllDayEventContainerViewModel : BindableBase
    {
        /// <summary>
        /// Region manager for navigation
        /// </summary>
        private readonly IRegionManager _regionManager;

        /// <summary>
        /// Event aggregator for subscribing to and publishing events
        /// </summary>
        private readonly IEventAggregator _eventAggregator;

        /// <summary>
        /// Logger for logging
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// DateTime object used for data binding in the view.
        /// </summary>
        public DateTime Date;

        /// <summary>
        /// Collection of AllDayEvents that can be bound to in the view.
        /// </summary>
        public ObservableCollection<CalendarAllDayEventViewModel> AllDayEvents { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="date">Date for the all day events</param>
        /// <param name="regionManager">Region manager for navigation</param>
        /// <param name="eventAggregator">Event aggregator for subscribing to and publishing events</param>
        /// <param name="logger">Logger for logging</param>
        public CalendarAllDayEventContainerViewModel(DateTime date, IRegionManager regionManager, IEventAggregator eventAggregator, ILogger logger)
        {
            Date = date;
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;
            _logger = logger;

            AllDayEvents = new ObservableCollection<CalendarAllDayEventViewModel>();
        }

        /// <summary>
        /// Setting the AllDayEvents list.
        /// </summary>
        /// <param name="events">List of AllDay events</param>
        public void SetAllDayEvents(List<Event> events = null)
        {
            AllDayEvents.Clear();
            if (events != null)
                foreach (var @event in events)
                {
                    AllDayEvents.Add(new CalendarAllDayEventViewModel(Date, _regionManager, _eventAggregator, _logger)
                    {
                        Event = @event
                    });
                }
        }
    }
}
