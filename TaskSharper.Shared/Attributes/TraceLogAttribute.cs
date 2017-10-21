using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace TaskSharper.Shared.Attributes
{
    public class TraceLogAttribute : Attribute
    {
        public TraceLogAttribute()
        {
            CallContext.SetData("CorrelationId", Guid.NewGuid().ToString());
        }
    }
}
