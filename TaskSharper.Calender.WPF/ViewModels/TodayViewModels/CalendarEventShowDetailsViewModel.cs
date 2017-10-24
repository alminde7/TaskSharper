using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using TaskSharper.Calender.WPF.Config;
using TaskSharper.Domain.BusinessLayer;
using TaskSharper.Domain.Calendar;

namespace TaskSharper.Calender.WPF.ViewModels
{
    public class CalendarEventShowDetailsViewModel : BindableBase , INavigationAware
    {
        private readonly IEventManager _calendarService;
        private readonly IRegionManager _regionManager;
        private Event _selectedEvent;

        public DelegateCommand EventDetailsClickCommand { get; set; }
        public DelegateCommand BackCommand { get; set; }

        public Event SelectedEvent
        {
            get => _selectedEvent;
            set => SetProperty(ref _selectedEvent, value);
        }

        public CalendarEventShowDetailsViewModel(IEventManager calendarService, IRegionManager regionManager)
        {
            _calendarService = calendarService;
            _regionManager = regionManager;

            BackCommand = new DelegateCommand(Back);
            EventDetailsClickCommand = new DelegateCommand(EventEditDetailsClick);
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            var id = navigationContext.Parameters["id"].ToString();

            SelectedEvent = _calendarService.GetEvent(id);
        }

        private void EventEditDetailsClick()
        {
            Navigate(ViewConstants.VIEW_CalendarEventDetails);
        }
        private void Back()
        {
            _regionManager.Regions["CalendarRegion"].NavigationService.Journal.GoBack();
        }
        private void Navigate(string uri)
        {
            _regionManager.RequestNavigate(ViewConstants.REGION_Calendar, uri + $"?id={_selectedEvent.Id}");
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
           
        }
    }
}
