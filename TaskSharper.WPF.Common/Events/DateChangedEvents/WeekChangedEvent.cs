using Prism.Events;
using TaskSharper.WPF.Common.Events.Resources;

namespace TaskSharper.WPF.Common.Events.DateChangedEvents
{
    public class WeekChangedEvent : PubSubEvent<DateChangedEnum>
    {
    }
}