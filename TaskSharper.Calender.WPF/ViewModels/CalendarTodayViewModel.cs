﻿using System;
using System.Windows.Input;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using TaskSharper.BusinessLayer;
using TaskSharper.Calender.WPF.Events;
using TaskSharper.Calender.WPF.Events.Resources;
using TaskSharper.Domain.BusinessLayer;
using TaskSharper.Domain.Calendar;

namespace TaskSharper.Calender.WPF.ViewModels
{
    public class CalendarTodayViewModel : BindableBase
    {
        public IEventAggregator EventAggregator { get; }
        public IEventManager CalendarService { get; }

        public CalendarEventsViewModel EventsViewModel { get; set; }
        public CalendarDateViewModel DateViewModel { get; set; }
        public DateTime CurrentDay { get; set; }

        public ICommand NextCommand { get; set; }
        public ICommand PrevCommand { get; set; }

        public CalendarTodayViewModel(IEventAggregator eventAggregator, IEventManager calendarService)
        {
            EventAggregator = eventAggregator;
            CalendarService = calendarService;
            CurrentDay = DateTime.Now;

            // Initialize views
            EventsViewModel = new CalendarEventsViewModel(CurrentDay, eventAggregator, calendarService);
            DateViewModel = new CalendarDateViewModel(CurrentDay, eventAggregator);

            // Initialize commands
            NextCommand = new DelegateCommand(NextDayCommandHandler);
            PrevCommand = new DelegateCommand(PreviousDayCommandHandler);
        }

        public void NextDayCommandHandler()
        {
            CurrentDay = CurrentDay.AddDays(1);
            EventAggregator.GetEvent<DateChangedEvent>().Publish(DateChangeEnum.IncreaseDay);
        }

        public void PreviousDayCommandHandler()
        {
            CurrentDay = CurrentDay.AddDays(-1);
            EventAggregator.GetEvent<DateChangedEvent>().Publish(DateChangeEnum.DecreaseDay);
        }
    }
}
