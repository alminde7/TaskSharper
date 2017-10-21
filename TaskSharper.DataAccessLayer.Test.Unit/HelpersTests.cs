using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Google.Apis.Calendar.v3.Data;
using GoogleEvent = Google.Apis.Calendar.v3.Data.Event;
using TaskSharper.DataAccessLayer.Google.Calendar.Helpers;
using TaskSharper.Domain.Calendar;
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

        #region public static Event GoogleEventParser(GoogleEvent googleEvent)

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

            Assert.That(parsed.Type, Is.EqualTo(EventType.Task));
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

            Assert.That(parsed.Type, Is.EqualTo(EventType.Appointment));
        }

        [Test]
        public void GoogleEventParser_FromGoogleEventToCalendarEvent_TypeIsNotSet_TypeIsSetToNoneAsEnumMember()
        {
            var parsed = Helpers.GoogleEventParser(new GoogleEvent());

            Assert.That(parsed.Type, Is.EqualTo(EventType.None));
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

            Assert.That(parsed.Type, Is.EqualTo(EventType.None));
        }

        [Test]
        public void GoogleEventParser_FromGoogleEventToCalendarEvent_ReminderIsListWithOneReminder_ReminderIsListWithOneReminder()
        {
            var googleEvent = new GoogleEvent
            {
                Reminders =
                    new GoogleEvent.RemindersData()
                    {
                        Overrides = new List<EventReminder>() { new EventReminder() { Minutes = 10 } }
                    }
            };

            var parsed = Helpers.GoogleEventParser(googleEvent);

            Assert.That(parsed.Reminders.Count, Is.EqualTo(1));
        }

        [Test]
        public void GoogleEventParser_FromGoogleEventToCalendarEvent_ReminderIsListOfRemindersData_ReminderIsListOfInts()
        {
            var googleEvent = new GoogleEvent
            {
                Reminders =
                    new GoogleEvent.RemindersData()
                    {
                        Overrides = new List<EventReminder>() { new EventReminder() { Minutes = 10 } }
                    }
            };

            var parsed = Helpers.GoogleEventParser(googleEvent);

            Assert.That(parsed.Reminders.GetType(), Is.EqualTo(typeof(List<int?>)));
        }

        #endregion

        #region public static List<Event> GoogleEventParser(List<GoogleEvent> googleEvents)

        [Test]
        public void GoogleEventParser_FromListOfGoogleEventsToListOfCalendarEvents_TypeIsTaskAsString_TypeIsSetToTaskAsEnumMember()
        {
            var googleEvents = new List<GoogleEvent>
            {
                new GoogleEvent
                {
                    ExtendedProperties = new GoogleEvent.ExtendedPropertiesData
                    {
                        Shared = new Dictionary<string, string>
                        {
                            {"Type", "Task"}
                        }
                    }
                }
            };

            var parsed = Helpers.GoogleEventParser(googleEvents);

            Assert.That(parsed.First().Type, Is.EqualTo(EventType.Task));
        }

        [Test]
        public void GoogleEventParser_FromListOfGoogleEventsToListOfCalendarEvents_TypeIsAppointmentAsString_TypeIsSetToAppointmentAsEnumMember()
        {
            var googleEvents = new List<GoogleEvent>
            {
                new GoogleEvent
                {
                    ExtendedProperties = new GoogleEvent.ExtendedPropertiesData
                    {
                        Shared = new Dictionary<string, string>
                        {
                            { "Type", "Appointment" }
                        }
                    }
                }
            };

            var parsed = Helpers.GoogleEventParser(googleEvents);

            Assert.That(parsed.First().Type, Is.EqualTo(EventType.Appointment));
        }

        [Test]
        public void GoogleEventParser_FromListOfGoogleEventsToListOfCalendarEvents_TypeIsNotSet_TypeIsSetToNoneAsEnumMember()
        {
            var parsed = Helpers.GoogleEventParser(new List<GoogleEvent>(){new GoogleEvent()});

            Assert.That(parsed.First().Type, Is.EqualTo(EventType.None));
        }

        [Test]
        public void GoogleEventParser_FromListOfGoogleEventsToListOfCalendarEvents_TypeIsStringNotMatchingAnyEnumMember_TypeIsSetToNoneAsEnumMember()
        {
            var googleEvents = new List<GoogleEvent>
            {
                new GoogleEvent
                {
                    ExtendedProperties = new GoogleEvent.ExtendedPropertiesData
                    {
                        Shared = new Dictionary<string, string>
                        {
                            { "Type", "InvalidString" }
                        }
                    }
                }
            };

            var parsed = Helpers.GoogleEventParser(googleEvents);

            Assert.That(parsed.First().Type, Is.EqualTo(EventType.None));
        }

        [Test]
        public void GoogleEventParser_FromListOfGoogleEventsToListOfCalendarEvents_ReminderIsListWithOneReminder_ReminderIsListWithOneReminder()
        {
            var googleEvents = new List<GoogleEvent>
            {
                new GoogleEvent
                {
                    Reminders =
                        new GoogleEvent.RemindersData()
                        {
                            Overrides = new List<EventReminder>() { new EventReminder() { Minutes = 10 } }
                        }
                }
            };

            var parsed = Helpers.GoogleEventParser(googleEvents);

            Assert.That(parsed.First().Reminders.Count, Is.EqualTo(1));
        }

        [Test]
        public void GoogleEventParser_FromListOfGoogleEventsToListOfCalendarEvents_ReminderIsListOfRemindersData_ReminderIsListOfInts()
        {
            var googleEvents = new List<GoogleEvent>
            {
                new GoogleEvent
                {
                    Reminders =
                        new GoogleEvent.RemindersData()
                        {
                            Overrides = new List<EventReminder>() { new EventReminder() { Minutes = 10 } }
                        }
                }
            };

            var parsed = Helpers.GoogleEventParser(googleEvents);

            Assert.That(parsed.First().Reminders.GetType(), Is.EqualTo(typeof(List<int?>)));
        }

        #endregion

        #region public static GoogleEvent GoogleEventParser(Event eventObj)

        [Test]
        public void GoogleEventParser_FromCalendarEventToGoogleEvent_TypeIsTaskAsEnumMember_TypeIsSetToTaskAsString()
        {
            var calendarEvent = new Event
            {
                Type = EventType.Task
            };

            var parsed = Helpers.GoogleEventParser(calendarEvent);
            Assert.That(parsed.ExtendedProperties.Shared["Type"], Is.EqualTo("Task"));
        }

        [Test]
        public void GoogleEventParser_FromCalendarEventToGoogleEvent_TypeIsAppointmentAsEnumMember_TypeIsSetToAppointmentAsString()
        {

            var calendarEvent = new Event
            {
                Type = EventType.Appointment
            };

            var parsed = Helpers.GoogleEventParser(calendarEvent);
            Assert.That(parsed.ExtendedProperties.Shared["Type"], Is.EqualTo("Appointment"));
        }

        [Test]
        public void GoogleEventParser_FromCalendarEventToGoogleEvent_TypeIsNotSet_TypeIsEmpty()
        {
            var parsed = Helpers.GoogleEventParser(new Event());

            Assert.That(parsed.ExtendedProperties.Shared["Type"], Is.EqualTo("None"));
        }

        [Test]
        public void GoogleEventParser_FromCalendarEventToGoogleEvent_ReminderIsListWithOneReminder_ReminderIsListWithOneReminder()
        {
            var calendarEvent = new Event
            {
                Reminders =
                    new List<int?>
                    {
                        10
                    }
            };

            var parsed = Helpers.GoogleEventParser(calendarEvent);

            Assert.That(parsed.Reminders.Overrides.Count, Is.EqualTo(1));
        }

        [Test]
        public void GoogleEventParser_FromCalendarEventToGoogleEvent_ReminderIsListOfInts_ReminderIsTypeOfRemindersData()
        {
            var calendarEvent = new Event
            {
                Reminders =
                    new List<int?>
                    {
                        10
                    }
            };

            var parsed = Helpers.GoogleEventParser(calendarEvent);

            Assert.That(parsed.Reminders.GetType(), Is.EqualTo(typeof(GoogleEvent.RemindersData)));
        }

        #endregion

        #region public static List<GoogleEvent> GoogleEventParser(List<Event> eventObjs)

        [Test]
        public void GoogleEventParser_FromListOfCalendarEventsToListOfGoogleEvents_TypeIsTaskAsEnumMember_TypeIsSetToTaskAsString()
        {
            var calendarEvents = new List<Event>
            {
                new Event
                {
                    Type = EventType.Task
                }
            };

            var parsed = Helpers.GoogleEventParser(calendarEvents);
            Assert.That(parsed.First().ExtendedProperties.Shared["Type"], Is.EqualTo("Task"));
        }

        [Test]
        public void GoogleEventParser_FromListOfCalendarEventsToListOfGoogleEvents_TypeIsAppointmentAsEnumMember_TypeIsSetToAppointmentAsString()
        {

            var calendarEvents = new List<Event>
            {
                new Event
                {
                    Type = EventType.Appointment
                }
            };

            var parsed = Helpers.GoogleEventParser(calendarEvents);
            Assert.That(parsed.First().ExtendedProperties.Shared["Type"], Is.EqualTo("Appointment"));
        }

        [Test]
        public void GoogleEventParser_FromListOfCalendarEventsToListOfGoogleEvents_TypeIsNotSet_TypeIsEmpty()
        {
            var parsed = Helpers.GoogleEventParser(new List<Event>(){new Event()});

            Assert.That(parsed.First().ExtendedProperties.Shared["Type"], Is.EqualTo("None"));
        }

        [Test]
        public void GoogleEventParser_FromListOfCalendarEventsToListOfGoogleEvents_ReminderIsListWithOneReminder_ReminderIsListWithOneReminder()
        {
            var calendarEvents = new List<Event>
            {
                new Event
                {
                    Reminders =
                        new List<int?>
                        {
                            10
                        }
                }
            };

            var parsed = Helpers.GoogleEventParser(calendarEvents);

            Assert.That(parsed.First().Reminders.Overrides.Count, Is.EqualTo(1));
        }

        [Test]
        public void GoogleEventParser_FromListOfCalendarEventsToListOfGoogleEvents_ReminderIsListOfInts_ReminderIsTypeOfRemindersData()
        {
            var calendarEvents = new List<Event>
            {
                new Event
                {
                    Reminders =
                        new List<int?>
                        {
                            10
                        }
                }
            };

            var parsed = Helpers.GoogleEventParser(calendarEvents);

            Assert.That(parsed.First().Reminders.GetType(), Is.EqualTo(typeof(GoogleEvent.RemindersData)));
        }

        #endregion

    }
}
