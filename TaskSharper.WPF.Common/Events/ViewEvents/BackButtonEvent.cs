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
