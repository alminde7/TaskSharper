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
    /// <summary>
    /// Factory that creates RestRequests
    /// </summary>
    public class RestRequestFactory : IRestRequestFactory
    {
        /// <summary>
        /// Create a RestRequest with added correlation id
        /// </summary>
        /// <param name="path"></param>
        /// <param name="method"></param>
        /// <returns></returns>
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
