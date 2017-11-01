using System;
using System.Collections.Generic;
using NSubstitute;
using NUnit.Framework;
using Serilog;
using TaskSharper.Domain.Calendar;

namespace TaskSharper.CacheStore.Test.Unit
{
    [TestFixture]
    public class EventCacheUnitTests
    {
        private EventCache _uut;
        private ILogger _logger;

        [SetUp]
        public void Setup()
        {
            _logger = Substitute.For<ILogger>();
            _uut = new EventCache(_logger);
        }

        [TearDown]
        public void TearDown()
        {
            
        }

        [Test]
        public void Constructor_EventsDictionaryHasBeenInitialized()
        {
            Assert.NotNull(_uut.Events);
        }

        [Test]
        public void Constructor_EventsContainerInitialized_EventsContains0Elements()
        {
            Assert.That(_uut.Events.Count, Is.EqualTo(0));
        }

        [Test]
        public void HasData_NotData_ReturnFalse()
        {
            Assert.False(_uut.HasData(DateTime.Now.Date));
        }

        [Test]
        public void HasData_InsertDataInDate03032017_ReturnTrueWhenAskedFor03032017()
        {
            var date = new DateTime(2017, 3, 3);
            _uut.Events.TryAdd(date, new Dictionary<string, CacheData>());

            Assert.True(_uut.HasData(date));
        }

        [Test]
        public void HasEvent_WithDate_DoesNotContainDate_ReturnFalse()
        {
            string id = "123";
            
            Assert.False(_uut.HasEvent(id, DateTime.Now));
        }

        [Test]
        public void HasEvent_WithDate_DoesContainDateButNotEvent_ReturnFalse()
        {
            string id = "123";
            string fakeId = "321";
            var date = new DateTime(2017,3,3);
            var dic = new Dictionary<string, CacheData> {{id, new CacheData(new Event(), DateTime.Now, false)}};

            _uut.Events.TryAdd(date, dic);

            Assert.False(_uut.HasEvent(fakeId, date));
        }

        [Test]
        public void HasEvent_WithDate_DoesContainDateAndEvent_ReturnTrue()
        {
            string id = "123";
            var date = new DateTime(2017, 3, 3);
            var dic = new Dictionary<string, CacheData> { { id, new CacheData(new Event(), DateTime.Now, false) } };

            _uut.Events.TryAdd(date, dic);

            Assert.True(_uut.HasEvent(id, date));
        }

        [Test]
        public void HasEvent_DoesNotContainEvent_ReturnFalse()
        {
            Assert.False(_uut.HasEvent("123"));
        }

        [Test]
        public void HasEvent_DoesContainEvent_ReturnTrue()
        {
            string id = "123";
            var date = new DateTime(2017, 3, 3);
            var dic = new Dictionary<string, CacheData> { { id, new CacheData(new Event(), DateTime.Now, false) } };

            _uut.Events.TryAdd(date, dic);

            Assert.True(_uut.HasEvent(id));
        }

        [Test]
        public void UpdateCacheStore_EmptyListOfEventsIsSuppliedDateFromAndDateToHasATimespanOf5Days_CountOfEventsContainerIs6()
        {
            var timeInt = 5;
            var list = new List<Event>();

            var timespan = TimeSpan.FromDays(timeInt);
            var startDate = new DateTime(2017,3,3);
            var endDate = startDate + timespan;

            _uut.UpdateCacheStore(list, startDate, endDate);

            Assert.That(_uut.Events.Count, Is.EqualTo(6));
        }

        [Test]
        public void UpdateCacheStore_EmptyListOfEventsIsSuppliedDateFromAndDateToHasATimespanOf5Days_NoActualEventsIsAddedToTheEventContainer()
        {
            var timeInt = 5;
            var list = new List<Event>();

            var timespan = TimeSpan.FromDays(timeInt);
            var startDate = new DateTime(2017, 3, 3);
            var endDate = startDate + timespan;

            _uut.UpdateCacheStore(list, startDate, endDate);

            foreach (var uutEvent in _uut.Events)
            {
                Assert.That(uutEvent.Value.Count, Is.EqualTo(0));
            }
        }

        [Test]
        public void UpdateCacheStore_AddListWithEmptyEventsOnlySpecifyingStartDate_CountOfEventsContainerIs1()
        {
            var list = new List<Event>();
            var date = new DateTime(2017,3,3);

            _uut.UpdateCacheStore(list, date, null);

            Assert.That(_uut.Events.Count, Is.EqualTo(1));
        }

