using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using RestSharp;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using TaskSharper.Service.RestClient.Extensions;

namespace TaskSharper.Service.RestClient.Test.Unit.Extensions
{
    [TestFixture]
    public class RestClientExtensionsUnitTests
    {
        private ILogger _logger;
        private IRestRequest _restRequest;
        private IRestClient _restClient;

        [SetUp]
        public void Setup()
        {
            var conf = new LoggerConfiguration().CreateLogger();
            _logger = conf;
            _restRequest = Substitute.For<IRestRequest>();
            _restClient = new RestSharp.RestClient();
        }
    }
}
