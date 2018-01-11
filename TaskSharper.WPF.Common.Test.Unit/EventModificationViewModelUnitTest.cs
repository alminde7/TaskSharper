using System;
using NSubstitute;
using NUnit.Framework;
using Prism.Events;
using Prism.Regions;
using Serilog;
using TaskSharper.Domain.Calendar;
using TaskSharper.Domain.Models;
using TaskSharper.Domain.RestClient;
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
        private IEventAggregator _eventAggregator;
        private IRegionManager _regionManager;
        private IEventRestClient _eventRestClient;
        private ILogger _logger;

        [SetUp]
        public void Setup()
        {
            _eventAggregator = Substitute.For<IEventAggregator>();
            _eventAggregator.GetEvent<CultureChangedEvent>().Returns(new MockCultureChangedEvent());
            _eventAggregator.GetEvent<CategoryClickedEvent>().Returns(new MockCategoryClickedEvent());

            _regionManager = Substitute.For<IRegionManager>();
            _eventRestClient = Substitute.For<IEventRestClient>();
            _logger = Substitute.For<ILogger>();

            _uut = new EventModificationViewModel(_regionManager, _eventRestClient, _eventAggregator, _logger);

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

        [Test]
        public void SaveEvent_EventIsWithoutErrorAndModificationTypeIsOfTypeCreate_CreateMethodOnDataServiceIsCalled()
        {
            var parameters = new NavigationParameters();
            parameters.Add("Type", EventType.Task);
            
            _uut.OnNavigatedTo(new NavigationContext(Substitute.For<IRegionNavigationService>(), new UriBuilder("EventModification").Uri, parameters));

            _uut.Event = new Event
            {
                Start = DateTime.Today,
                End = DateTime.Today.AddHours(2),
                Id = "1",
                Title = "SomeTitle",
                Type = EventType.Task
            };

            _uut.SaveEvent();
            _eventRestClient.Received(1).CreateAsync(Arg.Any<Event>());
        }

        [Test]
        public void SaveEvent_EventIsWithoutErrorAndModificationTypeIsOfTypeEdit_UpdateMethodOnDataServiceIsCalled()
        {
            // Arrange
            var parameters = new NavigationParameters();
            parameters.Add("Id", "1");
            parameters.Add("CalendarId", "1");
            parameters.Add("Type", EventType.Task);
            _uut.Event.Type = EventType.Task;
            _eventRestClient.Get("1", "1").Returns(_uut.Event);
            
            // Act
            _uut.OnNavigatedTo(new NavigationContext(Substitute.For<IRegionNavigationService>(), new UriBuilder("EventModification").Uri, parameters));
            _uut.SaveEvent();

            // Assert
            _eventRestClient.Received(1).UpdateAsync(Arg.Any<Event>());
        }
    }

    
}
