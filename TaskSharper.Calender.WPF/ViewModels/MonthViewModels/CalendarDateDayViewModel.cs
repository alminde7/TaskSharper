using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Prism.Commands;
using Prism.Regions;
using Serilog;
using TaskSharper.Calender.WPF.Events;
using TaskSharper.Calender.WPF.Events.Resources;
using TaskSharper.Domain.BusinessLayer;
using TaskSharper.Shared.Extensions;

namespace TaskSharper.Calender.WPF.ViewModels.MonthViewModels
{
    public class CalendarDateDayViewModel : BindableBase
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly CalendarTypeEnum _dateType;
        private readonly IRegionManager _regionManager;
        private readonly ILogger _logger;
        private int _dayOfMonth;
        private DateTime _date;
        private bool _isCurrentDay;

        public DelegateCommand GoToDayViewCommand { get; set; }

        public ObservableCollection<CalendarDayEventViewModel> CalendarEvents { get; set; }    
        public IEventManager EventManager { get; set; }

        public DateTime Date
        {
            get => _date;
            set
            {
                DayOfMonth = value.Day;
                if (value.Date == DateTime.Now.Date)
                    IsCurrentDay = true;
                else
                    IsCurrentDay = false;
                _date = value;
                UpdateView();
            }
        }

        public int DayOfMonth
        {
            get => _dayOfMonth;
            set => SetProperty(ref _dayOfMonth, value);
        }

        public bool IsCurrentDay
        {
            get => _isCurrentDay;
            set => SetProperty(ref _isCurrentDay, value);
        }

        public CalendarDateDayViewModel(DateTime date, IEventAggregator eventAggregator, IEventManager eventManager, CalendarTypeEnum dateType, ILogger logger, IRegionManager regionManager)
        {
            IsCurrentDay = false;

            // Initialize objects
            _eventAggregator = eventAggregator;
            _dateType = dateType;
            _regionManager = regionManager;
            EventManager = eventManager;
            _logger = logger.ForContext<CalendarDateDayViewModel>();
            
            // Initialize view containers
            CalendarEvents = new ObservableCollection<CalendarDayEventViewModel>();
            
            // Subscribe to events
            eventAggregator.GetEvent<MonthChangedEvent>().Subscribe(MonthChangedEventHandler);

            // Initialize event commands
            GoToDayViewCommand = new DelegateCommand(GoToDayView);

            // Set date => Will automatically call UpdateView()
            Date = date;
        }

        private void MonthChangedEventHandler(DateChangedEnum state)
        {
            if (_dateType != CalendarTypeEnum.Month) return;
            switch (state)
            {
                case DateChangedEnum.Increase:
                    Date = Date.AddDays(28);
                    UpdateView();
                    break;
                case DateChangedEnum.Decrease:
                    Date = Date.AddDays(-28);
                    UpdateView();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        private void UpdateView()
        {
            CalendarEvents.Clear();
            GetEvents();
        }

        public void UpdateDate(DateTime date)
        {
            Date = date;
            UpdateView();
        }

        public async void GetEvents()
        {
            _eventAggregator.GetEvent<SpinnerEvent>().Publish(EventResources.SpinnerEnum.Show);

            try
            {
                var calendarEvents = await EventManager.GetEventsAsync(Date);

                foreach (var calendarEvent in calendarEvents)
                {
                    if (!calendarEvent.Start.HasValue || !calendarEvent.End.HasValue) continue;
                    CalendarEvents.Add(new CalendarDayEventViewModel() { Event = calendarEvent });

                }
                _eventAggregator.GetEvent<SpinnerEvent>().Publish(EventResources.SpinnerEnum.Hide);
            }
            catch (Exception e)
            {
                _eventAggregator.GetEvent<SpinnerEvent>().Publish(EventResources.SpinnerEnum.Hide);
                _logger.Error(e, "Failed to update view");
            }
        }

        public void GoToDayView()
        {
            _logger.Information("Navigating from MonthView to DayView");
            _regionManager.RequestNavigate("CalendarRegion", $"CalendarDayView?date={Date.StartOfDay()}");
        }
    }
}
