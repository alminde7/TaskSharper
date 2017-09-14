using System;
using Prism.Commands;
using Prism.Mvvm;
using TaskSharper.Domain.Calendar;

namespace TaskSharper.Calender.WPF.ViewModels
{
    public class CalendarEventViewModel : BindableBase
    {
        private string _title;
        private string _description;
        private DateTime _start;
        private DateTime _end;
        private Event.EventType _type;
        private bool _isTitleAndDescriptionActivated;

        #region Non-binding properties
        public string Id { get; set; }
        public int TimeOfDay { get; set; }
        #endregion

        #region Binding properties
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
                if (value.Hour < TimeOfDay)
                {
                    IsTitleAndDescriptionActivated = false;
                }
                SetProperty(ref _start, value);
            }
        }

        public DateTime End
        {
            get => _end;
            set => SetProperty(ref _end, value);
        }

        public Event.EventType Type
        {
            get => _type;
            set => SetProperty(ref _type, value);
        }

        public bool IsTitleAndDescriptionActivated
        {
            get => _isTitleAndDescriptionActivated;
            set => SetProperty(ref _isTitleAndDescriptionActivated, value);
        }

        #endregion
        
        public CalendarEventViewModel(int timeOfDay)
        {
            TimeOfDay = timeOfDay;
            IsTitleAndDescriptionActivated = true;
        }
    }
}
