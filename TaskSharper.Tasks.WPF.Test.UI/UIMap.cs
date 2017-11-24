using System.Globalization;

namespace TaskSharper.Tasks.WPF.Test.UI
{
    using Microsoft.VisualStudio.TestTools.UITesting.WpfControls;
    using System;
    using System.Collections.Generic;
    using System.CodeDom.Compiler;
    using Microsoft.VisualStudio.TestTools.UITest.Extension;
    using Microsoft.VisualStudio.TestTools.UITesting;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Keyboard = Microsoft.VisualStudio.TestTools.UITesting.Keyboard;
    using Mouse = Microsoft.VisualStudio.TestTools.UITesting.Mouse;
    using MouseButtons = System.Windows.Forms.MouseButtons;
    using System.Drawing;
    using System.Windows.Input;
    using System.Text.RegularExpressions;


    public partial class UIMap
    {


        /// <summary>
        /// CreateTask - Use 'CreateTaskParams' to pass parameters into this method.
        /// </summary>
        public void CreateTask()
        {
            #region Variable Declarations
            WpfButton uIItemButton = this.UITaskSharperTasksWindow.UIItemCustom.UIContentScrollViewerPane.UIItemButton;
            WpfEdit uITitleTextBoxEdit = this.UITaskSharperTasksWindow.UIItemCustom1.UITitleTextBoxEdit;
            WpfEdit uIDescriptionTextBoxEdit = this.UITaskSharperTasksWindow.UIItemCustom1.UIDescriptionTextBoxEdit;
            WpfButton uIItemButton1 = this.UITaskSharperTasksWindow.UIItemCustom1.UIStatusText.UIItemButton;
            WpfButton uIItemButton2 = this.UITaskSharperTasksWindow.UIItemCustom1.UICategoryText.UIItemButton;
            WpfButton startDate = this.UITaskSharperTasksWindow.UIStartTimePickerCustom.UIDatePickerCalendarCalendar.UIStartDateButton;
            WpfButton endDate = this.UITaskSharperTasksWindow.UIEndTimePickerCustom.UIDatePickerCalendarCalendar.UIEndDateButton;
            WpfImage uIItemImage = this.UITaskSharperTasksWindow.UIEndTimePickerCustom.UIHourListBoxList.UIItemImage;
            WpfButton uIItemButton11 = this.UITaskSharperTasksWindow.UIItemCustom1.UICategoryText.UIItemButton1;
            #endregion

            // Click button numbered 2 next to 'ContentScrollViewer' pane
            Mouse.Click(uIItemButton, new Point(69, 71));

            // Type 'TestTitle' in 'TitleTextBox' text box
            uITitleTextBoxEdit.Text = this.CreateTaskParams.UITitleTextBoxEditText;

            // Type 'TestDescription' in 'DescriptionTextBox' text box
            uIDescriptionTextBoxEdit.Text = this.CreateTaskParams.UIDescriptionTextBoxEditText;

            // Click first button next to 'Status' label
            Mouse.Click(uIItemButton1, new Point(75, 80));

            // Click button numbered 4 next to 'Category' label
            Mouse.Click(uIItemButton2, new Point(84, 55));

            // Click 'Start Date' button
            startDate.SearchProperties[WpfButton.PropertyNames.Name] = CreateTaskParams.StartDate;
            Mouse.Click(startDate, new Point(18, 9));

            //Set "EndDate" button properties
            endDate.SearchProperties[WpfButton.PropertyNames.Name] = CreateTaskParams.EndDate;

            // Click 'EndDate' button twice
            Mouse.Click(endDate, new Point(22, 17));
            Mouse.Click(endDate, new Point(20, 14));

            // Double-Click first image next to 'HourListBox' list box
            Mouse.DoubleClick(uIItemImage, new Point(21, 17));

            // Click first image next to 'HourListBox' list box
            Mouse.Click(uIItemImage, new Point(21, 17));

            // Click button numbered 5 next to 'Category' label
            Mouse.Click(uIItemButton11, new Point(19, 36));
        }

        public virtual CreateTaskParams CreateTaskParams
        {
            get
            {
                if ((this.mCreateTaskParams == null))
                {
                    this.mCreateTaskParams = new CreateTaskParams();
                }
                return this.mCreateTaskParams;
            }
        }

        private CreateTaskParams mCreateTaskParams;
    }

    /// <summary>
    /// Parameters to be passed into 'CreateTask'
    /// </summary>
    [GeneratedCode("Coded UITest Builder", "15.0.26208.0")]
    public class CreateTaskParams
    {

        #region Fields
        /// <summary>
        /// Type 'TestTitle' in 'TitleTextBox' text box
        /// </summary>
        public string UITitleTextBoxEditText = "TestTitle";

        /// <summary>
        /// Type 'TestDescription' in 'DescriptionTextBox' text box
        /// </summary>
        public string UIDescriptionTextBoxEditText = "TestDescription";

        /// <summary>
        /// Format MUST be: "dddd, MMMM d, yyyy"
        /// </summary>
        public string StartDate = DateTime.Now.Date.AddDays(1).ToString("dddd, MMMM dd, yyyy", CultureInfo.InvariantCulture);

        /// <summary>
        /// Format MUST be: "dddd, MMMM d, yyyy"
        /// </summary>
        public string EndDate = DateTime.Now.Date.AddDays(1).ToString("dddd, MMMM dd, yyyy", CultureInfo.InvariantCulture);
        #endregion
    }
}
