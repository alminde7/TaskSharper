﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Events;
using TaskSharper.Domain.Calendar;

namespace TaskSharper.Tasks.WPF.Events
{
    public class TaskSelectedEvent : PubSubEvent<Event>
    {
    }
}
