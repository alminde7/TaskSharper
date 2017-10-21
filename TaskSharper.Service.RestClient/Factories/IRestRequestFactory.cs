using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;

namespace TaskSharper.Service.RestClient.Factories
{
    public interface IRestRequestFactory
    {
        IRestRequest Create(string path, Method method);
    }
}
