﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Events;
using TaskSharper.Domain.Calendar;
using TaskSharper.Domain.Models;

namespace TaskSharper.Appointments.WPF.Events
{
    public class AppointmentSelectedEvent : PubSubEvent<Event>
    {
    }
}
