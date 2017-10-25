using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;

namespace TaskSharper.Service.RestClient.Factories
{
    public class RestClientFactory
    {
        // NOTE:: Not in use
        public IRestClient Create(Uri url, string controller)
        {
            var restClient = new RestSharp.RestClient(url + "/" + controller);
            return restClient;
        }
    }
}
