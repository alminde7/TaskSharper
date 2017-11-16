using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using System.Web;
using RestSharp;
using Serilog;
using Serilog.Context;
using TaskSharper.Domain.Calendar;
using TaskSharper.Domain.RestDTO;
using TaskSharper.Service.RestClient.Extensions;
using TaskSharper.Service.RestClient.Factories;
using TaskSharper.Shared.Constants;
using TaskSharper.Shared.Exceptions;
using TaskSharper.Shared.Extensions;

namespace TaskSharper.Service.RestClient.Clients
{
    public class EventRestClient : IAppointmentRestClient, ITaskRestClient
    {
        private readonly IRestClient _restClient;
        private readonly IRestRequestFactory _requestFactory;
        private readonly ILogger _logger;
        private const string BaseUrl = "http://localhost:8000/api/";
        private readonly string Controller;

        public EventRestClient(string controller, IRestClient restClient, IRestRequestFactory requestFactory, ILogger logger)
        {
            Controller = controller;
            _restClient = restClient;
            _requestFactory = requestFactory;
            _logger = logger.ForContext<EventRestClient>();
            _restClient.BaseUrl = new Uri(BaseUrl);
        }

        public Event Get(string id)
        {
            var request = _requestFactory.Create($"{Controller}/{id}", Method.GET);

            var result = _restClient.Execute<Event>(request);

            return CreateResponse(result);
        }

        public async Task<Event> GetAsync(string id)
        {
            var request = _requestFactory.Create($"{Controller}/{id}", Method.GET);
            
            var result = await _restClient.ExecuteTaskAsync<Event>(request, _logger);
            
            return CreateResponse(result);
        }

        public async Task<IEnumerable<Event>> GetAsync(DateTime date)
        {
            var requestDateStart = date.StartOfDay();
            var requestDateEnd = date.EndOfDay();

            var request = _requestFactory.Create(Controller, Method.GET);
            request.AddQueryParameter("from", requestDateStart.ToString(CultureInfo.InvariantCulture));
            request.AddQueryParameter("to", requestDateEnd.ToString(CultureInfo.InvariantCulture));

            var result = await _restClient.ExecuteTaskAsync<List<Event>>(request, _logger);

            return CreateResponse(result);
        }

        public async Task<IEnumerable<Event>> GetAsync(DateTime from, DateTime to)
        {
            var request = _requestFactory.Create(Controller, Method.GET);
            request.AddQueryParameter("from", from.ToString(CultureInfo.InvariantCulture));
            request.AddQueryParameter("to", to.ToString(CultureInfo.InvariantCulture));

            var result = await _restClient.ExecuteTaskAsync<List<Event>>(request, _logger);

            return CreateResponse(result);
        }

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

            var request = _requestFactory.Create(Controller, Method.POST);
            request.AddJsonBody(eventDto);

            var result = await _restClient.ExecuteTaskAsync<Event>(request);

            return CreateResponse(result);
        }

        public async Task<Event> UpdateAsync(Event updatedEvent)
        {
            var request = _requestFactory.Create(Controller, Method.PUT);
            request.AddJsonBody(updatedEvent);

            var result = await _restClient.ExecuteTaskAsync<Event>(request);

            return result.Data;
        }

        public async Task DeleteAsync(string id, string calendarId)
        {
            var request = _requestFactory.Create($"{Controller}/{id}?calendarId={calendarId}", Method.DELETE);

            var result = await _restClient.ExecuteTaskAsync(request);

            CreateResponse(result);
        }

        public async Task<IEnumerable<EventCategory>> GetAsync()
        {
            var request = _requestFactory.Create("Categories", Method.GET);
            var result = await _restClient.ExecuteTaskAsync<List<EventCategory>>(request, _logger);

            return CreateResponse(result);

        }

        private T CreateResponse<T>(IRestResponse<T> response)
        {
            //TODO:: Seek a better solution for this - maybe create an enricher
            using (LogContext.PushProperty(HttpConstants.Header_CorrelationId, response.Request.GetCorrelationId()))
            {
                var statusCode = (int)response.StatusCode;

                switch (statusCode)
                {
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

        private void CreateResponse(IRestResponse response)
        {
            //TODO:: Seek a better solution for this - maybe create an enricher
            using (LogContext.PushProperty(HttpConstants.Header_CorrelationId, response.Request.GetCorrelationId()))
            {
                var statusCode = (int)response.StatusCode;

                switch (statusCode)
                {
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
