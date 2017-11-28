using System.Windows.Controls;
using System.Windows.Input;

namespace TaskSharper.Calender.WPF.Views
{
    /// <summary>
    /// Interaction logic for CalendarEventShowDetailsView.xaml
    /// </summary>
    public partial class CalendarEventShowDetailsView : UserControl
    {
        public CalendarEventShowDetailsView()
        {
            InitializeComponent();
        }

        void ScrollViewer_ManipulationBoundaryFeedback(object sender, ManipulationBoundaryFeedbackEventArgs e)
        {
            e.Handled = true;
        }
    }
}
