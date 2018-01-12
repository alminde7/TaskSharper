using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using System.Web;
using RestSharp;
using Serilog;
using Serilog.Context;
using TaskSharper.Domain.Models;
using TaskSharper.Domain.RestClient;
using TaskSharper.Domain.RestDTO;
using TaskSharper.Service.RestClient.Extensions;
using TaskSharper.Service.RestClient.Factories;
using TaskSharper.Shared.Constants;
using TaskSharper.Shared.Exceptions;
using TaskSharper.Shared.Extensions;

namespace TaskSharper.Service.RestClient.Clients
{
    /// <summary>
    /// Used to handle request to Rest API in windows service.
    /// </summary>
    public class EventRestClient : IAppointmentRestClient, ITaskRestClient
    {
        private readonly IRestClient _restClient;
        private readonly IRestRequestFactory _requestFactory;
        private readonly ILogger _logger;
        private readonly string _controller;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serverUrl">Url to Rest endpoints</param>
        /// <param name="controller">Name of rest controller</param>
        /// <param name="restClient">RestClient used to execute requests</param>
        /// <param name="requestFactory">RestRequestFactory used to create rest requests</param>
        /// <param name="logger"></param>
        public EventRestClient(string serverUrl, string controller, IRestClient restClient, IRestRequestFactory requestFactory, ILogger logger)
        {
            _controller = controller;
            _restClient = restClient;
            _requestFactory = requestFactory;
            _logger = logger.ForContext<EventRestClient>();
            _restClient.BaseUrl = new Uri(serverUrl);
        }

        /// <summary>
        /// Get event by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Event Get(string id, string calendarId)
        {
            var request = _requestFactory.Create($"{_controller}/{id}", Method.GET);

            request.AddQueryParameter("calendarId", calendarId);

            var result = _restClient.Execute<Event>(request);

            return CreateResponse(result);
        }

        /// <summary>
        /// Get event by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Event> GetAsync(string id, string calendarId)
        {
            var request = _requestFactory.Create($"{_controller}/{id}", Method.GET);
            request.AddQueryParameter("calendarId", calendarId);

            var result = await _restClient.ExecuteTaskAsync<Event>(request, _logger);
            
            return CreateResponse(result);
        }

        /// <summary>
        /// Get event by date
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Event>> GetAsync(DateTime date)
        {
            var requestDateStart = date.StartOfDay();
            var requestDateEnd = date.EndOfDay();

            var request = _requestFactory.Create(_controller, Method.GET);
            request.AddQueryParameter("from", requestDateStart.ToString(CultureInfo.InvariantCulture));
            request.AddQueryParameter("to", requestDateEnd.ToString(CultureInfo.InvariantCulture));

            var result = await _restClient.ExecuteTaskAsync<List<Event>>(request, _logger);

            return CreateResponse(result);
        }

        /// <summary>
        /// Get event by date
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Event>> GetAsync(DateTime from, DateTime to)
        {
            var request = _requestFactory.Create(_controller, Method.GET);
            request.AddQueryParameter("from", from.ToString(CultureInfo.InvariantCulture));
            request.AddQueryParameter("to", to.ToString(CultureInfo.InvariantCulture));

            var result = await _restClient.ExecuteTaskAsync<List<Event>>(request, _logger);

            return CreateResponse(result);
        }

        /// <summary>
        /// Create new event
        /// </summary>
        /// <param name="newEvent"></param>
        /// <returns></returns>
        public async Task<Event> CreateAsync(Event newEvent)
        {
            var eventDto = new EventDto()
            {
                Title = newEvent.Title,
                Description = newEvent.Description,
                EventStatus = newEvent.Status,
                EventType = newEvent.Type,
                Start = newEvent.Start.Value,
                End = newEvent.End.Value,
                EventCategory = newEvent.Category
            };

            var request = _requestFactory.Create(_controller, Method.POST);
            request.AddJsonBody(eventDto);

            var result = await _restClient.ExecuteTaskAsync<Event>(request);

            return CreateResponse(result);
        }

        /// <summary>
        /// Update existing event
        /// </summary>
        /// <param name="updatedEvent"></param>
        /// <returns></returns>
        public async Task<Event> UpdateAsync(Event updatedEvent)
        {
            var request = _requestFactory.Create(_controller, Method.PUT);
            request.AddJsonBody(updatedEvent);

            var result = await _restClient.ExecuteTaskAsync<Event>(request);

            return result.Data;
        }

        /// <summary>
        /// Delete event
        /// </summary>
        /// <param name="id"></param>
        /// <param name="calendarId"></param>
        /// <returns></returns>
        public async Task DeleteAsync(string id, string calendarId)
        {
            var request = _requestFactory.Create($"{_controller}/{id}?calendarId={calendarId}", Method.DELETE);

            var result = await _restClient.ExecuteTaskAsync(request);

            CreateResponse(result);
        }

        /// <summary>
        /// Get categories
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<EventCategory>> GetAsync()
        {
            var request = _requestFactory.Create("Categories", Method.GET);
            var result = await _restClient.ExecuteTaskAsync<List<EventCategory>>(request, _logger);

            return CreateResponse(result);

        }

        /// <summary>
        /// Creates response to caller based on response from Rest API. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="response"></param>
        /// <returns></returns>
        private T CreateResponse<T>(IRestResponse<T> response)
        {
            //TODO:: Seek a better solution for this - maybe create an enricher
            using (LogContext.PushProperty(HttpConstants.Header_CorrelationId, response.Request.GetCorrelationId()))
            {
                var statusCode = (int)response.StatusCode;

                switch (statusCode)
                {
                    case 401:
                        throw new UnauthorizedAccessException(response.Content);
                    case 599:
                        throw new ConnectionException("No internetconnection");
                }

                if (statusCode >= 400 && statusCode < 500) // User did something wrong
                {
                    var exception = new ArgumentException(response.ErrorMessage);
                    _logger.Error(exception, "Request faild");
                    throw exception;
                }
                else if (statusCode >= 500) // Application did something wrong
                {
                    var exception = new HttpException(response.ErrorMessage);
                    _logger.Error(exception, "Request faild");
                    throw exception;
                }
                else // All good
                {
                    return response.Data;
                }
            }
        }

        /// <summary>
        /// Creates response to caller based on response from Rest API. 
        /// </summary>
        /// <param name="response"></param>
        private void CreateResponse(IRestResponse response)
        {
            //TODO:: Seek a better solution for this - maybe create an enricher
            using (LogContext.PushProperty(HttpConstants.Header_CorrelationId, response.Request.GetCorrelationId()))
            {
                var statusCode = (int)response.StatusCode;

                switch (statusCode)
                {
                    case 401:
                        throw new UnauthorizedAccessException(response.Content);
                    case 599:
                        throw new ConnectionException("No internetconnection");
                }

                if (statusCode >= 400 && statusCode < 500) // User did something wrong
                {
                    var exception = new ArgumentException(response.ErrorMessage);
                    _logger.Error(exception, "Request faild");
                    throw exception;
                }
                else if (statusCode >= 500) // Application did something wrong
                {
                    var exception = new HttpException(response.ErrorMessage);
                    _logger.Error(exception, "Request faild");
                    throw exception;
                }
            }
        }
    }
}
