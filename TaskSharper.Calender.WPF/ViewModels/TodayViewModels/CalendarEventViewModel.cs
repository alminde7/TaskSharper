using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Serilog;
using TaskSharper.Calender.WPF.Config;
using TaskSharper.Domain.Calendar;
using TaskSharper.Domain.Models;
using TaskSharper.WPF.Common.Events;

namespace TaskSharper.Calender.WPF.ViewModels
{
    /// <inheritdoc />
    /// <summary>
    /// ViewModel for a calendar event.
    /// </summary>
    public class CalendarEventViewModel : BindableBase
    {
        private readonly IRegionManager _regionManager;
        private readonly IEventAggregator _eventAggregator;
        private readonly ILogger _logger;
        
        /// <summary>
        /// Command that is used to bind to the event click event.
        /// </summary>
        public DelegateCommand EventDetailsClickCommand { get; set; }

        /// <summary>
        /// Command that is used to bind to when the layout is updated (eg. when the size of the screen is changed).
        /// </summary>
        public DelegateCommand<object> OnLayoutUpdatedCommand { get; set; }

        /// <summary>
        /// Command that is used to bind to the event when the event is loaded in the view.
        /// </summary>
        public DelegateCommand<object> OnLoadedCommand { get; set; }


        private Event _event;
        private double _height;
        private double _width;
        private double _locY;
        private double _locX;
        private bool _isEventTask;

        /// <summary>
        /// Not binding to this anywhere, only used for LocX.
        /// </summary>
        public double Column { get; set; }

        /// <summary>
        /// Not binding to this anywhere, only used for LocX
        /// </summary>
        public double TotalColumns { get; set; }

        /// <summary>
        /// To determine how wide the event should be in the view (how many columns it should span across).
        /// </summary>
        public double ColumnSpan { get; set; }

        /// <summary>
        /// Event that this ViewModel works on.
        /// </summary>
        public Event Event
        {
            get => _event;
            set => SetProperty(ref _event, value);
        }

        /// <summary>
        /// Height of the presentation of the event on the canvas in the view.
        /// </summary>
        public double Height
        {
            get => _height;
            set => SetProperty(ref _height, value);
        }

        /// <summary>
        /// Width of the presentation of the event on the canvas in the view.
        /// </summary>
        public double Width
        {
            get => _width;
            set => SetProperty(ref _width, value);
        }

        /// <summary>
        /// X-position on the canvas in the view.
        /// </summary>
        public double LocX
        {
            get => _locX;
            set => SetProperty(ref _locX, value);
        }

        /// <summary>
        /// Y-position on the canvas in the view.
        /// </summary>
        public double LocY
        {
            get => _locY;
            set => SetProperty(ref _locY, value);
        }

        /// <summary>
        /// Whether or not the event is a task.
        /// </summary>
        public bool IsEventTask
        {
            get => _isEventTask;
            set => SetProperty(ref _isEventTask, value);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="regionManager">Region manager for navigation</param>
        /// <param name="eventAggregator">Event aggregator for subscribing to and publishing events</param>
        /// <param name="logger">Logger for logging</param>
        public CalendarEventViewModel(IRegionManager regionManager, IEventAggregator eventAggregator, ILogger logger)
        {
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;
            _logger = logger.ForContext<CalendarEventViewModel>();
            EventDetailsClickCommand = new DelegateCommand(EventDetailsClick);
            OnLayoutUpdatedCommand = new DelegateCommand<object>(OnLayoutUpdated);
            OnLoadedCommand = new DelegateCommand<object>(OnLoaded);
        }

        /// <summary>
        /// Event handler for when the event has been loaded in the view.
        /// </summary>
        /// <param name="containerWidth">The width of the container</param>
        private void OnLoaded(object containerWidth)
        {
            Width = (double) containerWidth * 1 / TotalColumns * (ColumnSpan + 1);
            LocX = Column * (double)containerWidth * 1 / TotalColumns;
            IsEventTask = Event?.Type == EventType.Task;
        }

        /// <summary>
        /// Event handler for when the layout has been changed (eg. the size of the screen has changed).
        /// </summary>
        /// <param name="containerWidth">The width of the container</param>
        private void OnLayoutUpdated(object containerWidth)
        {
            Width = (double)containerWidth * 1 / TotalColumns * (ColumnSpan + 1);
            LocX = Column * (double)containerWidth * 1 / TotalColumns;
        }

        /// <summary>
        /// To log when an event has been clicked.
        /// </summary>
        private void LogEventClick()
        {
            if (Event != null)
            {
                _logger.ForContext("Click", Event).Information($"Event {Event.Title} (id: {Event.Id}) was clicked.");
            }
        }

        /// <summary>
        /// To navigate to a different view.
        /// </summary>
        /// <param name="uri">URI of the view to be navigated to</param>
        private void Navigate(string uri)
        {
            _regionManager.RequestNavigate(ViewConstants.REGION_Calendar, uri + $"?id={Event.Id}");
        }

        /// <summary>
        /// Event handler for when an event has been clicked.
        /// </summary>
        private void EventDetailsClick()
        {
            LogEventClick();
            Navigate(ViewConstants.VIEW_CalendarEventShowDetails);
        }
    }
}
