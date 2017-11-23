using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http.Results;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;
using TaskSharper.Domain.BusinessLayer;
using TaskSharper.Service.Controllers;
using Serilog;
using TaskSharper.Domain.Calendar;
using TaskSharper.Domain.RestDTO;

namespace TaskSharper.Service.Test.Unit.Controllers
{
    [TestFixture]
    public class EventsControllerUnitTests
    {
        private ILogger _logger;
        private IEventManager _eventManager;
        private EventsController _uut;

        [SetUp]
        public void Setup()
        {
            _logger = Substitute.For<ILogger>();
            _eventManager = Substitute.For<IEventManager>();
            _uut = new EventsController(_eventManager, _logger);
        }

        [Test]
        public void Constructor_LoggerHasBeenInitialized()
        {
            Assert.NotNull(_uut.Logger);
        }


        #region GetById

        [Test]
        public async Task GetWithId_NoIdSupplied_Return400()
        {
            string id = null;

            var result = await _uut.Get(id);

            Assert.That(result, Is.TypeOf<BadRequestErrorMessageResult>());
        }

        [Test]
        public async Task GetWithId_EmptyIdSupplied_Return400()
        {
            string id = "";

            var result = await _uut.Get(id);

            Assert.That(result, Is.TypeOf<BadRequestErrorMessageResult>());
        }

        [Test]
        public async Task GetWithId_ValidIdSupplied_Return200()
        {
            string id = "1";

            var result = await _uut.Get(id);

            Assert.That(result, Is.TypeOf<OkNegotiatedContentResult<Event>>());
        }

        [Test]
        public async Task GetWithId_HttpRequestExceptionThrown_HttpStatus599IsReturned()
        {
            _eventManager.GetEventAsync(Arg.Any<string>()).Throws<HttpRequestException>();

            string id = "1";

            var result = await _uut.Get(id);

            var data = result as NegotiatedContentResult<HttpRequestException>;

            Assert.That(data?.StatusCode, Is.EqualTo((HttpStatusCode)599));
        }

        [Test]
        public async Task GetWithId_ExceptionIsThrown_HttpStatus599IsReturned()
        {
            _eventManager.GetEventAsync(Arg.Any<string>()).Throws<Exception>();

            string id = "1";

            var result = await _uut.Get(id);

            var data = result as NegotiatedContentResult<string>;

            Assert.That(data?.StatusCode, Is.EqualTo((HttpStatusCode)500));
        }

        #endregion

        #region GetByDates

        [Test]
        public async Task GetWithBetweenDates_InvalidDatesSupplied_Return400()
        {
            DateTime date1 = new DateTime(2017, 5, 15);
            DateTime date2 = new DateTime(2017, 5, 14);

            var result = await _uut.Get(date1, date2);

            Assert.That(result, Is.TypeOf<BadRequestErrorMessageResult>());
        }
        
        [Test]
        public async Task GetWithBetweenDates_DatesWithSameStartAndEnd_Return400()
        {
            DateTime date1 = new DateTime(2017, 5, 14);
            DateTime date2 = new DateTime(2017, 5, 14);

            var result = await _uut.Get(date1, date2);

            Assert.That(result, Is.TypeOf<BadRequestErrorMessageResult>());
        }

        [Test]
        public async Task GetWithBetweenDates_ValidTimeSpan_ReturnListOfEvents()
        {
            DateTime date1 = new DateTime(2017, 5, 14);
            DateTime date2 = new DateTime(2017, 5, 15);

            var result = await _uut.Get(date1, date2);

            Assert.That(result, Is.TypeOf<OkNegotiatedContentResult<IList<Event>>>());
        }

        [Test]
        public async Task GetWithBetweenDates_HttpRequestExceptionIsThrown_HttpStatus599IsReturned()
        {
            _eventManager.GetEventsAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>()).Throws<HttpRequestException>();

            DateTime date1 = new DateTime(2017, 5, 14);
            DateTime date2 = new DateTime(2017, 5, 15);

            var result = await _uut.Get(date1, date2);

            var data = result as NegotiatedContentResult<HttpRequestException>;

            Assert.That(data?.StatusCode, Is.EqualTo((HttpStatusCode)599));
        }

        [Test]
        public async Task GetWithBetweenDates_ExceptionIsThrown_HttpStatus599IsReturned()
        {
            _eventManager.GetEventsAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>()).Throws<Exception>();

            DateTime date1 = new DateTime(2017, 5, 14);
            DateTime date2 = new DateTime(2017, 5, 15);

            var result = await _uut.Get(date1, date2);

            var data = result as NegotiatedContentResult<string>;

