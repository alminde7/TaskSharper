using System;
using System.Collections.ObjectModel;
using Prism.Events;
using Prism.Mvvm;
using TaskSharper.Calender.WPF.Events;
using TaskSharper.Calender.WPF.Events.Resources;
using Prism.Commands;
using Prism.Regions;
using TaskSharper.Domain.BusinessLayer;

namespace TaskSharper.Calender.WPF.ViewModels
{
    public class CalendarWeekViewModel : BindableBase 
    {
        private const int DaysInWeek = 7;

        private readonly IEventManager _service;
        private IEventAggregator _eventAggregator;
        private readonly IRegionManager _regionManager;

        public ObservableCollection<CalendarDateViewModel> DateHeaders { get; set; }
        public ObservableCollection<CalendarEventsViewModel> EventContainers { get; set; }
        public DateTime CurrentWeek { get; set; }

        public DelegateCommand NextCommand { get; set; }
        public DelegateCommand PrevCommand { get; set; }


        public CalendarWeekViewModel(IEventManager service, IEventAggregator eventAggregator, IRegionManager regionManager)
        {
            _service = service;
            _eventAggregator = eventAggregator;
            _regionManager = regionManager;

            NextCommand = new DelegateCommand(NextWeek);
            PrevCommand = new DelegateCommand(PreviousWeek);
            
            DateHeaders = new ObservableCollection<CalendarDateViewModel>();
            EventContainers = new ObservableCollection<CalendarEventsViewModel>();

            CurrentWeek = DateTime.Now;

            InitializeViews();
        }

        #region Commands
        private void NextWeek()
        {
            CurrentWeek = CurrentWeek.Date.AddDays(7);
            _eventAggregator.GetEvent<DateChangedEvent>().Publish(DateChangeEnum.IncreaseWeek);
        }

        private void PreviousWeek()
        {
            CurrentWeek = CurrentWeek.Date.AddDays(-7);
            _eventAggregator.GetEvent<DateChangedEvent>().Publish(DateChangeEnum.DecreaseWeek);
        }
        #endregion

        #region Bootstrap Views
        private void InitializeViews()
        {
            for (int i = 1; i <= DaysInWeek; i++)
            {
                var date = CalculateDate(i);
                DateHeaders.Add(new CalendarDateViewModel(date, _eventAggregator));
                EventContainers.Add(new CalendarEventsViewModel(date, _eventAggregator, _service, CalendarTypeEnum.Week));
            }
        }

        private DateTime CalculateDate(int day)
        {
            var dayOffset = day - (int)DateTime.Now.DayOfWeek;

            var dateTime = DateTime.Now.AddDays(dayOffset);

            return dateTime;
        }
        #endregion

    }
}
