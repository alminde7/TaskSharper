using System;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using TaskSharper.Domain.Calendar;

namespace TaskSharper.Calender.WPF.ViewModels
{
    public class CalendarEventViewModel : BindableBase
    {
        private bool _isTitleAndDescriptionActivated;
        private Event _event;
        

        public int TimeOfDay { get; set; }

        public Event Event
        {
            get => _event;
            set
            {
                if (value?.Start != null && value.End.HasValue)
                {
                    if (value.Start.Value.Hour < TimeOfDay)
                    {
                        IsTitleAndDescriptionActivated = false;
                    }
                }
                SetProperty(ref _event, value);
            }
        }

        public bool IsTitleAndDescriptionActivated
        {
            get => _isTitleAndDescriptionActivated;
            set => SetProperty(ref _isTitleAndDescriptionActivated, value);
        }
        
        public CalendarEventViewModel(int timeOfDay)
        {
            TimeOfDay = timeOfDay;
            IsTitleAndDescriptionActivated = true;
        }
    }
}
