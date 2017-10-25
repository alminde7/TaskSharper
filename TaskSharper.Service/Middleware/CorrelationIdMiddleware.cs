using System;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;
using Microsoft.Owin;
using TaskSharper.Shared.Constants;

namespace TaskSharper.Service.Middleware
{
    public class CorrelationIdMiddleware : OwinMiddleware
    {
        public CorrelationIdMiddleware(OwinMiddleware next) : base(next)
        {
        }

        public override async Task Invoke(IOwinContext context)
        {
            try
            {
                if (context.Request.Headers.ContainsKey(HttpConstants.Header_CorrelationId))
                {
                    var id = context.Request.Headers.Get(HttpConstants.Header_CorrelationId);
                    CallContext.LogicalSetData(HttpConstants.Header_CorrelationId, id);
                }
                else
                {
                    CallContext.LogicalSetData(HttpConstants.Header_CorrelationId, Guid.NewGuid().ToString());
                }
            }
            catch (Exception )
            {
                // Do nothing - dont break the pipeline
            }
            
            await Next.Invoke(context);
        }
    }
}
