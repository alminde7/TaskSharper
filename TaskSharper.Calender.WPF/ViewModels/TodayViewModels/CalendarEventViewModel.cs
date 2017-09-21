using System;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Serilog;
using TaskSharper.Domain.Calendar;

namespace TaskSharper.Calender.WPF.ViewModels
{
    public class CalendarEventViewModel : BindableBase
    {
        private readonly IRegionManager _regionManager;
        private readonly ILogger _logger;

        public DelegateCommand EventClickCommand { get; set; }
        
        private bool _isTitleAndDescriptionActivated;
        private Event _event;
        

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
        
        public CalendarEventViewModel(int timeOfDay, IRegionManager regionManager, ILogger logger)
        {
            TimeOfDay = timeOfDay;
            IsTitleAndDescriptionActivated = true;
            _regionManager = regionManager;
            _logger = logger;
            EventClickCommand = new DelegateCommand(EventClick, CanExecute);
        }

        private void Navigate(string uri)
        {
            _regionManager.RequestNavigate("CalendarRegion", uri + $"?id={Event.Id}");
        }

        private void EventClick()
        {
            Navigate("CalendarEventDetailsView");
        }

        private bool CanExecute()
        {
            return !string.IsNullOrEmpty(Event.Id);
        }
    }
}
