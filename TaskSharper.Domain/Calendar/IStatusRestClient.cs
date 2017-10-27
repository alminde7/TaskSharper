using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TaskSharper.Domain.Calendar
{
    public interface IStatusRestClient
    {
        Task<bool> IsAliveAsync();
    }
}
