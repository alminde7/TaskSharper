using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskSharper.Calender.WPF.ViewComponents
{
    public class CalendarAppointmentsViewModel
    {
        IEnumerable<CalendarAppointmentsViewModel> Appointments { get; set; }
    }
}
