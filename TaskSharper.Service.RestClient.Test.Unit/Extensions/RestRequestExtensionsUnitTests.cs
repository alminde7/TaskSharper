using System.Linq;
using NUnit.Framework;
using RestSharp;
using TaskSharper.Service.RestClient.Extensions;
using TaskSharper.Shared.Constants;

namespace TaskSharper.Service.RestClient.Test.Unit.Extensions
{
    [TestFixture]
    public class RestRequestExtensionsUnitTests
    {
        [Test]
        public void AddCorrelationId_CorrelationIdHeaderHasBeenAddedToRequest()
        {
            IRestRequest request = new RestRequest("", Method.GET);

            request.AddCorrelationId();

            var id = request.Parameters.FirstOrDefault(x => x.Name == HttpConstants.Header_CorrelationId)?.Value as string;

            Assert.NotNull(id);
        }

        [Test]
        public void AddCorrelationId_NotCalled_NoCorrelationIdAdded()
        {
            IRestRequest request = new RestRequest("", Method.GET);
            
            var id = request.Parameters.FirstOrDefault(x => x.Name == HttpConstants.Header_CorrelationId)?.Value as string;

            Assert.Null(id);
        }
    }
}
