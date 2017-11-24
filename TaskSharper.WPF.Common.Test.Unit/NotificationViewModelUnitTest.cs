using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Threading.Tasks;
using NSubstitute;
using Prism.Events;
using Serilog;
using TaskSharper.Domain.Calendar;
using TaskSharper.WPF.Common.Components.Notification;
using TaskSharper.WPF.Common.Events;
using TaskSharper.WPF.Common.Config;
using TaskSharper.WPF.Common.Events.NotificationEvents;
using TaskSharper.WPF.Common.Events.Resources;
using WPFLocalizeExtension.Engine;

namespace TaskSharper.WPF.Common.Test.Unit
{
    public class MockNotificationEvent : NotificationEvent { }

    public class MockCultureEvent : CultureChangedEvent { }

    public class MockSpinnerEvent : SpinnerEvent { }

    [TestFixture]
    public class NotificationViewModelUnitTest
    {
        private IEventAggregator _eventAggregator;

        private NotificationViewModel _notificationViewModel;
        [SetUp]
        public void SetUp()
        {
            _eventAggregator = Substitute.For<IEventAggregator>();
            _eventAggregator.GetEvent<NotificationEvent>().Returns(new MockNotificationEvent());
            _eventAggregator.GetEvent<CultureChangedEvent>().Returns(new MockCultureChangedEvent());
            _eventAggregator.GetEvent<SpinnerEvent>().Returns(new MockSpinnerEvent());
            var logger = Substitute.For<ILogger>();
            var dataservice = Substitute.For<IEventRestClient>();

            _notificationViewModel = new NotificationViewModel(_eventAggregator, logger, dataservice);
        }

        // Constructor

        [Test]
        public void Constructor_PopUp_IsFalse()
        {
            Assert.That(_notificationViewModel.IsPopupOpen, Is.EqualTo(false));
        }

        // ClosePopUp

        [Test]
        public void ClosePopUp_CloseNotificationCommandIsCalled_ClosePopUpIsFalse()
        {
            _notificationViewModel.IsPopupOpen = true;
            _notificationViewModel.CloseNotificationCommand.Execute();

            Assert.That(_notificationViewModel.IsPopupOpen, Is.EqualTo(false));
        }

        [Test]
        public void ClosePopUp_CloseNotificationCommandIsCalledWhenFalse_ClosePopUpIsFlase()
        {
            _notificationViewModel.IsPopupOpen = false;
            _notificationViewModel.CloseNotificationCommand.Execute();

            Assert.That(_notificationViewModel.IsPopupOpen, Is.EqualTo(false));
        }
        [Test]
        public void HandleNotificationEvent_InternetConnection_IsTrue()
        {
            // have to pre set because of other tests.
            ApplicationStatus.InternetConnection = true;

            var notificationevent = new Notification() { Event = new Event() { MarkedAsDone = false } };
            _eventAggregator.GetEvent<NotificationEvent>().Publish(notificationevent);

            Assert.That(ApplicationStatus.InternetConnection, Is.EqualTo(true));
        }

        // HandleNotificationEvent
        [Test]
        public void HandleNotificationEvent_InternetConnection_IsFalse()
        {
            var notificationevent = new ConnectionErrorNotification() {Event = new Event() {MarkedAsDone = false}};
            _eventAggregator.GetEvent<NotificationEvent>().Publish(notificationevent);

            Assert.That(ApplicationStatus.InternetConnection, Is.EqualTo(false));
        }
  
        // Shownotification

        [Test]
        public void Shownotification_TitleIsSet_SetCorrect()
        {
            var notificationevent = new Notification() { Event = new Event() { MarkedAsDone = false}, Title = "test123" };
            _eventAggregator.GetEvent<NotificationEvent>().Publish(notificationevent);

            Assert.That(_notificationViewModel.NotificationTitle, Is.EqualTo("test123"));
        }

        [Test]
        public void Shownotification_MessageIsSet_SetCorrect()
        {
            var notificationevent = new Notification() { Event = new Event() { MarkedAsDone = false }, Message = "test123" };
            _eventAggregator.GetEvent<NotificationEvent>().Publish(notificationevent);

            Assert.That(_notificationViewModel.NotificationMessage, Is.EqualTo("test123"));
        }

