using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using TaskSharper.Service.RestClient.Extensions;

namespace TaskSharper.Service.RestClient.Factories
{
    // http://www.hackered.co.uk/articles/restsharp-and-the-factory-pattern-you-really-should
    public class RestRequestFactory : IRestRequestFactory
    {
        public IRestRequest Create(string path, Method method)
        {
            // Create request
            var request = new RestRequest(path, method);

            // Bootstrap request headers etc.
            request.AddCorrelationId();

            // Return request object
            return request;
        }
    }
}
