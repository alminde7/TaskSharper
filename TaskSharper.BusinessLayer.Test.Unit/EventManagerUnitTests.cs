using NSubstitute;
using NUnit.Framework;
using Serilog;
using TaskSharper.Domain.BusinessLayer;
using TaskSharper.Domain.Cache;
using TaskSharper.Domain.Calendar;
using TaskSharper.Domain.DataAccessLayer;
using TaskSharper.Domain.Notification;

namespace TaskSharper.BusinessLayer.Test.Unit
{
    [TestFixture]
    public class EventManagerUnitTests
    {
        private IEventManager _uut;
        private IEventRepository _eventRepository;
        private ICategoryRepository _categoryRepository;
        private IEventCache _eventCache;
        private IEventCategoryCache _eventCategoryCachce;
        private INotification _notification;
        private ILogger _logger;
        private INotificationPublisher _notificationPublisher;


        [SetUp]
        public void Setup()
        {
            _eventRepository = Substitute.For<IEventRepository>();
            _categoryRepository = Substitute.For<ICategoryRepository>();
            _eventCache = Substitute.For<IEventCache>();
            _eventCategoryCachce = Substitute.For<IEventCategoryCache>();
            _logger = Substitute.For<ILogger>();
            _notification = Substitute.For<INotification>();
            _notificationPublisher = Substitute.For<INotificationPublisher>();

            _uut = new EventManager(_eventRepository, _categoryRepository, _eventCategoryCachce, _eventCache, _notification, _notificationPublisher, _logger);
        }

        [TearDown]
        public void TearDown()
        {
            
        }

        [Test]
        public void Constructor_CalendarServiceAndCacheHasBeenInitialized()
        {
            _eventRepository.Received(1);
            _eventCache.Received(1);
        }

        //[Test]
        //public void GetEvent_GetEventThatIsInTheCache_EventIsReturned()
        //{
        //    var id = "123";
        //    var calEvent = new Event()
        //    {
        //        Id = "123",
        //        Description = "123"
        //    };

        //    _cache.GetEvent(Arg.Is(id)).Returns(calEvent);

        //    Assert.That(_uut.GetEvent(id), Is.EqualTo(calEvent));
        //    _calendarService.DidNotReceive().GetEvent(Arg.Any<string>(), Arg.Any<string>());
        //}

        //[Test]
        //public void GetEvent_CacheReturnNullCalendarServiceIsCalled_CalendarServiceHasBeenCalledOnce()
        //{
        //    var id = "123";
        //    Event calEvent = null;

        //    _cache.GetEvent(Arg.Any<string>()).Returns(calEvent);

        //    _uut.GetEvent(id);

        //    _calendarService.Received(1).GetEvent(Arg.Any<string>(), Arg.Any<string>());
        //}

        //[Test]
        //public void GetEvent_CacheReturnNullCalendarServiceIsCalled_CacheIsUpdated()
        //{
        //    var id = "123";
        //    Event calEvent = null;

        //    _cache.GetEvent(Arg.Any<string>()).Returns(calEvent);

        //    _uut.GetEvent(id);

        //    _calendarService.Received(1).GetEvent(Arg.Any<string>(), Arg.Any<string>());
        //    _cache.Received(1).AddOrUpdateEvent(Arg.Any<Event>());
        //}

    }
}
