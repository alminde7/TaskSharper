using System.Collections.Generic;
using NUnit.Framework;
using Google.Apis.Calendar.v3.Data;
using GoogleEvent = Google.Apis.Calendar.v3.Data.Event;
using TaskSharper.DataAccessLayer.Google.Calendar.Helpers;
using Event = TaskSharper.Domain.Calendar.Event;

namespace TaskSharper.DataAccessLayer.Test.Unit
{
    [TestFixture]
    public class HelpersTests
    {
        [SetUp]
        public void Init()
        {

        }

        [TearDown]
        public void Dispose()
        {

        }

        [Test]
        public void GoogleEventParser_FromGoogleEventToCalendarEvent_TypeIsTaskAsString_TypeIsSetToTaskAsEnumMember()
        {

            var googleEvent = new GoogleEvent
            {
                ExtendedProperties = new GoogleEvent.ExtendedPropertiesData
                {
                    Shared = new Dictionary<string, string>
                    {
                        { "Type", "Task" }
                    }
                }
            };

            var parsed = Helpers.GoogleEventParser(googleEvent);

            Assert.That(parsed.Type, Is.EqualTo(Event.EventType.Task));
        }

        [Test]
        public void GoogleEventParser_FromGoogleEventToCalendarEvent_TypeIsAppointmentAsString_TypeIsSetToAppointmentAsEnumMember()
        {

            var googleEvent = new GoogleEvent
            {
                ExtendedProperties = new GoogleEvent.ExtendedPropertiesData
                {
                    Shared = new Dictionary<string, string>
                    {
                        { "Type", "Appointment" }
                    }
                }
            };

            var parsed = Helpers.GoogleEventParser(googleEvent);

            Assert.That(parsed.Type, Is.EqualTo(Event.EventType.Appointment));
        }

        [Test]
        public void GoogleEventParser_FromGoogleEventToCalendarEvent_TypeIsNotSet_TypeIsSetToNoneAsEnumMember()
        {
            var parsed = Helpers.GoogleEventParser(new GoogleEvent());

            Assert.That(parsed.Type, Is.EqualTo(Event.EventType.None));
        }

        [Test]
        public void GoogleEventParser_FromGoogleEventToCalendarEvent_TypeIsStringNotMatchingAnyEnumMember_TypeIsSetToNoneAsEnumMember()
        {
            var googleEvent = new GoogleEvent
            {
                ExtendedProperties = new GoogleEvent.ExtendedPropertiesData
                {
                    Shared = new Dictionary<string, string>
                    {
                        { "Type", "InvalidString" }
                    }
                }
            };

            var parsed = Helpers.GoogleEventParser(googleEvent);

            Assert.That(parsed.Type, Is.EqualTo(Event.EventType.None));
        }
    }
}
