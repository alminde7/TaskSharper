﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskSharper.Calender.WPF.ViewComponents
{
    public class CalendarTasksViewModel
    {
        IEnumerable<TaskViewModel> Tasks { get; set; }
    }
}
