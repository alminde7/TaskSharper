using System;
using NUnit.Framework;
using TaskSharper.Domain.Calendar;
using TaskSharper.WPF.Common.Media;

namespace TaskSharper.WPF.Common.Test.Unit
{
    [TestFixture]
    public class CategoryToIconConverterUnitTest
    {
        [Test]
        public void ConvertToFontAwesomeIcon_CategoryIsMedicationTypeIsNone_ResultIsMedkit()
        {
            var result = CategoryToIconConverter.ConvertToFontAwesomeIcon("Medication", EventType.None);
            Assert.That(result, Is.EqualTo("Medkit"));
        }

        [Test]
        public void ConvertToFontAwesomeIcon_CategoryIsMedicationTypeIsTask_ResultIsMedkit()
        {
            var result = CategoryToIconConverter.ConvertToFontAwesomeIcon("Medication", EventType.Task);
            Assert.That(result, Is.EqualTo("Medkit"));
        }

        [Test]
        public void ConvertToFontAwesomeIcon_CategoryIsHygieneTypeIsNone_ResultIsShower()
        {
            var result = CategoryToIconConverter.ConvertToFontAwesomeIcon("Hygiene", EventType.None);
            Assert.That(result, Is.EqualTo("Shower"));
        }

        [Test]
        public void ConvertToFontAwesomeIcon_CategoryIsSocialTypeIsNone_ResultIsUsers()
        {
            var result = CategoryToIconConverter.ConvertToFontAwesomeIcon("Social", EventType.None);
            Assert.That(result, Is.EqualTo("Users"));
        }

        [Test]
        public void ConvertToFontAwesomeIcon_CategoryIsNonExistingCategoryTypeIsNone_ResultIsInfo()
        {
            var result = CategoryToIconConverter.ConvertToFontAwesomeIcon("NonExistingCategory", EventType.None);
            Assert.That(result, Is.EqualTo("Info"));
        }

        [Test]
        public void ConvertToFontAwesomeIcon_CategoryIsNonExistingCategoryTypeIsAppointment_ResultIsListUl()
        {
            var result = CategoryToIconConverter.ConvertToFontAwesomeIcon("NonExistingCategory", EventType.Appointment);
            Assert.That(result, Is.EqualTo("ListUl"));
        }

        [Test]
        public void ConvertToFontAwesomeIcon_CategoryIsNonExistingCategoryTypeIsTask_ResultIsTasks()
        {
            var result = CategoryToIconConverter.ConvertToFontAwesomeIcon("NonExistingCategory", EventType.Task);
            Assert.That(result, Is.EqualTo("Tasks"));
        }
    }
}
