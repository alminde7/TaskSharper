using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskSharper.Domain.Configuration
{
    public class CacheConfiguration : ICacheConfiguration
    {
        public TimeSpan AllowedOldData { get; set; } = TimeSpan.FromMinutes(5);
    }
}
