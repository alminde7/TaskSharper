using System.Windows.Controls;
using System.Windows.Input;

namespace TaskSharper.Calender.WPF.Views
{ 
    /// <summary>
    /// Interaction logic for CalendarTodayComponent.xaml
    /// </summary>
    public partial class CalendarDayView : UserControl
    {
        public CalendarDayView()
        {
            InitializeComponent();
        }

        private void UIElement_OnManipulationBoundaryFeedback(object sender, ManipulationBoundaryFeedbackEventArgs e)
        {
            e.Handled = true;
        }
    }
}
