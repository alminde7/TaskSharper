using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Events;

namespace TaskSharper.WPF.Common.Events.ViewEvents
{
    public class BackButtonEvent : PubSubEvent<BackButtonStatus>
    {
    }

    public enum BackButtonStatus
    {
        Show,
        Hide
    }
}
