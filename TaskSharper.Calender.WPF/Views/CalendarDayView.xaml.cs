﻿using System.Windows.Controls;
using System.Windows.Input;
using Prism.Events;
using TaskSharper.WPF.Common.Events.ScrollEvents;
using Settings = TaskSharper.WPF.Common.Properties.Settings;

namespace TaskSharper.Calender.WPF.Views
{ 
    /// <summary>
    /// Interaction logic for CalendarTodayComponent.xaml
    /// </summary>
    public partial class CalendarDayView : UserControl
    {
        private IEventAggregator _eventAggregator;
        public CalendarDayView(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<ScrollUpEvent>().Subscribe(() => Scroll(-3 * Settings.Default.CalendarEvent_Height));
            _eventAggregator.GetEvent<ScrollDownEvent>().Subscribe(() => Scroll(3 * Settings.Default.CalendarEvent_Height));
            InitializeComponent();

            ContentScrollViewer.ScrollToVerticalOffset(8 * Settings.Default.CalendarEvent_Height);
        }

        /// <summary>
        /// Found at
        /// https://stackoverflow.com/questions/16126400/tablet-wpf-windows-desktop-application-scrolling-issue
        /// </summary>
        private void UIElement_OnManipulationBoundaryFeedback(object sender, ManipulationBoundaryFeedbackEventArgs e)
        {
            e.Handled = true;
        }

        private void Scroll(double offset)
        {
            ContentScrollViewer.ScrollToVerticalOffset(ContentScrollViewer.VerticalOffset + offset);
        }
    }
}
