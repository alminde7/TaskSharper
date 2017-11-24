using System.Linq;
using NUnit.Framework;
using RestSharp;
using TaskSharper.Service.RestClient.Factories;
using TaskSharper.Shared.Constants;

namespace TaskSharper.Service.RestClient.Test.Unit.Factories
{
    [TestFixture]
    public class RestRequestFactoryUnitTests
    {
        private IRestRequestFactory _uut;

        [SetUp]
        public void Setup()
        {
            _uut = new RestRequestFactory();
        }

        [Test]
        public void Create_CreateRequestWithApiEventsPathAndMethodGet_ReturnAnInstanceWithCorrectProperties()
        {
            var path = "api/events";

            var request = _uut.Create(path, Method.GET);

            Assert.That(request.Resource, Is.EqualTo(path));
            Assert.That(request.Method, Is.EqualTo(Method.GET));
        }

        [Test]
        public void Create_CreateRequestWithApiEventsPathAndMethodGet_CorrelationIdHasBeenAdded()
        {
            var path = "api/events";

            var request = _uut.Create(path, Method.GET);

            var id = request.Parameters.FirstOrDefault(x => x.Name == HttpConstants.Header_CorrelationId)?.Value as string;

            Assert.NotNull(id);
        }
    }
}
