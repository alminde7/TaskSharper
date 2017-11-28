using System;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Serilog;
using TaskSharper.Calender.WPF.Config;
using TaskSharper.Domain.Calendar;

namespace TaskSharper.Calender.WPF.ViewModels
{
    public class CalendarAllDayEventViewModel : BindableBase
    {
        private readonly IRegionManager _regionManager;
        private readonly IEventAggregator _eventAggregator;
        private readonly ILogger _logger;
        public DateTime Date;
        private Event _event;
        public DelegateCommand EventDetailsClickCommand { get; set; }

        private string _eventTypeAndTitle;
        
        public Event Event
        {
            get => _event;
            set
            {
                EventTypeAndTitle = $"{value.Type.ToString()}: {value.Title}";
                SetProperty(ref _event, value);
            }
        }

        public string EventTypeAndTitle
        {
            get => _eventTypeAndTitle;
            set => SetProperty(ref _eventTypeAndTitle, value);
        }

        public CalendarAllDayEventViewModel(DateTime date, IRegionManager regionManager, IEventAggregator eventAggregator, ILogger logger)
        {
            Date = date;
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;
            _logger = logger;
            EventDetailsClickCommand = new DelegateCommand(EventDetailsClick);
        }

        private void Navigate(string uri)
        {
            _regionManager.RequestNavigate(ViewConstants.REGION_Calendar, uri + $"?id={Event.Id}");
        }

        private void EventDetailsClick()
        {
            Navigate(ViewConstants.VIEW_CalendarEventShowDetails);
        }
    }
}
