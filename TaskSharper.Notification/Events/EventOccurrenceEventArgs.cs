using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskSharper.Domain.Calendar;

namespace TaskSharper.Notification.Events
{
    public class EventOccurrenceEventArgs : EventArgs
    {
        public Event Event { get; set; } 
    }
}
