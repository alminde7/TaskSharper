using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskSharper.Calender.WPF.ViewComponents
{
    public class CalendarDateViewModel
    {
        public string DayOfWeek { get; set; } = DateTime.Now.DayOfWeek.ToString();
        public int DayOfMonth { get; set; } = DateTime.Now.Day;
    }
}
