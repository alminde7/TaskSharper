using System;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Serilog;
using TaskSharper.Calender.WPF.Config;
using TaskSharper.Domain.Calendar;
using TaskSharper.Domain.Models;

namespace TaskSharper.Calender.WPF.ViewModels
{
    /// <inheritdoc />
    /// <summary>
    /// ViewModel for CalendarAllDayEvent
    /// </summary>
    public class CalendarAllDayEventViewModel : BindableBase
    {
        private readonly IRegionManager _regionManager;
        private readonly IEventAggregator _eventAggregator;
        private readonly ILogger _logger;
        
        /// <summary>
        /// DateTime object used for data binding in the view.
        /// </summary>
        public DateTime Date;
        private Event _event;

        /// <summary>
        /// Command that is used to bind to the Event details click event.
        /// </summary>
        public DelegateCommand EventDetailsClickCommand { get; set; }

        private string _eventTypeAndTitle;
        
        /// <summary>
        /// The AllDay event.
        /// This also sets the EventTypeAndTitle.
        /// </summary>
        public Event Event
        {
            get => _event;
            set
            {
                EventTypeAndTitle = $"{value.Type.ToString()}: {value.Title}";
                SetProperty(ref _event, value);
            }
        }

        /// <summary>
        /// Message to be bound to in the view.
        /// </summary>
        public string EventTypeAndTitle
        {
            get => _eventTypeAndTitle;
            set => SetProperty(ref _eventTypeAndTitle, value);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="date">Date for the all day event</param>
        /// <param name="regionManager">Region manager for navigation</param>
        /// <param name="eventAggregator">Event aggregator for subscribing to and publishing events</param>
        /// <param name="logger">Logger for logging</param>
        public CalendarAllDayEventViewModel(DateTime date, IRegionManager regionManager, IEventAggregator eventAggregator, ILogger logger)
        {
            Date = date;
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;
            _logger = logger;
            EventDetailsClickCommand = new DelegateCommand(EventDetailsClick);
        }

        /// <summary>
        /// Used for navigating to other views.
        /// </summary>
        /// <param name="uri">URI of the view that is to be navigated to</param>
        private void Navigate(string uri)
        {
            _regionManager.RequestNavigate(ViewConstants.REGION_Calendar, uri + $"?id={Event.Id}");
        }

        /// <summary>
        /// Handler for when clicking the EventDetails button.
        /// </summary>
        private void EventDetailsClick()
        {
            Navigate(ViewConstants.VIEW_CalendarEventShowDetails);
        }
    }
}
