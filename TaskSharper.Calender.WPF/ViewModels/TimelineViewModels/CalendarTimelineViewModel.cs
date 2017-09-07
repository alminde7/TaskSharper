using System.Collections.ObjectModel;
using System.Diagnostics;
using Prism.Mvvm;

namespace TaskSharper.Calender.WPF.ViewModels
{
    public class CalendarTimelineViewModel : BindableBase
    {
        public ObservableCollection<CalendarTimeViewModel> Timeline { get; set; }
        
        public CalendarTimelineViewModel()
        {
            Timeline = new ObservableCollection<CalendarTimeViewModel>();
            InitializeView();
        }

        private void InitializeView()
        {
            for (int i = 0; i < 24; i++)
            {
                Timeline.Add(new CalendarTimeViewModel()
                {
                    Hour = i
                });
            }
        }
    }
}
