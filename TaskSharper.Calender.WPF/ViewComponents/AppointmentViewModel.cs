using System;

namespace TaskSharper.Calender.WPF.ViewComponents
{
    internal class AppointmentViewModel
    {
        public string Headline { get; set; }
        public string Description { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }
}