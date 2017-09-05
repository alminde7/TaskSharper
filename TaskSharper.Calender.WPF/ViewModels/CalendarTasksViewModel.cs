using System.Collections.Generic;

namespace TaskSharper.Calender.WPF.ViewModels
{
    public class CalendarTasksViewModel
    {
        IEnumerable<TaskViewModel> Tasks { get; set; }
    }
}
