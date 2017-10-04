using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using TaskSharper.Domain.Calendar;
using Assert = NUnit.Framework.Assert;

namespace TaskSharper.Notification.Test.Unit
{
    [TestFixture]
    public class NotificationUnitTets
    {
        private EventNotification _uut;
        private IEnumerable<int> List;

        [SetUp]
        public void Setup()
        {
            var list = new List<int>()
            {
                -15,
                -5,
                0,
                5,
                10,
                15
            };

            List = list;

            _uut = new EventNotification(List);
        }



        [Test]
        public void Constructor_EverythingHasBeenInitialized()
        {
            Assert.NotNull(_uut.EventNotifications);
            Assert.NotNull(_uut.NotificationOffsets);
        }

        [Test]
        public void Attach_AddEvent_EventHasBeenAddedOnceToDictionaryWithAListOf7Timers()
        {
            bool eventFired = false;

            void HandleEvent(Event caleEvent)
            {
                eventFired = true;
            }

            var calEvent = new Event();

            calEvent.Start = DateTime.Now.AddMinutes(30);

            _uut.Attach(calEvent, HandleEvent);
        }
    }
}
