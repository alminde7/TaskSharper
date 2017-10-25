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
