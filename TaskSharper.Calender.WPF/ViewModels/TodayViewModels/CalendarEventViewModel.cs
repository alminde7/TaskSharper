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

        private bool _isTitleAndDescriptionActivated;
        private Event _event;
        private bool _isPopupOpen;
        SubscriptionToken _subscriptionToken;


        public int TimeOfDay { get; set; }

        public Event Event
        {
            get => _event;
            set
            {
                if (value?.Start != null && value.End.HasValue)
                {
                    if (value.Start.Value.Hour < TimeOfDay)
                    {
                        IsTitleAndDescriptionActivated = false;
                    }
                    else
                    {
                        IsTitleAndDescriptionActivated = true;
                    }
                }
                SetProperty(ref _event, value);
            }
        }

        public bool IsTitleAndDescriptionActivated
        {
            get => _isTitleAndDescriptionActivated;
            set => SetProperty(ref _isTitleAndDescriptionActivated, value);
        }

        public bool IsPopupOpen
        {
            get => _isPopupOpen;
            set => SetProperty(ref _isPopupOpen, value);
        }
        
        public CalendarEventViewModel(int timeOfDay, IRegionManager regionManager, IEventAggregator eventAggregator, ILogger logger)
        {
            TimeOfDay = timeOfDay;
            IsTitleAndDescriptionActivated = true;
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
                _logger.ForContext("Click", Event).Information($"Event {Event.Title} (id: {Event.Id}) with timespan {TimeOfDay} was clicked at {DateTime.Now}");
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
            var eventClickObject = new EventClickObject {Event = Event, TimeOfDay = TimeOfDay};
            _eventAggregator.GetEvent<EventClickedEvent>().Publish(eventClickObject);

            return canExecute;
        }

        private void ClosePopup(EventClickObject obj)
        {
            IsPopupOpen = false;
            _eventAggregator.GetEvent<EventClickedEvent>().Unsubscribe(_subscriptionToken);
        }
    }
}
