using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using TaskSharper.Domain.BusinessLayer;
using TaskSharper.Domain.Cache;
using TaskSharper.Domain.Calendar;

namespace TaskSharper.BusinessLayer.Unit.Test
{
    [TestFixture]
    public class EventManagerUnitTests
    {
        private IEventManager _uut;
        private ICalendarService _calendarService;
        private ICacheStore _cache;


        [SetUp]
        public void Setup()
        {
            _calendarService = Substitute.For<ICalendarService>();
            _cache = Substitute.For<ICacheStore>();

            _uut = new EventManager(_calendarService, _cache);
        }

        [TearDown]
        public void TearDown()
        {
            
        }

        [Test]
        public void Constructor_CalendarServiceAndCacheHasBeenInitialized()
        {
            _calendarService.Received(1);
            _cache.Received(1);
        }

        [Test]
        public void GetEvent_GetEventThatIsInTheCache_EventIsReturned()
        {
            var id = "123";
            var calEvent = new Event()
            {
                Id = "123",
                Description = "123"
            };

            _cache.GetEvent(Arg.Is(id)).Returns(calEvent);

            Assert.That(_uut.GetEvent(id), Is.EqualTo(calEvent));
            _calendarService.DidNotReceive().GetEvent(Arg.Any<string>(), Arg.Any<string>());
        }

        [Test]
        public void GetEvent_CacheReturnNullCalendarServiceIsCalled_CalendarServiceHasBeenCalledOnce()
        {
            var id = "123";
            Event calEvent = null;

            _cache.GetEvent(Arg.Any<string>()).Returns(calEvent);

            _uut.GetEvent(id);

            _calendarService.Received(1).GetEvent(Arg.Any<string>(), Arg.Any<string>());
        }

        [Test]
        public void GetEvent_CacheReturnNullCalendarServiceIsCalled_CacheIsUpdated()
        {
            var id = "123";
            Event calEvent = null;

            _cache.GetEvent(Arg.Any<string>()).Returns(calEvent);

            _uut.GetEvent(id);

            _calendarService.Received(1).GetEvent(Arg.Any<string>(), Arg.Any<string>());
            _cache.Received(1).AddOrUpdateEvent(Arg.Any<Event>());
        }

    }
}
