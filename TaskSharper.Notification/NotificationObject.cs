﻿using System.Timers;

namespace TaskSharper.Notification
{
    public class NotificationObject
    {
        public Timer Timer { get; set; }
        public bool HasFired { get; set; }
    }
}