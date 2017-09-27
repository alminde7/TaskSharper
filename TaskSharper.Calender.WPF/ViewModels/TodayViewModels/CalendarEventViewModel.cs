using System;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Serilog;
using TaskSharper.Calender.WPF.Events;
using TaskSharper.Domain.Calendar;

namespace TaskSharper.Calender.WPF.ViewModels
{
    public class CalendarEventViewModel : BindableBase
    {
        private readonly IRegionManager _regionManager;
        private readonly IEventAggregator _eventAggregator;
        private readonly ILogger _logger;

        public DelegateCommand EventClickCommand { get; set; }
        public DelegateCommand EventDetailsClickCommand { get; set; }
        
        private Event _event;
        private bool _isPopupOpen;
        SubscriptionToken _subscriptionToken;
        private double _height;
        private double _width;
        private double _locY;
        private double _locX;

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

        public CalendarEventViewModel(IRegionManager regionManager, IEventAggregator eventAggregator, ILogger logger)
        {
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;
            _logger = logger.ForContext<CalendarEventViewModel>();
            EventClickCommand = new DelegateCommand(EventClick);
            EventDetailsClickCommand = new DelegateCommand(EventDetailsClick);
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
            _regionManager.RequestNavigate("CalendarRegion", uri + $"?id={Event.Id}");
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
            Navigate("CalendarEventDetailsView");
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
