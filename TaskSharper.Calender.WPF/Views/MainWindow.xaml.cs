using System.Globalization;
using System.Windows;
using System.Windows.Input;

namespace TaskSharper.Calender.WPF.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
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
        }
    }
}
