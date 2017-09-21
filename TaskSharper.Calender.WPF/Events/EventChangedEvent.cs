using Prism.Events;
using TaskSharper.Domain.Calendar;

namespace TaskSharper.Calender.WPF.Events
{
    public class EventChangedEvent : PubSubEvent<Event>
    {
    }
}
