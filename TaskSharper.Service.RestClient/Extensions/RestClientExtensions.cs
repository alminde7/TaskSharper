﻿using System;
using System.Linq;
using System.Threading.Tasks;
using RestSharp;
using Serilog;
using Serilog.Context;
using TaskSharper.Shared.Constants;

namespace TaskSharper.Service.RestClient.Extensions
{
    public static class RestClientExtensions
    {
        public static async Task<IRestResponse<T>> ExecuteTaskAsync<T>(this IRestClient client, IRestRequest request,
            ILogger logger)
        {
            // Log request
            using (LogContext.PushProperty(HttpConstants.Header_CorrelationId, request.GetCorrelationId()))
            {
                LogRequest(client, request, logger);

                var response = await client.ExecuteTaskAsync<T>(request);

                // log response
                LogResponse(client, response, logger);

                return response;
            }
        }

        public static async Task<IRestResponse> ExecuteTaskAsync(this IRestClient client, IRestRequest request,
            ILogger logger)
        {
            // Log request
            using (LogContext.PushProperty(HttpConstants.Header_CorrelationId, request.GetCorrelationId()))
            {
                LogRequest(client, request, logger);

                var response = await client.ExecuteTaskAsync(request);

                // log response
                LogResponse(client, response, logger);

                return response;
            }
        }

        private static void LogRequest(IRestClient client, IRestRequest request, ILogger logger)
        {
            logger.Information("Request {@RequestedResource} type {@HttpMethod}, with parameters {@HttpParameters} and contenttype {@ContentType}", client.BaseUrl + request.Resource, request.Method.ToString(), request.Parameters, request.JsonSerializer.ContentType);
        }

        private static void LogResponse(IRestClient client, IRestResponse response, ILogger logger)
        {
            logger.Information("Response {@RequestedResource} with code {@StatusCode}", client.BaseUrl + response.Request.Resource, response.StatusCode.ToString());
        }
    }
}