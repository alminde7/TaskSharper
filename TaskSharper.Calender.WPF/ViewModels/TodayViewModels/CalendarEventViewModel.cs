using System;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

namespace TaskSharper.Calender.WPF.ViewModels
{
    public class CalendarEventViewModel : BindableBase
    {
        private readonly IRegionManager _regionManager;

        private string _title;
        private string _description;
        private bool _hasAppointment;
        private bool _hasTask;
        private DateTime _start;
        private DateTime _end;
        public DelegateCommand EventClickCommand { get; set; }
        

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        public DateTime Start
        {
            get => _start;
            set
            {
                HasAppointment = true;
                if (value.Hour < TimeOfDay)
                {
                    Title = "";
                    Description = "";
                }
                SetProperty(ref _start, value);
            }
        }

        public DateTime End
        {
            get => _end;
            set => SetProperty(ref _end, value);
        }

        public bool HasAppointment
        {
            get => _hasAppointment;
            set => SetProperty(ref _hasAppointment, value);
        }

        public bool HasTask
        {
            get => _hasTask;
            set => SetProperty(ref _hasTask, value);
        }

        public int TimeOfDay { get; set; }


        public CalendarEventViewModel(int timeOfDay, IRegionManager regionManager)
        {
            TimeOfDay = timeOfDay;
            _regionManager = regionManager;
            EventClickCommand = new DelegateCommand(EventClick, CanExecute);
        }

        private void Navigate(string uri)
        {
            _regionManager.RequestNavigate("CalendarRegion", uri);
        }

        private void EventClick()
        {
            Navigate("CalendarEventDetailsView");
        }

        private bool CanExecute()
        {
            return !string.IsNullOrEmpty(Title);
        }
    }
}
