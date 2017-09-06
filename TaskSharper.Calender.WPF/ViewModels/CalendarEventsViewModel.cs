using System.Collections.Generic;

namespace TaskSharper.Calender.WPF.ViewModels
{
    public class CalendarEventsViewModel
    {
        IEnumerable<CalendarEventsViewModel> Appointments { get; set; }
    }
}
