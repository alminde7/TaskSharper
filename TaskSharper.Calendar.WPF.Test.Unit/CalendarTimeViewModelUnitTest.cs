using NUnit.Framework;
using TaskSharper.Calender.WPF.ViewModels;

namespace TaskSharper.Calendar.WPF.Test.Unit
{
    [TestFixture]
    public class CalendarTimeViewModelUnitTest
    {
        private CalendarTimeViewModel _uut;

        [SetUp]
        public void Setup()
        {
            _uut = new CalendarTimeViewModel();
        }

        [Test]
        public void Hour_PropertySet_HasValueOfSeven()
        {
            _uut.Hour = 7;

            Assert.That(_uut.Hour,Is.EqualTo(7));
        }
    }
}
