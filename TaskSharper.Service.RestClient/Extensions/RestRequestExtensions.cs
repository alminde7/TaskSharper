using System;
using System.Linq;
using RestSharp;
using TaskSharper.Shared.Constants;

namespace TaskSharper.Service.RestClient.Extensions
{
    /// <summary>
    /// Extension methods to IRestRequest
    /// </summary>
    public static class RestRequestExtensions
    {
        /// <summary>
        /// Add correlation id to rest request
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static IRestRequest AddCorrelationId(this IRestRequest request)
        {
            request.AddHeader(HttpConstants.Header_CorrelationId, Guid.NewGuid().ToString());
            return request;
        }

        /// <summary>
        /// Retrieves coorelation id from a rest request
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string GetCorrelationId(this IRestRequest request)
        {
            var id = request.Parameters.FirstOrDefault(x => x.Name == HttpConstants.Header_CorrelationId)?.Value as string;
            return string.IsNullOrWhiteSpace(id) ? Guid.NewGuid().ToString() : id;
        }
    }
}
