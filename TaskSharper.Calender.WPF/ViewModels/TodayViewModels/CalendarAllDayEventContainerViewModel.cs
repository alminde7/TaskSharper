using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Serilog;
using TaskSharper.Domain.Calendar;

namespace TaskSharper.Calender.WPF.ViewModels
{
    public class CalendarAllDayEventContainerViewModel : BindableBase
    {
        private readonly IRegionManager _regionManager;
        private readonly IEventAggregator _eventAggregator;
        private readonly ILogger _logger;
        public DateTime Date;

        public ObservableCollection<CalendarAllDayEventViewModel> AllDayEvents { get; set; }

        public CalendarAllDayEventContainerViewModel(DateTime date, IRegionManager regionManager, IEventAggregator eventAggregator, ILogger logger)
        {
            Date = date;
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;
            _logger = logger;

            AllDayEvents = new ObservableCollection<CalendarAllDayEventViewModel>();
        }

        public void SetAllDayEvents(List<Event> events = null)
        {
            AllDayEvents.Clear();
            if (events != null)
                foreach (var @event in events)
                {
                    AllDayEvents.Add(new CalendarAllDayEventViewModel(Date, _regionManager, _eventAggregator, _logger)
                    {
                        Event = @event
                    });
                }
        }
    }
}
