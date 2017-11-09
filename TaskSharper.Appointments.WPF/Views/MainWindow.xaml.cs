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
using TaskSharper.WPF.Common.Events.ViewEvents;

namespace TaskSharper.Appointments.WPF.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(IEventAggregator eventAggregator)
        {
            InitializeComponent();

            // From https://stackoverflow.com/questions/27574096/wpf-fullscreen-toggle-still-showing-part-of-desktop
            this.ResizeMode = ResizeMode.NoResize;
            this.WindowStyle = WindowStyle.ToolWindow;
            this.WindowState = WindowState.Maximized;
            //this.Topmost = true;
            this.PreviewKeyDown +=
                (s, e) =>
                {
                    if (e.Key == Key.F11)
                    {
                        if (this.WindowStyle != WindowStyle.SingleBorderWindow)
                        {
                            this.ResizeMode = ResizeMode.CanResize;
                            this.WindowStyle = WindowStyle.SingleBorderWindow;
                            this.WindowState = WindowState.Normal;
                            this.Topmost = false;
                        }
                        else
                        {
                            this.ResizeMode = ResizeMode.NoResize;
                            this.WindowStyle = WindowStyle.ToolWindow;
                            this.WindowState = WindowState.Maximized;
                            this.Topmost = true;
                        }
                    }
                };

            eventAggregator.GetEvent<BackButtonEvent>().Subscribe(status =>
            {
                switch (status)
                {
                    case BackButtonStatus.Show:
                        BackButton.Visibility = Visibility.Visible;
                        break;
                    case BackButtonStatus.Hide:
                        BackButton.Visibility = Visibility.Collapsed;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(status), status, null);
                }
            });
        }
    }
}
