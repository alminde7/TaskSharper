using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Practices.ObjectBuilder2;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Serilog;
using TaskSharper.BusinessLayer;
using TaskSharper.Calender.WPF.Events;
using TaskSharper.Calender.WPF.Events.Resources;
using TaskSharper.Domain.BusinessLayer;
using TaskSharper.Domain.Calendar;
using TaskSharper.Shared.Constants;

namespace TaskSharper.Calender.WPF.ViewModels
{
    public class CalendarEventsViewModel : BindableBase
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IRegionManager _regionManager;
        private readonly CalendarTypeEnum _dateType;
        private readonly ILogger _logger;

        public DateTime Date { get; set; }
        public IEventManager Service { get; set; }

        public ObservableCollection<CalendarEventViewModel> CalendarEvents { get; set; }
        public ObservableCollection<CalendarEventsBackground> Backgrounds { get; set; }

        public CalendarEventsViewModel(DateTime date, IEventAggregator eventAggregator, IRegionManager regionManager, IEventManager service, CalendarTypeEnum dateType, ILogger logger)
        {
            _eventAggregator = eventAggregator;
            _regionManager = regionManager;
            _dateType = dateType;
            _logger = logger.ForContext<CalendarEventsViewModel>();
            Date = date;
            Service = service;
            CalendarEvents = new ObservableCollection<CalendarEventViewModel>();
            Backgrounds = new ObservableCollection<CalendarEventsBackground>();

            _eventAggregator.GetEvent<DayChangedEvent>().Subscribe(DayChangedEventHandler);
            _eventAggregator.GetEvent<WeekChangedEvent>().Subscribe(WeekChangedEventHandler);
            _eventAggregator.GetEvent<MonthChangedEvent>().Subscribe(MonthChangedEventHandler);
            eventAggregator.GetEvent<EventChangedEvent>().Subscribe(EventChangedEventHandler);

            InitializeView();
            GetEvents();
        }

        #region EventHandlers
        private void MonthChangedEventHandler(DateChangedEnum state)
        {
            if (_dateType != CalendarTypeEnum.Month) return;
            switch (state)
            {
                case DateChangedEnum.Increase:
                    Date = Date.AddMonths(1);
                    UpdateView();
                    break;
                case DateChangedEnum.Decrease:
                    Date = Date.AddMonths(-1);
                    UpdateView();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        private void WeekChangedEventHandler(DateChangedEnum state)
        {
            if (_dateType != CalendarTypeEnum.Week) return;
            switch (state)
            {
                case DateChangedEnum.Increase:
                    Date = Date.AddDays(7);
                    UpdateView();
                    break;
                case DateChangedEnum.Decrease:
                    Date = Date.AddDays(-7);
                    UpdateView();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        private void DayChangedEventHandler(DateChangedEnum state)
        {
            if (_dateType != CalendarTypeEnum.Day) return;
            switch (state)
            {
                case DateChangedEnum.Increase:
                    Date = Date.AddDays(1);
                    UpdateView();
                    break;
                case DateChangedEnum.Decrease:
                    Date = Date.AddDays(-1);
                    UpdateView();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        private void EventChangedEventHandler(Event obj)
        {
            if (Date.Date == obj.Start.Value.Date)
            {
                Service.UpdateEvent(obj);
                UpdateView();
            }
        }
        #endregion

        private void InitializeView()
        {
            for (int i = 1; i < Time.HoursInADay; i = i + 2)
            {
                Backgrounds.Add(new CalendarEventsBackground
                {
                    Height = 50,
                    LocX = 0,
                    LocY = i * 50,
                    Color = Brushes.AliceBlue
                });
            }
        }

        private void UpdateView()
        {
            CalendarEvents.Clear();
            GetEvents();
        }

        private async void GetEvents()
        {
            _eventAggregator.GetEvent<SpinnerEvent>().Publish(EventResources.SpinnerEnum.Show);

            try
            {
                var calendarEvents = await Service.GetEventsAsync(Date.Date);

                foreach (var calendarEvent in calendarEvents)
                {
                    if (!calendarEvent.Start.HasValue || !calendarEvent.End.HasValue) continue;
                    var viewModel = new CalendarEventViewModel(_regionManager, _eventAggregator, _logger)
                    {
                        LocY = calendarEvent.Start.Value.Hour / Time.HoursInADay * 1200 +
                               calendarEvent.Start.Value.Minute / Time.MinutesInAnHour / Time.HoursInADay * 1200, // TODO: Declare 1200 as a constant somewhere
                        Height = (calendarEvent.End.Value - calendarEvent.Start.Value).TotalMinutes / Time.MinutesInAnHour / Time.HoursInADay * 1200,
                        Event = calendarEvent
                    };
                    
                    CalendarEvents.Add(viewModel);
                }

                _eventAggregator.GetEvent<SpinnerEvent>().Publish(EventResources.SpinnerEnum.Hide);
            }
            catch (Exception e)
            {
                _eventAggregator.GetEvent<SpinnerEvent>().Publish(EventResources.SpinnerEnum.Hide);
                _logger.Error(e, "Error orcurred while getting event data");

            }
        }

    }

    public class CalendarEventsBackground : BindableBase
    {
        private double _height;
        private double _locX;
        private double _locY;
        private Brush _color;

        public double Height
        {
            get => _height;
            set => SetProperty(ref _height, value);
        }

        public double LocX
        {
            get => _locX;
            set => SetProperty(ref _locX, value);
        }

        public double LocY
        {
            get => _locY;
            set => SetProperty(ref _locY, value);
        }

        public Brush Color
        {
            get => _color;
            set => SetProperty(ref _color, value);
        }
    }
}
