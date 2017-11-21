using System;
using System.Collections.Generic;
using NUnit.Framework;
using TaskSharper.Domain.Calendar;
using TaskSharper.Calender.WPF.Helpers.EventLocation;

namespace TaskSharper.Calendar.WPF.Test.Unit
{
    [TestFixture]
    public class EventLocationDataUnitTest
    {
        [Test]
        public void EventLocationDataUnitTestTest()
        {
            List<Event> events = new List<Event>();
            var event10 = new Event()
            {
                Start = new DateTime(2017, 11, 17, 1, 0, 0),
                End = new DateTime(2017, 11, 17, 3, 0, 0),
                Id = "1"
            };
            var event11 = new Event()
            {
                Start = new DateTime(2017, 11, 17, 3, 15, 0),
                End = new DateTime(2017, 11, 17, 10, 0, 0),
                Id = "3"
            };
            var event01 = new Event()
            {
                Start = new DateTime(2017, 11, 17, 2, 0, 0),
                End = new DateTime(2017, 11, 17, 14, 0, 0),
                Id = "2"
            };
            var event00 = new Event()
            {
                Start = new DateTime(2017, 11, 17, 1, 0, 0),
                End = new DateTime(2017, 11, 17, 2, 0, 0),
                Id = "5"
            };
            var event20 = new Event()
            {
                Start = new DateTime(2017, 11, 17, 4, 30, 0),
                End = new DateTime(2017, 11, 17, 7, 0, 0),
                Id = "14"
            };
            var event02 = new Event()
            {
                Start = new DateTime(2017, 11, 17, 14, 30, 0),
                End = new DateTime(2017, 11, 17, 23, 0, 0),
                Id = "4"
            };

            events.Add(event00);
            events.Add(event01);
            events.Add(event02);
            events.Add(event11);
            events.Add(event10);
            events.Add(event20);

            var result = EventLocation.FindLayout(events);

            Assert.That(result[0].Exists(i => i.Event.Id == event00.Id));
            Assert.That(result[0].Exists(i => i.Event.Id == event01.Id));
            Assert.That(result[0].Exists(i => i.Event.Id == event02.Id));
            Assert.That(result[1].Exists(i => i.Event.Id == event11.Id));
            Assert.That(result[1].Exists(i => i.Event.Id == event10.Id));
            Assert.That(result[2].Exists(i => i.Event.Id == event20.Id));
            Assert.That(result[0][0].ColumnSpan == 0);
            Assert.That(result[0][1].ColumnSpan == 0);
            Assert.That(result[0][2].ColumnSpan == 2);
            Assert.That(result[1][0].ColumnSpan == 1);
            Assert.That(result[1][1].ColumnSpan == 0);
            Assert.That(result[2][0].ColumnSpan == 0);

        }

        [Test]
        public void TwoSimultaneousEventsAndOneNotOverlapping()
        {
            List<Event> events = new List<Event>();
            var event1 = new Event()
            {
                Start = new DateTime(2017, 11, 24, 3, 0, 0),
                End = new DateTime(2017, 11, 24, 9, 0, 0),
                Id = "1"
            };
            var event2 = new Event()
            {
                Start = new DateTime(2017, 11, 24, 3, 30, 0),
                End = new DateTime(2017, 11, 24, 5, 0, 0),
                Id = "2"
            };
            var event3 = new Event()
            {
                Start = new DateTime(2017, 11, 24, 12, 0, 0),
                End = new DateTime(2017, 11, 24, 13, 0, 0),
                Id = "3"
            };
            events.Add(event1);
            events.Add(event2);
            events.Add(event3);

            var result = EventLocation.FindLayout(events);

            Assert.That(result[0].Exists(i => i.Event.Id == event1.Id));
            Assert.That(result[0].Exists(i => i.Event.Id == event3.Id));
            Assert.That(result[1].Exists(i => i.Event.Id == event2.Id));
            Assert.That(result[0][0].ColumnSpan == 0);
            Assert.That(result[1][0].ColumnSpan == 0);
            Assert.That(result[0][1].ColumnSpan == 1);
        }
    }

}
