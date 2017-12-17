using System.Collections.ObjectModel;
using System.Windows.Media;
using Prism.Mvvm;

namespace TaskSharper.Calender.WPF.ViewModels
{
    /// <summary>
    /// Is the ViewModel behind the CalendarTimeLineView, it is holding 24 elements of CalendarTimeViewModel's.
    /// </summary>
    public class CalendarTimelineViewModel : BindableBase
    {
        public ObservableCollection<CalendarTimeViewModel> Timeline { get; set; }
        

        /// <summary>
        /// The constructor
        /// </summary>
        public CalendarTimelineViewModel()
        {
            Timeline = new ObservableCollection<CalendarTimeViewModel>();
            InitializeView();
        }

        /// <summary>
        /// The foreach loop will indicate the time from 00:00 to 24:00. When the number is even the coler will change, 
        /// so it is easyer to see the differance between the elements. 
        /// </summary>
        private void InitializeView()
        {
            for (int i = 0; i < 24; i++)
            {
                Timeline.Add(new CalendarTimeViewModel()
                {
                    Hour = i,
                    BackgroundColor = i % 2 != 0 ? Brushes.AliceBlue : Brushes.White
                });
            }
        }
    }
}
