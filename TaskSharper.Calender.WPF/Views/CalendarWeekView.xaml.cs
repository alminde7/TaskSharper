using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Prism.Events;
using TaskSharper.Calender.WPF.Properties;
using TaskSharper.WPF.Common.Events.ScrollEvents;

namespace TaskSharper.Calender.WPF.Views
{
    /// <summary>
    /// Interaction logic for CalendarWeekView.xaml
    /// </summary>
    public partial class CalendarWeekView : UserControl
    {
        private IEventAggregator _eventAggregator;
        public CalendarWeekView(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<ScrollUpEvent>().Subscribe(() => Scroll(-3 * Settings.Default.CalendarEvent_Height));
            _eventAggregator.GetEvent<ScrollDownEvent>().Subscribe(() => Scroll(3 * Settings.Default.CalendarEvent_Height));
            InitializeComponent();
        }

        void ScrollViewer_ManipulationBoundaryFeedback(object sender, ManipulationBoundaryFeedbackEventArgs e)
        {
            e.Handled = true;
        }

        private void Scroll(double offset)
        {
            ContentScrollViewer.ScrollToVerticalOffset(ContentScrollViewer.VerticalOffset + offset);
        }
    }
}
