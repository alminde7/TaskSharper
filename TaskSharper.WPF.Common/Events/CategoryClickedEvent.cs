﻿using Prism.Events;
using TaskSharper.Domain.Calendar;

namespace TaskSharper.WPF.Common.Events
{
    public class CategoryClickedEvent : PubSubEvent<EventCategory>
    {
    }
}
