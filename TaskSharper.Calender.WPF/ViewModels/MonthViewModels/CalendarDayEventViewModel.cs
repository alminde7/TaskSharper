using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskSharper.Domain.Calendar;

namespace TaskSharper.Calender.WPF.ViewModels
{
    public class CalendarDayEventViewModel : BindableBase
    {
        private Event _event;
        private string _viewText;

        public Event Event
        {
            get => _event;
            set
            {
                SetProperty(ref _event, value);
                var hourTrailingZero = Event.Start.Value.Hour < 10 ? "0" : "";
                var minuteTrailingZero = Event.Start.Value.Minute < 10 ? "0" : "";
                ViewText = $"{hourTrailingZero}{Event.Start.Value.Hour}:{minuteTrailingZero}{Event.Start.Value.Minute} {Event.Title}";
            }
        }

        public string ViewText
        {
            get => _viewText;
            set => SetProperty(ref _viewText, value);
        }

    }
}
