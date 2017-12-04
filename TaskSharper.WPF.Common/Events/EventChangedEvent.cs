using Prism.Events;
using TaskSharper.Domain.Calendar;
using TaskSharper.Domain.Models;

namespace TaskSharper.WPF.Common.Events
{
    public class EventChangedEvent : PubSubEvent<Event>
    {
    }
}
