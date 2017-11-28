using System.Windows.Controls;

namespace TaskSharper.WPF.Common.Components.Notification
{
    /// <summary>
    /// Interaction logic for NotificationView.xaml
    /// </summary>
    public partial class NotificationView : UserControl
    {
        public NotificationView()
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                InitializeComponent();
            }
            
        }
    }
}