        [Test]
        public void UpdateCacheStore_AddListWithOneEventOnOneDate_EventContainerHasEventWithSpecifiedId()
        {
            string id = "123";
            var start = new DateTime(2017, 3, 3);
            var end = new DateTime(2017, 3, 3).AddHours(2);

            var list = new List<Event>()
            {
                new Event()
                {
                    Start = start,
                    End = end,
                    Id = id
                }
            };

            _uut.UpdateCacheStore(list, start, end);

            Assert.True(_uut.HasEvent(id));
        }

        [Test]
        public void UpdateCacheStore_AddListWithOneEventWithNoId_ThrowsArgumentNullException()
        {
            var date = new DateTime(2017, 3, 3);
            var list = new List<Event>()
            {
                new Event()
                {
                    Start = date,
                }
            };

            Assert.Throws<InvalidOperationException>(()=> _uut.UpdateCacheStore(list, date, null));
        }

        [Test]
        public void UpdateCacheStore_AddListWithOneEventOnOneDate_LastUpdatedHasBeenUpdated()
        {
            var updated = _uut.LastUpdated;
            string id = "123";
            var start = new DateTime(2017, 3, 3);
            var end = new DateTime(2017, 3, 3).AddHours(2);
            var list = new List<Event>()
            {
                new Event()
                {
                    Start = start,
                    End = end,
                    Id = id
                }
            };

            _uut.UpdateCacheStore(list, start, end);

            Assert.AreNotEqual(updated, _uut.LastUpdated);
        }

        [Test]
        public void UpdateCacheStore_CallTwiceWithEventsWithSameId_ContentOfEventHasBeenUpdated()
        {
            string id = "123";
            var d1 = "The first description";
            var d2 = "The second description";
            var start = new DateTime(2017, 3, 3);
            var end = new DateTime(2017, 3, 3).AddHours(2);
            var firstList = new List<Event>()
            {
                new Event()
                {
                    Start = start,
                    End = end,
                    Description = d1,
                    Id = id
                }
            };

            _uut.UpdateCacheStore(firstList, start, end);

            var secondList = new List<Event>()
            {
                new Event()
                {
                    Start = start,
                    End = end,
                    Description = d2,
                    Id = id
                }
            };

            _uut.UpdateCacheStore(secondList, start, end);

            Assert.That(_uut.GetEvent(id).Description, Is.EqualTo(d2));
        }

        [Test]
        public void GetEvents_DateNotCached_ReturnsNull()
        {
            var date = new DateTime(2017,3,3);
            Assert.Null(_uut.GetEvents(date));
        }

        [Test]
        public void GetEvents_NoEventsOnDate_ReturnsEmptyList()
        {
            var date = new DateTime(2017, 3, 3);

            var list = new List<Event>();
            _uut.UpdateCacheStore(list, date, null);

            Assert.That(_uut.GetEvents(date).Count, Is.EqualTo(0));
        }

        [Test]
        public void GetEvents_3EventsAddedToDate_ReturnsListWith3Items()
        {
            var start = new DateTime(2017, 3, 3);
            var end = new DateTime(2017, 3, 3).AddHours(2);

            var list = new List<Event>()
            {
                new Event()
                {
                    Start = start,
                    End = end,
                    Id = "123"
                },
                new Event()
                {
                    Start = start,
                    End = end,
                    Id = "234"
                },
                new Event()
                {
                    Start = start,
                    End = end,
                    Id = "345"
                }
            };

            _uut.UpdateCacheStore(list, start, end);

            Assert.That(_uut.GetEvents(start).Count, Is.EqualTo(3));
        }

        [Test]
        public void GetEvents_3EventsAddedToDateWithOnly1UniqueId_ReturnsListWith1Item()
        {
            var start = new DateTime(2017, 3, 3);
            var end = new DateTime(2017, 3, 3).AddHours(2);

            var list = new List<Event>()
            {
                new Event()
                {
                    Start = start,
                    End = end,
                    Id = "123"
                },
                new Event()
                {
                    Start = start,
                    End = end,
                    Id = "123"
                },
                new Event()
                {
                    Start = start,
                    End = end,
                    Id = "123"
                }
            };

            _uut.UpdateCacheStore(list, start, end);

            Assert.That(_uut.GetEvents(start).Count, Is.EqualTo(1));
        }

        [Test]
        public void GetEvents_WithEndDate_NoDataFoundBetweenGivenDates_ReturnNull()
        {
            var date1 = new DateTime(2017,3,3);
            var date2 = new DateTime(2017,8,8);

            Assert.Null(_uut.GetEvents(date1, date2));
        }

