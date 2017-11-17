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
            events.Add(new Event()
            {
                Start = new DateTime(2017, 11, 17, 1, 0, 0),
                End = new DateTime(2017, 11, 17, 3, 0, 0),
                Id = "1"
            });
            events.Add(new Event()
            {
                Start = new DateTime(2017, 11, 17, 2, 0, 0),
                End = new DateTime(2017, 11, 17, 14, 0, 0),
                Id = "2"
            });
            events.Add(new Event()
            {
                Start = new DateTime(2017, 11, 17, 3, 15, 0),
                End = new DateTime(2017, 11, 17, 10, 0, 0),
                Id = "3"
            });
            events.Add(new Event()
            {
                Start = new DateTime(2017, 11, 17, 4, 30, 0),
                End = new DateTime(2017, 11, 17, 7, 0, 0),
                Id = "14"
            });
            events.Add(new Event()
            {
                Start = new DateTime(2017, 11, 17, 1, 0, 0),
                End = new DateTime(2017, 11, 17, 2, 0, 0),
                Id = "5"
            });

            var result = EventLocation.FindLayout(events);
        }
    }
}
