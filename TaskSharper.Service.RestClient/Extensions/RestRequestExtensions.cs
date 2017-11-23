using System;
using System.Linq;
using RestSharp;
using TaskSharper.Shared.Constants;

namespace TaskSharper.Service.RestClient.Extensions
{
    public static class RestRequestExtensions
    {
        public static IRestRequest AddCorrelationId(this IRestRequest request)
        {
            request.AddHeader(HttpConstants.Header_CorrelationId, Guid.NewGuid().ToString());
            return request;
        }

        public static string GetCorrelationId(this IRestRequest request)
        {
            var id = request.Parameters.FirstOrDefault(x => x.Name == HttpConstants.Header_CorrelationId)?.Value as string;
            return string.IsNullOrWhiteSpace(id) ? Guid.NewGuid().ToString() : id;
        }
    }
}
