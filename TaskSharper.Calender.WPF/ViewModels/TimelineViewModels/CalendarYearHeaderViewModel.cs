using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskSharper.Calender.WPF.ViewModels
{
    public class CalendarYearHeaderViewModel
    {
        public int Year { get; set; }
        public string Month { get; set; }
        public int WeekNumber { get; set; }

        public CalendarYearHeaderViewModel()
        {
            var date = DateTime.Now;

            var dateCultureInfo = DateTimeFormatInfo.CurrentInfo;

            Year = date.Year;
            Month = dateCultureInfo?.GetMonthName(date.Month);
            WeekNumber = dateCultureInfo.Calendar.GetWeekOfYear(date, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday);
        }
    }
}
