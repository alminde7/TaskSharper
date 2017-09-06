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
        public void GoogleEventParser_TypeIsTask_TypeIsTask()
        {
            
            var googleEvent = new GoogleEvent()
            {
                ExtendedProperties = new GoogleEvent.ExtendedPropertiesData { Shared = new Dictionary<string, string>
                {
                    { "Type", "Task" }
                }}
            };

            var parsed = Helpers.GoogleEventParser(googleEvent);

            Assert.That(parsed.Type, Is.EqualTo(Event.EventType.Task));
        }
    }
}
