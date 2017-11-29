﻿using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Serilog;
using TaskSharper.Calender.WPF.Config;
using TaskSharper.Domain.Calendar;
using TaskSharper.WPF.Common.Events;

namespace TaskSharper.Calender.WPF.ViewModels
{
    public class CalendarEventViewModel : BindableBase
    {
        private readonly IRegionManager _regionManager;
        private readonly IEventAggregator _eventAggregator;
        private readonly ILogger _logger;

        public DelegateCommand EventClickCommand { get; set; }
        public DelegateCommand EventDetailsClickCommand { get; set; }
        public DelegateCommand<object> OnLayoutUpdatedCommand { get; set; }
        public DelegateCommand<object> OnLoadedCommand { get; set; }


        private Event _event;
        private bool _isPopupOpen;
        SubscriptionToken _subscriptionToken;
        private double _height;
        private double _width;
        private double _locY;
        private double _locX;
        private bool _isEventTask;
        public double Column { get; set; } // Not binding to this anywhere, only used for LocX
        public double TotalColumns { get; set; } // Not binding to this anywhere, only used for LocX
        public double ColumnSpan { get; set; }

        public Event Event
        {
            get => _event;
            set => SetProperty(ref _event, value);
        }

        public bool IsPopupOpen
        {
            get => _isPopupOpen;
            set => SetProperty(ref _isPopupOpen, value);
        }

        public double Height
        {
            get => _height;
            set => SetProperty(ref _height, value);
        }

        public double Width
        {
            get => _width;
            set => SetProperty(ref _width, value);
        }

        public double LocX
        {
            get => _locX;
            set => SetProperty(ref _locX, value);
        }

        public double LocY
        {
            get => _locY;
            set => SetProperty(ref _locY, value);
        }

        public bool IsEventTask
        {
            get => _isEventTask;
            set => SetProperty(ref _isEventTask, value);
        }

        public CalendarEventViewModel(IRegionManager regionManager, IEventAggregator eventAggregator, ILogger logger)
        {
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;
            _logger = logger.ForContext<CalendarEventViewModel>();
            EventClickCommand = new DelegateCommand(EventClick);
            EventDetailsClickCommand = new DelegateCommand(EventDetailsClick);
            OnLayoutUpdatedCommand = new DelegateCommand<object>(OnLayoutUpdated);
            OnLoadedCommand = new DelegateCommand<object>(OnLoaded);
        }

        private void OnLoaded(object containerWidth)
        {
            Width = (double) containerWidth * 1 / TotalColumns * (ColumnSpan + 1);
            LocX = Column * (double)containerWidth * 1 / TotalColumns;
            IsEventTask = Event?.Type == EventType.Task;
        }

        private void OnLayoutUpdated(object containerWidth)
        {
            Width = (double)containerWidth * 1 / TotalColumns * (ColumnSpan + 1);
            LocX = Column * (double)containerWidth * 1 / TotalColumns;
        }

        private void LogEventClick()
        {
            if (Event != null)
            {
                _logger.ForContext("Click", Event).Information($"Event {Event.Title} (id: {Event.Id}) was clicked.");
            }
        }

        private void Navigate(string uri)
        {
            _regionManager.RequestNavigate(ViewConstants.REGION_Calendar, uri + $"?id={Event.Id}");
        }

        private void EventClick()
        {
            if (CanExecuteEventClick())
            {
                LogEventClick();
                IsPopupOpen = true;
                _subscriptionToken = _eventAggregator.GetEvent<EventClickedEvent>().Subscribe(ClosePopup);
            }
        }

        private void EventDetailsClick()
        {
            Navigate(ViewConstants.VIEW_CalendarEventShowDetails);
        }

        private bool CanExecuteEventClick()
        {
            var canExecute = !string.IsNullOrEmpty(Event?.Id);
            _eventAggregator.GetEvent<EventClickedEvent>().Publish(Event);

            return canExecute;
        }

        private void ClosePopup(Event obj)
        {
            IsPopupOpen = false;
            _eventAggregator.GetEvent<EventClickedEvent>().Unsubscribe(_subscriptionToken);
        }
    }
}
