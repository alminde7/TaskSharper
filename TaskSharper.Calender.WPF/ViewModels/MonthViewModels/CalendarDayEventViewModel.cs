using Prism.Mvvm;
using TaskSharper.Domain.Calendar;
using TaskSharper.Domain.Models;

namespace TaskSharper.Calender.WPF.ViewModels
{
    /// <summary>
    /// This is the ViewModel of the view, that is inside the CalendarDateDayView. 
    /// </summary>
    public class CalendarDayEventViewModel : BindableBase
    {
        private Event _event;
        private string _viewText;


        /// <summary>
        /// When the Event is set there will be removed traling zero from houres and minutes
        /// So the correct ViewText can be displayed. 
        /// </summary>
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