            Assert.That(data?.StatusCode, Is.EqualTo((HttpStatusCode)500));
        }

        #endregion

        #region Post

        [Test]
        public async Task Post_EventWithInvalidTimespan_Return400()
        {
            var data = new EventDto()
            {
                Title = "The title",
                Start = new DateTime(2017, 5, 15),
                End = new DateTime(2017, 5, 14)
            };

            var result = await _uut.Post(data);

            Assert.That(result, Is.TypeOf<BadRequestErrorMessageResult>());
        }

        [Test]
        public async Task Post_EventWithNullTitle_Return400()
        {
            var data = new EventDto()
            {
                Title = null,
                Start = new DateTime(2017, 5, 14),
                End = new DateTime(2017, 5, 15)
            };

            var result = await _uut.Post(data);

            Assert.That(result, Is.TypeOf<BadRequestErrorMessageResult>());
        }

        [Test]
        public async Task Post_EventWithEmptyTitle_Return400()
        {
            var data = new EventDto()
            {
                Title = "",
                Start = new DateTime(2017, 5, 14),
                End = new DateTime(2017, 5, 15)
            };

            var result = await _uut.Post(data);

            Assert.That(result, Is.TypeOf<BadRequestErrorMessageResult>());
        }

        [Test]
        public async Task Post_EventWithWhitespaceTitle_Return400()
        {
            var data = new EventDto()
            {
                Title = "  ",
                Start = new DateTime(2017, 5, 14),
                End = new DateTime(2017, 5, 15)
            };

            var result = await _uut.Post(data);

            Assert.That(result, Is.TypeOf<BadRequestErrorMessageResult>());
        }

        [Test]
        public async Task Post_ThrowHttpRequestException_Returns599()
        {
            _eventManager.CreateEventAsync(Arg.Any<Event>()).Throws<HttpRequestException>();

            var data = new EventDto()
            {
                Title = "123",
                Start = new DateTime(2017, 5, 14),
                End = new DateTime(2017, 5, 15)
            };

            var result = await _uut.Post(data);

            var resultData = result as NegotiatedContentResult<HttpRequestException>;

            Assert.That(resultData?.StatusCode, Is.EqualTo((HttpStatusCode)599));
        }

        [Test]
        public async Task Post_ThrowException_Returns500()
        {
            _eventManager.CreateEventAsync(Arg.Any<Event>()).Throws<Exception>();

            var data = new EventDto()
            {
                Title = "123",
                Start = new DateTime(2017, 5, 14),
                End = new DateTime(2017, 5, 15)
            };

            var result = await _uut.Post(data);

            var resultData = result as NegotiatedContentResult<string>;

            Assert.That(resultData?.StatusCode, Is.EqualTo((HttpStatusCode)500));
        }

        [Test]
        public async Task Post_ValidTimespanAndValidTitle_ReturnCreatedWithAnEventAsContent()
        {
            _eventManager.CreateEventAsync(Arg.Any<Event>()).Returns(new Event());

            var data = new EventDto()
            {
                Title = "123",
                Start = new DateTime(2017, 5, 14),
                End = new DateTime(2017, 5, 15)
            };

            var result = await _uut.Post(data);

            Assert.That(result, Is.TypeOf<CreatedNegotiatedContentResult<Event>>());
        }

        #endregion

        #region Put

        [Test]
        public async Task Put_ThrowHttpRequestException_Returns599()
        {
            _eventManager.UpdateEventAsync(Arg.Any<Event>()).Throws<HttpRequestException>();

            var inputData = new Event();

            var result = await _uut.Put(inputData);

            var data = result as NegotiatedContentResult<HttpRequestException>;

            Assert.That(data?.StatusCode, Is.EqualTo((HttpStatusCode)599));
        }

        [Test]
        public async Task Put_ThrowsException_Returns500()
        {
            _eventManager.UpdateEventAsync(Arg.Any<Event>()).Throws<Exception>();

            var inputData = new Event();

            var result = await _uut.Put(inputData);

            var data = result as NegotiatedContentResult<string>;

            Assert.That(data?.StatusCode, Is.EqualTo((HttpStatusCode)500));
        }

        [Test]
        public async Task Put_ValidEvent_CreatedResultWithEventTypeIsReturned()
        {
            _eventManager.UpdateEventAsync(Arg.Any<Event>()).Returns(new Event());

            var inputData = new Event();

            var result = await _uut.Put(inputData);

            Assert.That(result, Is.TypeOf<CreatedNegotiatedContentResult<Event>>());
        }

        #endregion

        #region Delete

        [Test]
        public async Task Delete_IdIsNull_Return400()
        {
            string id = null;
            string calendarId = "123";

            var result = await _uut.Delete(id, calendarId);

            Assert.That(result, Is.TypeOf<BadRequestErrorMessageResult>());
        }

        [Test]
        public async Task Delete_IdIsEmpty_Return400()
        {
            string id = "";
            string calendarId = "123";

            var result = await _uut.Delete(id, calendarId);

            Assert.That(result, Is.TypeOf<BadRequestErrorMessageResult>());
        }

        [Test]
        public async Task Delete_CalendarIdIsNull_Return400()
        {
            string id = "123";
            string calendarId = null;

            var result = await _uut.Delete(id, calendarId);

            Assert.That(result, Is.TypeOf<BadRequestErrorMessageResult>());
        }

        [Test]
        public async Task Delete_CalendarIdIsEmpty_Return400()
        {
            string id = "123";
            string calendarId = "";

            var result = await _uut.Delete(id, calendarId);

            Assert.That(result, Is.TypeOf<BadRequestErrorMessageResult>());
        }

        [Test]
        public async Task Delete_ThrowsHttpRequestException_Return599()
        {
            _eventManager.DeleteEventAsync(Arg.Any<string>(), Arg.Any<string>()).Throws<HttpRequestException>();

            string id = "123";
            string calendarId = "123";

            var result = await _uut.Delete(id, calendarId);

            var data = result as NegotiatedContentResult<HttpRequestException>;

            Assert.That(data?.StatusCode, Is.EqualTo((HttpStatusCode)599));
        }

        [Test]
        public async Task Delete_ThrowsException_Return500()
        {
            _eventManager.DeleteEventAsync(Arg.Any<string>(), Arg.Any<string>()).Throws<Exception>();

            string id = "123";
            string calendarId = "123";

            var result = await _uut.Delete(id, calendarId);

            var data = result as NegotiatedContentResult<string>;

            Assert.That(data?.StatusCode, Is.EqualTo((HttpStatusCode)500));
        }

        [Test]
        public async Task Delete_ValidIdAndCalendarId_Return200()
        {
            string id = "123";
            string calendarId = "123";

            var result = await _uut.Delete(id, calendarId);

            Assert.That(result, Is.TypeOf<OkResult>());
        }

        #endregion

    }
}
