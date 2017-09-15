using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Events;
using TaskSharper.Calender.WPF.Events.Resources;

namespace TaskSharper.Calender.WPF.Events
{
    public class WeekChangedEvent : PubSubEvent<ChangeWeekEnum>
    {
    }
}
