using System;
using RestSharp;
using Http = TaskSharper.Shared.Constants.Http;

namespace TaskSharper.Service.RestClient.Extensions
{
    public static class RestClientExtensions
    {
        public static IRestRequest AddCorrelationId(this IRestRequest request)
        {
            request.AddHeader(Http.Header_CorrelationId, Guid.NewGuid().ToString());
            return request;
        }
    }
}