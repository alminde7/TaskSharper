﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskSharper.Domain.Configuration
{
    public interface ICacheConfiguration
    {
        TimeSpan AllowedOldData { get; set; }
    }
}
