﻿using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using Prism.Events;
using Prism.Mvvm;
using Serilog;
using TaskSharper.Calender.WPF.Events;
using TaskSharper.Calender.WPF.Events.Resources;

namespace TaskSharper.Calender.WPF.ViewModels
{
    public class CalendarDateViewModel : BindableBase
    {
        private readonly CalendarTypeEnum _dateType;
        private readonly ILogger _logger;
        private string _dayOfWeek;
        private int _dayOfMonth;
        private DateTime _currentDate;

        public string DayOfWeek
        {
            get => _dayOfWeek;
            set => SetProperty(ref _dayOfWeek, value);
        }

        public int DayOfMonth
        {
            get => _dayOfMonth;
            set => SetProperty(ref _dayOfMonth, value);
        }

        public DateTime CurrentDate
        {
            get => _currentDate;
            set
            {
                DayOfWeek = CurrentCulture.TextInfo.ToTitleCase(DateCultureInfo.GetDayName(value.DayOfWeek));
                DayOfMonth = value.Day;
                _currentDate = value;
            }
        }

        public DateTimeFormatInfo DateCultureInfo { get; set; }
        public CultureInfo CurrentCulture { get; set; }

        public CalendarDateViewModel(DateTime date, IEventAggregator eventAggregator, CalendarTypeEnum dateType, ILogger logger)
        {
            _dateType = dateType;
            _logger = logger;
            // Initialization
            CurrentCulture = CultureInfo.CurrentCulture;
            DateCultureInfo = DateTimeFormatInfo.CurrentInfo;
            CurrentDate = date;

            // Event subscription
            eventAggregator.GetEvent<DayChangedEvent>().Subscribe(DayChangedEventHandler);
            eventAggregator.GetEvent<WeekChangedEvent>().Subscribe(WeekChangedEventHandler);
            eventAggregator.GetEvent<MonthChangedEvent>().Subscribe(MonthChangedEventHandler);
        }

        private void MonthChangedEventHandler(DateChangedEnum state)
        {
            if (_dateType != CalendarTypeEnum.Month) return;
            switch (state)
            {
                case DateChangedEnum.Increase:
                    CurrentDate = CurrentDate.AddMonths(1);
                    break;
                case DateChangedEnum.Decrease:
                    CurrentDate = CurrentDate.AddMonths(-1);
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
                    CurrentDate = CurrentDate.AddDays(7);
                    break;
                case DateChangedEnum.Decrease:
                    CurrentDate = CurrentDate.AddDays(-7);
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
                    CurrentDate = CurrentDate.AddDays(1);
                    break;
                case DateChangedEnum.Decrease:
                    CurrentDate = CurrentDate.AddDays(-1);
                    break;
                case DateChangeEnum.IncreaseMonth:
                    CurrentDate = CurrentDate.AddMonths(1);
                    break;
                case DateChangeEnum.DecreaseMonth:
                    CurrentDate = CurrentDate.AddMonths(-1);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }
    }
}
