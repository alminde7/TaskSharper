using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using TaskSharper.DataAccessLayer.Google;
using TaskSharper.Domain.Calendar;

namespace TaskSharper.Calender.WPF.ViewModels
{
    public class CalendarEventDetailsViewModel : BindableBase, INavigationAware
    {
        public DelegateCommand BackCommand { get; set; }
        private readonly IRegionManager _regionManager;
        
        private readonly ICalendarService _calendarService;
        private string _title;
        private Event _selectedEvent;

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public Event SelectedEvent
        {
            get => _selectedEvent;
            set => SetProperty(ref _selectedEvent, value);
        }

        public CalendarEventDetailsViewModel(IRegionManager regionManager, ICalendarService calendarService)
        {
            _regionManager = regionManager;
            _calendarService = calendarService;
            BackCommand = new DelegateCommand(Back);
        }
        

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            var id = navigationContext.Parameters["id"].ToString();

            SelectedEvent = _calendarService.GetEvents(Constants.DefaultGoogleCalendarId).Find(i => i.Id == id);
            
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        private void Back()
        {
            _regionManager.Regions["CalendarRegion"].NavigationService.Journal.GoBack();
        }
    }
}
