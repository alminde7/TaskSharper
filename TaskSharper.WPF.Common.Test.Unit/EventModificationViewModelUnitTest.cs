using System;
using NSubstitute;
using NUnit.Framework;
using Prism.Events;
using Prism.Regions;
using TaskSharper.Domain.Calendar;
using TaskSharper.WPF.Common.Components.EventModification;
using TaskSharper.WPF.Common.Events;
using TaskSharper.WPF.Common.Properties;

namespace TaskSharper.WPF.Common.Test.Unit
{
    public class MockCultureChangedEvent : CultureChangedEvent {}
    public class MockCategoryClickedEvent : CategoryClickedEvent {}

    [TestFixture]
    public class EventModificationViewModelUnitTest
    {
        private EventModificationViewModel _uut;
        [SetUp]
        public void Setup()
        {
            var eventAggregator = Substitute.For<IEventAggregator>();
            eventAggregator.GetEvent<CultureChangedEvent>().Returns(new MockCultureChangedEvent());
            eventAggregator.GetEvent<CategoryClickedEvent>().Returns(new MockCategoryClickedEvent());
            _uut = new EventModificationViewModel(Substitute.For<IRegionManager>(), Substitute.For<IEventRestClient>(), eventAggregator);

            _uut.Event = new Event
            {
                Start = DateTime.Today,
                End = DateTime.Today.AddHours(2),
                Id = "1",
                Title = "SomeTitle"
            };
        }

        [Test]
        public void SaveEvent_TitleIsNotSet_TitleErrorMessageIsSetToErrorTitleNotSet()
        {
            _uut.Event.Title = null;
            _uut.SaveEvent();

            Assert.That(_uut.TitleErrorMessage, Is.EqualTo(Resources.ErrorTitleNotSet));
        }

        [Test]
        public void SaveEvent_StartTimeIsAfterEndTime_DateTimeErrorMessageIsSetToErrorEndTimeIsEarlierThanStartTime()
        {
            _uut.Event.Start = DateTime.Today.AddHours(3);
            _uut.SaveEvent();

            Assert.That(_uut.DateTimeErrorMessage, Is.EqualTo(Resources.ErrorEndTimeIsEarlierThanStartTime));
        }

        [Test]
        public void SaveEvent_StartTimeIsInThePast_DateTimeErrorMessageIsSetToErrorStartTimeIsBeforeTodaysDate()
        {
            _uut.Event.Start = DateTime.Today.AddDays(-1);
            _uut.Event.End = DateTime.Today.AddDays(-1);
            _uut.SaveEvent();

            Assert.That(_uut.DateTimeErrorMessage, Is.EqualTo(Resources.ErrorStartTimeIsBeforeTodaysDate));
        }

        [Test]
        public void SaveEvent_EventSpansAcrossMultipleDays_DateTimeErrorMessageIsSetToErrorEventSpansAccrossMultipleDays()
        {
            _uut.Event.End = DateTime.Today.AddDays(2);
            _uut.SaveEvent();

            Assert.That(_uut.DateTimeErrorMessage, Is.EqualTo(Resources.ErrorEventSpansAccrossMultipleDays));
        }
    }

    
}
