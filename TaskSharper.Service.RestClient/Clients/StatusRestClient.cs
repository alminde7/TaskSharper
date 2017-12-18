using System;
using System.Net;
using System.Threading.Tasks;
using RestSharp;
using Serilog;
using TaskSharper.Domain.RestClient;
using TaskSharper.Service.RestClient.Extensions;
using TaskSharper.Service.RestClient.Factories;

namespace TaskSharper.Service.RestClient.Clients
{
    /// <summary>
    /// Used to ensure that rest API is alive
    /// </summary>
    public class StatusRestClient : IStatusRestClient
    {
        private readonly IRestClient _restClient;
        private readonly IRestRequestFactory _requestFactory;
        private readonly ILogger _logger;
        private const string Controller = "status";

        public StatusRestClient(string serverUrl, IRestClient restClient, IRestRequestFactory requestFactory, ILogger logger)
        {
            _restClient = restClient;
            _requestFactory = requestFactory;
            _logger = logger.ForContext<EventRestClient>();
            _restClient.BaseUrl = new Uri(serverUrl);
        }

        /// <summary>
        /// Send request to server, if it return 200 the methods return true. Otherwise return false.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> IsAliveAsync()
        {
            var request = _requestFactory.Create(Controller, Method.GET);

            var result = await _restClient.ExecuteTaskAsync<HttpStatusCode>(request, _logger);
            
            if (result.StatusCode == HttpStatusCode.OK)
                return true;

            return false;
        }
    }
}
