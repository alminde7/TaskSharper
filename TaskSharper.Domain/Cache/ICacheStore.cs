using System;

namespace TaskSharper.Domain.Cache
{
    public interface ICacheStore
    {
        TimeSpan UpdatedOffset { get; set; }
    }
}
