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

        }



        [Test]
        public void Constructor_EverythingHasBeenInitialized()
        {
        }


    }
}