        [Test]
        public void GetEvents_WithEndDate_ThereIs3ElementsBetweenGivenDates_ListWith3ElementsIsReturned()
        {
            var date1 = new DateTime(2017, 3, 3);
            var date2 = new DateTime(2017, 8, 8);

            var list = new List<Event>();

            for (int i = 0; i < 3; i++)
            {
                list.Add(new Event()
                {
                    Id = i.ToString(),
                    Start = date1.AddDays(i),
                    End = date1.AddDays(i).AddHours(2)
                });
            }

            _uut.UpdateCacheStore(list, date1, date2);

            Assert.That(_uut.GetEvents(date1, date2).Count, Is.EqualTo(3));
        }

        [Test]
        public void GetEvents_WithEndDate_10ElementsInCacheOnly2BetweenGivenDates_ListWith2ElementsIsReturned()
        {
            var date1 = new DateTime(2017, 3, 3);
            var date2 = new DateTime(2017, 3, 8);
            var startDate = new DateTime(2017, 3, 16);
            var list = new List<Event>();

            for (int i = 0; i < 10; i++)
            {
                list.Add(new Event()
                {
                    Id = i.ToString(),
                    Start = startDate.Date.AddDays(-i),
                    End = startDate.Date.AddDays(-i).AddHours(2)
                });
            }

            _uut.UpdateCacheStore(list, date1, startDate);

            Assert.That(_uut.GetEvents(date1, date2).Count, Is.EqualTo(2));
        }

        [Test]
        public void GetEvent_WithDate_NoDateInCache_ReturnNull()
        {
            var id = "123";
            var date = new DateTime(2017,3,3);

            Assert.Null(_uut.GetEvent(id, date));
        }

        [Test]
        public void GetEvent_WithDate_NoEventOnDate_ReturnNull()
        {
            var id = "123";
            var date = new DateTime(2017, 3, 3);

            var list = new List<Event>();

            _uut.UpdateCacheStore(list, date, null);

            Assert.Null(_uut.GetEvent(id, date));
        }

        [Test]
        public void GetEvent_WithDate_EventAdded_ReturnTheSpecifiedEvent()
        {
            var id = "123";
            var date = new DateTime(2017, 3, 3);
            var calEvent = new Event()
            {
                Id = id,
                Start = date,
                End = date.AddHours(2)
            };

            var list = new List<Event>()
            {
                calEvent
            };

            _uut.UpdateCacheStore(list, date, null);

            Assert.That(_uut.GetEvent(id, date).Id, Is.EqualTo(id));
        }

        [Test]
        public void GetEvent_WithDate_EventAddedWithIdOtherThanProvidedId_ReturnNull()
        {
            var id = "123";
            var date = new DateTime(2017, 3, 3);
            var calEvent = new Event()
            {
                Id = "321",
                Start = date,
                End = date.AddHours(2)
            };

            var list = new List<Event>()
            {
                calEvent
            };

            _uut.UpdateCacheStore(list, date, null);

            Assert.Null(_uut.GetEvent(id, date));
        }

        [Test]
        public void GetEvent_NoEventWithIdInCache_ReturnNull()
        {
            Assert.Null(_uut.GetEvent("123"));
        }

        [Test]
        public void GetEvent_EventAddedWithMatchingId_EventReturnedWithId()
        {
            var id = "123";
            var date = new DateTime(2017, 3, 3);
            var calEvent = new Event()
            {
                Id = id,
                Start = date,
                End = date.AddHours(2)
            };

            var list = new List<Event>()
            {
                calEvent
            };

            _uut.UpdateCacheStore(list, date, null);

            Assert.That(_uut.GetEvent(id).Id, Is.EqualTo(id));
        }

        [Test]
        public void AddOrUpdateUpdateEvent_EventNotAddedToCache_EventHasBeenAdded()
        {
            var id = "123";
            var date = new DateTime(2017,3,3);
            var d1 = "Description 1";

            var calEvent = new Event()
            {
                Id = id,
                Start = date,
                Description = d1
            };

            _uut.AddOrUpdateEvent(calEvent);

            Assert.That(_uut.GetEvent(id), Is.EqualTo(calEvent));
        }

        [Test]
        public void AddOrUpdateUpdateEvent_EventAddedAndUpdated_EventHasBeenUpdated()
        {
            var id = "123";
            var date = new DateTime(2017, 3, 3);
            var d1 = "Description 1";
            var d2 = "Description 2";

            var calEvent = new Event()
            {
                Id = id,
                Start = date,
                Description = d1
            };

            _uut.AddOrUpdateEvent(calEvent);

            calEvent.Description = d2;

            _uut.AddOrUpdateEvent(calEvent);

            Assert.That(_uut.GetEvent(id).Description, Is.EqualTo(d2));
        }
    }
}
