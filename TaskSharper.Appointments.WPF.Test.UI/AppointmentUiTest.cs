using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Windows.Forms;
using System.Drawing;
using System.Globalization;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UITest.Extension;
using Microsoft.VisualStudio.TestTools.UITesting.WpfControls;
using Keyboard = Microsoft.VisualStudio.TestTools.UITesting.Keyboard;


namespace TaskSharper.Appointments.WPF.Test.UI
{
    /// <summary>
    /// Summary description for AppointmentUiTest
    /// </summary>
    [CodedUITest]
    public class AppointmentUiTest
    {
        
        public AppointmentUiTest()
        {
            
        }

        [TestInitialize]
        public void Init()
        {
            Playback.PlaybackSettings.WaitForReadyLevel = WaitForReadyLevel.Disabled;
        }

        [TestMethod]
        public void OpenAppointmentApplication()
        {
            this.UIMap.OpenAppointmentApplication();
            this.UIMap.AssertApplicationIsAppointments();
        }

        [TestMethod]
        public void CloseAppointmentApplication()
        {
            this.UIMap.CloseAppointmentApplication();

        }

        [TestMethod]
        public void CreateAppointment_TitleIsEmpty()
        {
            this.UIMap.CreateAppointmentParams.UITitleTextBoxEditText = string.Empty;
            this.UIMap.CreateAppointmentParams.StartDate = DateTime.Now.Date.AddDays(1).ToString("dddd, MMMM dd, yyyy", CultureInfo.InvariantCulture);
            this.UIMap.CreateAppointmentParams.EndDate = DateTime.Now.Date.AddDays(1).ToString("dddd, MMMM dd, yyyy", CultureInfo.InvariantCulture);
            this.UIMap.CreateAppointment();

            this.UIMap.AssertTitleErrorTextBoxIsShowingEmptyErrorMessage();

            this.UIMap.ClickCancelButton();
        }

        [TestMethod]
        public void CreateAppointment_EndTimeIsBeforeStartTime()
        {
            this.UIMap.CreateAppointmentParams.StartDate = DateTime.Now.Date.AddDays(1).ToString("dddd, MMMM dd, yyyy", CultureInfo.InvariantCulture);
            this.UIMap.CreateAppointmentParams.EndDate = DateTime.Now.Date.ToString("dddd, MMMM dd, yyyy", CultureInfo.InvariantCulture);
            this.UIMap.CreateAppointment();
            
            this.UIMap.AssertDateTimeErrorIsShowingEndTimeBeforeStartTimeErrorMessage();

            this.UIMap.ClickCancelButton();
        }

        [TestMethod]
        public void CreateAppointment_EndTimeIsDayLaterThanStartTime()
        {
            this.UIMap.CreateAppointmentParams.StartDate = DateTime.Now.Date.AddDays(1).ToString("dddd, MMMM dd, yyyy", CultureInfo.InvariantCulture);
            this.UIMap.CreateAppointmentParams.EndDate = DateTime.Now.Date.AddDays(2).ToString("dddd, MMMM dd, yyyy", CultureInfo.InvariantCulture);
            this.UIMap.CreateAppointment();
            
            this.UIMap.AssertDateTimeErrorIsShowingDateSpanGreaterThanOneError();

            this.UIMap.ClickCancelButton();
        }

        [TestMethod]
        public void CreateAppointment_Success()
        {
            this.UIMap.CreateAppointmentParams.StartDate = DateTime.Now.Date.AddDays(1).ToString("dddd, MMMM dd, yyyy", CultureInfo.InvariantCulture);
            this.UIMap.CreateAppointmentParams.EndDate = DateTime.Now.Date.AddDays(1).ToString("dddd, MMMM dd, yyyy", CultureInfo.InvariantCulture);
            this.UIMap.CreateAppointment();

            this.UIMap.AssertTitleIsTestTitle();
        }

        [TestMethod]
        public void UpdateAppointment()
        {
            this.UIMap.UpdateAppointment();
            this.UIMap.AssertTitleIsUpdatedTitle();
        }

        [TestMethod]
        public void DeleteAppointment()
        {
            Playback.PlaybackSettings.DelayBetweenActions = 500; // Need to delay just a bit, to allow for the appointment to be properly deleted
            this.UIMap.DeleteAppointment();
            WpfText uIUpdatedTitleText = this.UIMap.UITaskSharperAppointmeWindow.UIItemCustom.UIContentScrollViewerPane.UIUpdatedTitleText;

            
            Playback.PlaybackSettings.SearchTimeout = 2000; // Timeout earlier than default, because if it doesn't find something within 2 seconds, it won't find anything at all.
            var found = uIUpdatedTitleText.TryFind();
            Assert.IsFalse(found);
        }

        #region Additional test attributes

        // You can use the following additional attributes as you write your tests:

        ////Use TestInitialize to run code before running each test 
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{        
        //    // To generate code for this test, select "Generate Code for Coded UI Test" from the shortcut menu and select one of the menu items.
        //}

        ////Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{        
        //    // To generate code for this test, select "Generate Code for Coded UI Test" from the shortcut menu and select one of the menu items.
        //}

        #endregion

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }
        private TestContext testContextInstance;

        public UIMap UIMap
        {
            get
            {
                if (this.map == null)
                {
                    this.map = new UIMap();
                }

                return this.map;
            }
        }

        private UIMap map;
    }
}
