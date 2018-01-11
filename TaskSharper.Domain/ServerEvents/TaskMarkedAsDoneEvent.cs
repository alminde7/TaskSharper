using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskSharper.Domain.Models;

namespace TaskSharper.Domain.ServerEvents
{
    public class TaskMarkedAsDoneEvent
    {
        public Event Event { get; set; }
    }
}
