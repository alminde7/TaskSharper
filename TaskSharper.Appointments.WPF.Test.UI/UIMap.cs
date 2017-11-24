namespace TaskSharper.Appointments.WPF.Test.UI
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
        /// CreateAppointment - Use 'CreateAppointmentParams' to pass parameters into this method.
        /// </summary>
        public void CreateAppointment()
        {
            #region Variable Declarations
            WpfButton uIItemButton = this.UIMainWindowWindow.UIItemButton;
            WpfButton uIItemButton1 = this.UITaskSharperAppointmeWindow.UIItemCustom.UIContentScrollViewerPane.UIItemButton;
            WpfEdit uITitleTextBoxEdit = this.UITaskSharperAppointmeWindow.UIItemCustom1.UITitleTextBoxEdit;
            WpfEdit uIDescriptionTextBoxEdit = this.UITaskSharperAppointmeWindow.UIItemCustom1.UIDescriptionTextBoxEdit;
            WpfButton uIItemButton2 = this.UITaskSharperAppointmeWindow.UIItemCustom1.UIStatusText.UIItemButton;
            WpfButton uIItemButton3 = this.UITaskSharperAppointmeWindow.UIItemCustom1.UICategoryText.UIItemButton;
            WpfButton startDate = this.UITaskSharperAppointmeWindow.UIStartTimePickerCustom.UIDatePickerCalendarCalendar.UIStartDateButton;
            WpfButton endDate = this.UITaskSharperAppointmeWindow.UIEndTimePickerCustom.UIDatePickerCalendarCalendar.UIEndDateButton;
            WpfImage uIItemImage = this.UITaskSharperAppointmeWindow.UIEndTimePickerCustom.UIHourListBoxList.UIItemImage;
            WpfButton uIItemButton11 = this.UITaskSharperAppointmeWindow.UIItemCustom1.UICategoryText.UIItemButton1;
            #endregion

            // Click button numbered 2 in 'MainWindow' window
            //Mouse.Click(uIItemButton, new Point(373, 504));

            // Click button numbered 2 next to 'ContentScrollViewer' pane
            Mouse.Click(uIItemButton1, new Point(91, 87));

            // Type 'TestTitle' in 'TitleTextBox' text box
            uITitleTextBoxEdit.Text = this.CreateAppointmentParams.UITitleTextBoxEditText;

            // Type 'TestDescription' in 'DescriptionTextBox' text box
            uIDescriptionTextBoxEdit.Text = this.CreateAppointmentParams.UIDescriptionTextBoxEditText;

            // Click first button next to 'Status' label
            Mouse.Click(uIItemButton2, new Point(72, 86));

            // Click button numbered 4 next to 'Category' label
            Mouse.Click(uIItemButton3, new Point(98, 86));

            // Click 'Start Date' button
            startDate.SearchProperties[WpfButton.PropertyNames.Name] = CreateAppointmentParams.StartDate;
            Mouse.Click(startDate, new Point(18, 9));

            //Set "EndDate" button properties
            endDate.SearchProperties[WpfButton.PropertyNames.Name] = CreateAppointmentParams.EndDate;

            // Click 'EndDate' button twice
            Mouse.Click(endDate, new Point(22, 17));
            Mouse.Click(endDate, new Point(20, 14));

            // Double-Click first image next to 'HourListBox' list box
            Mouse.DoubleClick(uIItemImage, new Point(37, 19));

            // Click first image next to 'HourListBox' list box
            Mouse.Click(uIItemImage, new Point(37, 19));

            // Click button numbered 5 next to 'Category' label
            Mouse.Click(uIItemButton11, new Point(37, 33));
        }

        public virtual CreateAppointmentParams CreateAppointmentParams
        {
            get
            {
                if ((this.mCreateAppointmentParams == null))
                {
                    this.mCreateAppointmentParams = new CreateAppointmentParams();
                }
                return this.mCreateAppointmentParams;
            }
        }

        private CreateAppointmentParams mCreateAppointmentParams;
    }
    /// <summary>
    /// Parameters to be passed into 'CreateAppointment'
    /// </summary>
    [GeneratedCode("Coded UITest Builder", "15.0.26208.0")]
    public class CreateAppointmentParams
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
        public string StartDate = "Friday, November 24, 2017";

        /// <summary>
        /// Format MUST be: "dddd, MMMM d, yyyy"
        /// </summary>
        public string EndDate = "Friday, November 24, 2017";

        #endregion
    }
}