        [Test]
        public void Shownotification_NotificationEventTypeSet_IsSetCorrect()
        {
            var notificationevent = new Notification() { Event = new Event() { MarkedAsDone = false, Type = EventType.Task } };
            _eventAggregator.GetEvent<NotificationEvent>().Publish(notificationevent);

            Assert.That(_notificationViewModel.NotificationEvent.Type, Is.EqualTo(EventType.Task));
        }

        [Test]
        public void Shownotification_NotificationTypeSet_IsSetCorrect()
        {
            var notificationevent = new Notification() { Event = new Event() { MarkedAsDone = false}, NotificationType = NotificationTypeEnum.Warning};
            _eventAggregator.GetEvent<NotificationEvent>().Publish(notificationevent);

            Assert.That(_notificationViewModel.NotificationType, Is.EqualTo(NotificationTypeEnum.Warning));
        }

        [Test]
        public void Shownotification_EventSet_IsSetCorrect()
        {
            var testEvent = new Event() {MarkedAsDone = false, Title = "test123", Description = "testDescription"};
            var notificationevent = new Notification() { Event = testEvent };
            _eventAggregator.GetEvent<NotificationEvent>().Publish(notificationevent);

            Assert.That(_notificationViewModel.NotificationEvent, Is.EqualTo(testEvent));
        }

        [Test]
        public void Shownotification_CategoryTask_CategoryGetsTasksIcon()
        {
            var testEvent = new Event() { MarkedAsDone = false, Category = new EventCategory(){Id = "123", Name = "Tasks"}, Type =EventType.Task };
            var notificationevent = new Notification() { Event = testEvent };
            _eventAggregator.GetEvent<NotificationEvent>().Publish(notificationevent);

            Assert.That(_notificationViewModel.Category, Is.EqualTo(testEvent.Category.Name));
        }

        [Test]
        public void Shownotification_CategoryDefault_CategoryGetsInfoIcon()
        {
            var testEvent = new Event() { MarkedAsDone = false, Category = new EventCategory() { Id = "123", Name = "test123" }};
            var notificationevent = new Notification() { Event = testEvent };
            _eventAggregator.GetEvent<NotificationEvent>().Publish(notificationevent);

            Assert.That(_notificationViewModel.Category, Is.EqualTo("Info"));
        }

        [Test]
        public void Shownotification_NotificationEventStartIsNull_NotificationTimeTextIsNull()
        {
            var testEvent = new Event() { MarkedAsDone = false, Category = new EventCategory() { Id = "123", Name = "test123" } ,Start = null};
            var notificationevent = new Notification() { Event = testEvent };
            _eventAggregator.GetEvent<NotificationEvent>().Publish(notificationevent);

            Assert.That(_notificationViewModel.NotificationTimeText, Is.EqualTo(null));
        }

        [Test]
        public void Shownotification_NotificationEventEndIsNull_NotificationTimeTextIsNull()
        {
            var testEvent = new Event() { MarkedAsDone = false, Category = new EventCategory() { Id = "123", Name = "test123" }, End = null };
            var notificationevent = new Notification() { Event = testEvent };
            _eventAggregator.GetEvent<NotificationEvent>().Publish(notificationevent);

            Assert.That(_notificationViewModel.NotificationTimeText, Is.EqualTo(null));
        }

        //[Test]
        //public void Shownotification_NotificationEventStartAndEndIsEqual_NotificationTimeTextIsCorrect()
        //{
        //    LocalizeDictionary.Instance.Culture = CultureInfo.CurrentCulture;
        //    var tempDatetime = DateTime.Now;
        //    var testEvent = new Event() { MarkedAsDone = false, Category = new EventCategory() { Id = "123", Name = "test123" }, Start = tempDatetime , End = tempDatetime };
        //    var notificationevent = new Notification() { Event = testEvent };
        //    _eventAggregator.GetEvent<NotificationEvent>().Publish(notificationevent);

        //    Assert.That(_notificationViewModel.NotificationTimeText, Is.EqualTo("check"));
        //}


    }
}
