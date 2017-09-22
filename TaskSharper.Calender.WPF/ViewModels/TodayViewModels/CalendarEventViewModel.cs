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
            _logger = logger;
            EventClickCommand = new DelegateCommand(EventClick, CanExecute);
            _eventAggregator.GetEvent<EventClickedEvent>().Subscribe(LogEventClick);
        }

        private void LogEventClick(EventClickObject obj)
        {
            if (obj.Event != null)
            {
                _logger.Information($"Event {obj.Event.Title} (id: {obj.Event.Id}) with timespan {obj.TimeOfDay} was clicked at {DateTime.Now}");
            }
        }

        private void Navigate(string uri)
        {
            _regionManager.RequestNavigate("CalendarRegion", uri + $"?id={Event.Id}");
        }

        private void EventClick()
        {
            
            if (IsPopupOpen)
            {
                Navigate("CalendarEventDetailsView");
            }
            else
            {
                IsPopupOpen = true;
                _subscriptionToken = _eventAggregator.GetEvent<EventClickedEvent>().Subscribe(ClosePopup);
            }
        }

        private bool CanExecute()
        {
            var canExecute = !string.IsNullOrEmpty(Event?.Id);
            _eventAggregator.GetEvent<EventClickedEvent>().Publish(new EventClickObject() { Event = Event, TimeOfDay = TimeOfDay });

            return canExecute;
        }

        private void ClosePopup(EventClickObject obj)
        {
            IsPopupOpen = false;
            _eventAggregator.GetEvent<EventClickedEvent>().Unsubscribe(_subscriptionToken);
        }
    }
}
