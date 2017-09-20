using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskSharper.Domain.Calendar;

namespace TaskSharper.Calender.WPF.ViewModels
{
    public class CalendarDayEventViewModel : BindableBase
    {
        private Event _event;

        public Event Event
        {
            get => _event;
            set { SetProperty(ref _event, value); }
        }

        public CalendarDayEventViewModel()
        {

        }
    }
}
