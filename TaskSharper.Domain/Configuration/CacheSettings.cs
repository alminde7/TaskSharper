using System;

namespace TaskSharper.Domain.Configuration
{
    public class CacheSettings
    {
        public bool EnableCache { get; set; } = true;
        public TimeSpan AllowedTimeInCache { get; set; } = TimeSpan.FromMinutes(5);
    }
}