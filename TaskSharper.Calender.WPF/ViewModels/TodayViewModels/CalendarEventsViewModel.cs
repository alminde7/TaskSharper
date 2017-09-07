using System.Collections.ObjectModel;

namespace TaskSharper.Calender.WPF.ViewModels
{
    public class CalendarEventsViewModel 
    {
        public ObservableCollection<CalendarEventViewModel> CalendarEvents { get; set; }

        public CalendarEventsViewModel()
        {
            CalendarEvents = new ObservableCollection<CalendarEventViewModel>();
            InitializeView();
        }

        private void InitializeView()
        {
            for (int i = 0; i < 24; i++)
            {
                CalendarEvents.Add(new CalendarEventViewModel()
                {
                    Title = "yoyo",
                    Description = "Jeppe"
                });
            }
        }
    }
}
