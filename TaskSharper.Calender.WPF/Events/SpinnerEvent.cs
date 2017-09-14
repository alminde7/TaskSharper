using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Events;
using TaskSharper.Calender.WPF.Events.Resources;
using TaskSharper.Calender.WPF.ViewModels;

namespace TaskSharper.Calender.WPF.Events
{
    
    public class SpinnerEvent : PubSubEvent<EventResources.SpinnerEnum>
    {
    }
}
