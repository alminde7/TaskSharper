using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;

namespace TaskSharper.Calender.WPF.ViewModels
{
    public class CalendarWeekViewModel : BindableBase
    {
        private const int DAYS_IN_WEEK = 7;

        public ObservableCollection<CalendarDateViewModel> DateHeaders { get; set; }
        public ObservableCollection<CalendarEventsViewModel> EventContainers { get; set; }

        public CalendarWeekViewModel()
        {
            DateHeaders = new ObservableCollection<CalendarDateViewModel>();
            EventContainers = new ObservableCollection<CalendarEventsViewModel>();
            InitializeView();
        }

        private void InitializeView()
        {
            for (int i = 1; i <= DAYS_IN_WEEK; i++)
            {
                DateHeaders.Add(new CalendarDateViewModel(i%DAYS_IN_WEEK));
                EventContainers.Add(new CalendarEventsViewModel());
            }
        }
    }
}
