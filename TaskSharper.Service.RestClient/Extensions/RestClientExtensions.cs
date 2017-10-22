using System;
using System.Threading.Tasks;
using RestSharp;
using Serilog;
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

        public static async Task<IRestResponse<T>> ExecuteTaskAsync<T>(this IRestClient client, IRestRequest request,
            ILogger logger)
        {
            // Log request

            var response = await client.ExecuteTaskAsync<T>(request);

            // log response

            return response;
        }

        public static async Task<IRestResponse> ExecuteTaskAsync(this IRestClient client, IRestRequest request,
            ILogger logger)
        {
            // Log request

            var response = await client.ExecuteTaskAsync(request);

            // log response

            return response;
        }
    }
}