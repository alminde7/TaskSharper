using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Serilog;

namespace TaskSharper.Service.Attributes
{
    public class Log : ActionFilterAttribute
    {
        private readonly ILogger _logger = Serilog.Log.Logger;
        private readonly Stopwatch _watch = new Stopwatch();
        
        public override async Task OnActionExecutingAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            _watch.Start();
            var httpMethod = actionContext.Request.Method.Method;
            var path = actionContext.Request.RequestUri.AbsolutePath;


            _logger.Information("Request Path:{Path}, HttpMethod:{HttpMethod}", path, httpMethod);

             base.OnActionExecuting(actionContext);
        }

        public override async Task OnActionExecutedAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
        {
            string body = string.Empty;
            var statusCode = actionExecutedContext.Response.StatusCode;
            var path = actionExecutedContext.Request.RequestUri.AbsolutePath;
            if (actionExecutedContext.Response.Content != null)
            {
                body = await actionExecutedContext.Response.Content.ReadAsStringAsync();
            }

            _watch.Stop();
            if ((int)statusCode >= 400 && (int)statusCode < 500)
            {
                _logger.Warning("Request ended with responseCode {ResponseCode} and responseBody:{ResponseBody}. Request took {Elapsed} ms", (int)statusCode, body, _watch.Elapsed.TotalMilliseconds);
            }
            else if ((int)statusCode >= 500)
            {
                _logger.Error("Request ended with responseCode {ResponseCode} and responseBody:{ResponseBody}. Request took {Elapsed} ms", (int)statusCode, body, _watch.Elapsed.TotalMilliseconds);
            }
            else
            {
                _logger.Information("Request ended with responseCode {ResponseCode}. Request took {Elapsed} ms", (int)statusCode, _watch.Elapsed.TotalMilliseconds);
            }

            if (actionExecutedContext.Exception != null)
            {
                var requestBody = string.Empty;
                if (actionExecutedContext.Request.Content != null)
                {
                    requestBody = await actionExecutedContext.Request.Content.ReadAsStringAsync();
                }
                _logger.Error(actionExecutedContext.Exception, "An error occurred while processing {Path}. RequestBody:{RequestBody}", path, requestBody);
            }
            _watch.Reset();
            base.OnActionExecuted(actionExecutedContext);
        }
    }
}
