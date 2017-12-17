using System.Windows.Media;
using Prism.Mvvm;

namespace TaskSharper.Calender.WPF.ViewModels
{
    /// <summary>
    /// This is the ViewModel for the CalendarTimeView 
    /// The propertys of this ViewModel is set from the CalendarTimelineViewModel
    /// </summary>
    public class CalendarTimeViewModel : BindableBase
    {
        private int _hour;
        private Brush _backgroundColor;

        public int Hour
        {
            get => _hour;
            set => SetProperty(ref _hour, value);
        }

        /// <summary>
        /// A system.media Brush type that get visualized in the View. 
        /// </summary>
        public Brush BackgroundColor
        {
            get => _backgroundColor;
            set => SetProperty(ref _backgroundColor, value);
        }
    }
}
