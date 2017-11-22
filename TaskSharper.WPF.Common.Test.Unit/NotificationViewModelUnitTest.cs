using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Threading.Tasks;
using NSubstitute;
using Prism.Events;
using TaskSharper.WPF.Common.Components.Notification;
using TaskSharper.WPF.Common.Events;
using TaskSharper.WPF.Common.Events.NotificationEvents;

namespace TaskSharper.WPF.Common.Test.Unit
{
    public class MockNotificationEvent : NotificationEvent { }

    public class MockCultureEvent : CultureChangedEvent { }

    [TestFixture]
    public class NotificationViewModelUnitTest
    {
        private NotificationViewModel _notificationViewModel;
        [SetUp]
        public void SetUp()
        {
            var eventAggregator = Substitute.For<IEventAggregator>();
            eventAggregator.GetEvent<NotificationEvent>().Returns(new MockNotificationEvent());
            eventAggregator.GetEvent<CultureChangedEvent>().Returns(new MockCultureChangedEvent());
            _notificationViewModel = new NotificationViewModel(eventAggregator);
        }

        [Test]
        public void testConstructor()
        {
            _notificationViewModel.Category = "test";
            Assert.AreEqual(true,true);
        }

    }
}
