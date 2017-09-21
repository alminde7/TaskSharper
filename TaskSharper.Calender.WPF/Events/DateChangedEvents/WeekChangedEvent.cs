using Prism.Events;
using TaskSharper.Calender.WPF.Events.Resources;

namespace TaskSharper.Calender.WPF.Events
{
    public class WeekChangedEvent : PubSubEvent<DateChangedEnum>
    {
    }
}