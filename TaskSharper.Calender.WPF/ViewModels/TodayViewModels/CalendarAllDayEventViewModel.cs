using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private Event _event;
        public DelegateCommand EventDetailsClickCommand { get; set; }

        private double _scrollPosition;

        public Event Event
        {
            get => _event;
            set => SetProperty(ref _event, value);
        }

        public double ScrollPosition
        {
            get => _scrollPosition;
            set => SetProperty(ref _scrollPosition, value);
        }

        public CalendarAllDayEventViewModel(IRegionManager regionManager, IEventAggregator eventAggregator, ILogger logger)
        {
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;
            _logger = logger;
            EventDetailsClickCommand = new DelegateCommand(EventDetailsClick);
            ScrollPosition = 0;
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
