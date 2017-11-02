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
using TaskSharper.Appointments.WPF.Properties;
using TaskSharper.WPF.Common.Events.ScrollEvents;

namespace TaskSharper.Appointments.WPF.Views
{
    /// <summary>
    /// Interaction logic for AppointmentsContainerView.xaml
    /// </summary>
    public partial class AppointmentCardContainerView : UserControl
    {
        private IEventAggregator _eventAggregator;

        public AppointmentCardContainerView(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<ScrollUpEvent>().Subscribe(() => Scroll(-150));
            _eventAggregator.GetEvent<ScrollDownEvent>().Subscribe(() => Scroll(150));
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
