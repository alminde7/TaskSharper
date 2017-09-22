using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Events;
using TaskSharper.Domain.Calendar;

namespace TaskSharper.Calender.WPF.Events
{
    public class EventClickedEvent : PubSubEvent<EventClickObject>
    {
    }

    public class EventClickObject
    {
        public Event Event { get; set; }
        public int? TimeOfDay { get; set; }
    }
}
