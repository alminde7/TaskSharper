using System;
using System.Collections.Generic;
using NSubstitute;
using NUnit.Framework;
using TaskSharper.Domain.Calendar;
using TaskSharper.Domain.Configuration;
using TaskSharper.Domain.Configuration.Notification;
using TaskSharper.Domain.Notification;
using ILogger = Serilog.ILogger;


namespace TaskSharper.Notification.Test.Unit
{
    [TestFixture]
    public class NotificationUnitTets
    {
        private EventNotification _uut;
        private INotificationPublisher _notificationPublisher;
        private ILogger _logger;
        private NotificationSettings _notificationSettings;

        [SetUp]
        public void Setup()
        {
            _notificationPublisher = Substitute.For<INotificationPublisher>();
            _logger = Substitute.For<ILogger>();

            _notificationSettings = new NotificationSettings();

            _uut = new EventNotification(_notificationSettings, _logger,_notificationPublisher);
        }

        // Constructor

        [Test]
        public void Constructor_EverythingHasBeenInitialized()
        {
            Assert.That(_uut.NotificationSettings, Is.EqualTo(_notificationSettings));
            Assert.That(_uut.NotificationPublisher, Is.Not.EqualTo(null));
            Assert.That(_uut.Logger, Is.Not.EqualTo(null));
        }
        

        // EventNotifications

        // Attach Event
        [Test]
        public void EventNotifications_AttachEventToEventNotification_ContainsTwoEvents()
        {
            _uut.Attach(new Event() {Id = "Event1", Start = DateTime.Now.AddSeconds(30) });
            _uut.Attach(new Event() {Id = "Event2", Start = DateTime.Now.AddSeconds(50) });

            Assert.That(_uut.EventNotifications.Count, Is.EqualTo(2));
        }

        [Test]
        public void EventNotifications_AttachDublicatedEventToEventNotification_ContainsOneEvent()
        {
            _uut.Attach(new Event() { Id = "Event1", Start = DateTime.Now.AddSeconds(30) });
            _uut.Attach(new Event() { Id = "Event1", Start = DateTime.Now.AddSeconds(50) });

            Assert.That(_uut.EventNotifications.Count, Is.EqualTo(1));
        }
        [Test]
        public void EventNotifications_AttachEventToEventNotification_ContaintsZeroEvents()
        {
            Assert.That(_uut.EventNotifications.Count, Is.EqualTo(0));
        }

        // Attach EventList

        [Test]
        public void EventNotifications_AttachEventListToEventNotification_ContainsEvents()
        {
            List<Event> eventList = new List<Event>();
            eventList.Add(new Event { Id = "Event1", Start = DateTime.Now.AddSeconds(30) });
            eventList.Add(new Event { Id = "Event2", Start = DateTime.Now.AddSeconds(30) });

            _uut.Attach(eventList);

            Assert.That(_uut.EventNotifications.Count, Is.EqualTo(2));
        }

        [Test]
        public void EventNotifications_AttachEmptyEventListToEventNotification_ContainsZeroEvents()
        {
            List<Event> eventList = new List<Event>();

            _uut.Attach(eventList);

            Assert.That(_uut.EventNotifications.Count, Is.EqualTo(0));
        }

        // Detach Event
        
        [Test]
        public void EventNotifications_DetachEventFromEventNotification_ContainsOneEvent()
        {
            _uut.Attach(new Event() { Id = "Event1", Start = DateTime.Now.AddSeconds(30) });
            _uut.Attach(new Event() { Id = "Event2", Start = DateTime.Now.AddSeconds(50) });

            _uut.Detatch("Event1");

            Assert.That(_uut.EventNotifications.Count, Is.EqualTo(1));
        }

        [Test]
        public void EventNotifications_DetachMultipleEventsFromEventNotification_ContainsOneEvent()
        {
            _uut.Attach(new Event() { Id = "Event1", Start = DateTime.Now.AddSeconds(30) });
            _uut.Attach(new Event() { Id = "Event2", Start = DateTime.Now.AddSeconds(30) });
            _uut.Attach(new Event() { Id = "Event3", Start = DateTime.Now.AddSeconds(30) });

            _uut.Detatch(new List<string>(){"Event1", "Event2"});

            Assert.That(_uut.EventNotifications.Count, Is.EqualTo(1));
        }

        [Test]
        public void EventNotifications_DetachEventFromEmptyEventNotification_ReturnZeroWithNoErrors()
        {
            _uut.Detatch("Event1");

            Assert.That(_uut.EventNotifications.Count, Is.EqualTo(0));
        }


    }
}
