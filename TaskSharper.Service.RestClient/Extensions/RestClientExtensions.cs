using System;
using System.Linq;
using System.Threading.Tasks;
using RestSharp;
using Serilog;
using Serilog.Context;
using TaskSharper.Shared.Constants;

namespace TaskSharper.Service.RestClient.Extensions
{
    /// <summary>
    /// Extension methods to IRestClient
    /// </summary>
    public static class RestClientExtensions
    {
        /// <summary>
        /// Executes rest request, and log request and response
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="client"></param>
        /// <param name="request"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static async Task<IRestResponse<T>> ExecuteTaskAsync<T>(this IRestClient client, IRestRequest request,
            ILogger logger)
        {
            using (LogContext.PushProperty(HttpConstants.Header_CorrelationId, request.GetCorrelationId()))
            {
                LogRequest(client, request, logger);

                var response = await client.ExecuteTaskAsync<T>(request);
                
                LogResponse(client, response, logger);

                return response;
            }
        }

        /// <summary>
        /// Executes rest request, and log request and response
        /// </summary>
        /// <param name="client"></param>
        /// <param name="request"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static async Task<IRestResponse> ExecuteTaskAsync(this IRestClient client, IRestRequest request,
            ILogger logger)
        {
            using (LogContext.PushProperty(HttpConstants.Header_CorrelationId, request.GetCorrelationId()))
            {
                LogRequest(client, request, logger);

                var response = await client.ExecuteTaskAsync(request);
                
                LogResponse(client, response, logger);

                return response;
            }
        }

        /// <summary>
        /// Helper method to log request
        /// </summary>
        /// <param name="client"></param>
        /// <param name="request"></param>
        /// <param name="logger"></param>
        private static void LogRequest(IRestClient client, IRestRequest request, ILogger logger)
        {
            logger.Information("Request {@RequestedResource} type {@HttpMethod}, with parameters {@HttpParameters} and contenttype {@ContentType}", client.BaseUrl + request.Resource, request.Method.ToString(), request.Parameters, request.JsonSerializer.ContentType);
        }

        /// <summary>
        /// Helper method to log response
        /// </summary>
        /// <param name="client"></param>
        /// <param name="response"></param>
        /// <param name="logger"></param>
        private static void LogResponse(IRestClient client, IRestResponse response, ILogger logger)
        {
            logger.Information("Response {@RequestedResource} with code {@StatusCode}", client.BaseUrl + response.Request.Resource, response.StatusCode.ToString());
        }
    }
}