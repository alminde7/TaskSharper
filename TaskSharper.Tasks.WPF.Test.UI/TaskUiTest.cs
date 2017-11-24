using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Windows.Forms;
using System.Drawing;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UITest.Extension;
using Microsoft.VisualStudio.TestTools.UITesting.WpfControls;
using Keyboard = Microsoft.VisualStudio.TestTools.UITesting.Keyboard;


namespace TaskSharper.Tasks.WPF.Test.UI
{
    /// <summary>
    /// Summary description for TaskUiTest
    /// </summary>
    [CodedUITest]
    public class TaskUiTest
    {
        public TaskUiTest()
        {
        }

        [TestInitialize]
        public void Init()
        {
            Playback.PlaybackSettings.WaitForReadyLevel = WaitForReadyLevel.Disabled;
        }

        [TestMethod]
        public void OpenTaskApplication()
        {
            this.UIMap.OpenTaskApplication();
            this.UIMap.AssertTaskApplicationIsOpen();
        }

        [TestMethod]
        public void CreateTask_Success()
        {
            this.UIMap.CreateTaskParams.StartDate = DateTime.Now.Date.AddDays(1).ToString("dddd, MMMM dd, yyyy", CultureInfo.InvariantCulture);
            this.UIMap.CreateTaskParams.EndDate = DateTime.Now.Date.AddDays(1).ToString("dddd, MMMM dd, yyyy", CultureInfo.InvariantCulture);
            this.UIMap.CreateTask();
            this.UIMap.AssertTaskIsCreatedWithTitleTestTitle();
        }

        [TestMethod]
        public void UpdateTask()
        {
            this.UIMap.UpdateTask();
            this.UIMap.AssertTaskIsUpdatedWithTitleUpdatedTitle();
        }

        [TestMethod]
        public void MarkTaskAsCompleted()
        {
            Playback.PlaybackSettings.DelayBetweenActions = 500; // Need to delay just a bit, to allow for the task to be properly updated
            this.UIMap.ClickCheckboxForTaskWhereTitleIsUpdatedTitleParams.UIUpdatedTitleCheckBoxChecked = true;
            this.UIMap.ClickCheckboxForTaskWhereTitleIsUpdatedTitle();
            this.UIMap.AssertTaskWithTitleUpdatedTitleIsChecked();
        }

        [TestMethod]
        public void MarkTaskAsIncomplete()
        {
            Playback.PlaybackSettings.DelayBetweenActions = 500; // Need to delay just a bit, to allow for the task to be properly updated
            this.UIMap.ClickCheckboxForTaskWhereTitleIsUpdatedTitleParams.UIUpdatedTitleCheckBoxChecked = false;
            this.UIMap.ClickCheckboxForTaskWhereTitleIsUpdatedTitle();
            this.UIMap.AssertTaskWithTitleUpdatedTitleIsUnchecked();
        }

        [TestMethod]
        public void DeleteTask()
        {

            Playback.PlaybackSettings.DelayBetweenActions = 500; // Need to delay just a bit, to allow for the task to be properly deleted
            this.UIMap.DeleteTask();
            WpfText uIUpdatedTitleText = this.UIMap.UITaskSharperTasksWindow.UIItemCustom.UIContentScrollViewerPane.UIUpdatedTitleText;


            Playback.PlaybackSettings.SearchTimeout = 2000; // Timeout earlier than default, because if it doesn't find something within 2 seconds, it won't find anything at all.
            var found = uIUpdatedTitleText.TryFind();
            Assert.IsFalse(found);
        }

        [TestMethod]
        public void CloseTaskApplication()
        {
            this.UIMap.CloseTaskApplication();
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
