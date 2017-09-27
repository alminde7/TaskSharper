using System.Windows.Controls;
using System.Windows.Input;

namespace TaskSharper.Calender.WPF.Views
{
    /// <summary>
    /// Interaction logic for CalendarEventView.xaml
    /// </summary>
    public partial class CalendarEventView : UserControl
    {
        public CalendarEventView()
        {
            InitializeComponent();
        }

        private void UIElement_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            eventPop.IsOpen = true;
        }
    }
}
