using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
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

namespace TaskSharper.Launcher.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly bool _allGood;

        private readonly string _pathToCalendarApp;
        private readonly string _pathToAppointmentsApp;
        private readonly string _pathToTasksApp;

        public MainWindow()
        {
            InitializeComponent();

            this.WindowState = WindowState.Maximized;

            _allGood = false;

            var executingPathString = AppDomain.CurrentDomain.BaseDirectory;
            var executingPath = new DirectoryInfo(executingPathString);

#if DEBUG

            var basePath = executingPath?.Parent?.Parent?.Parent;

            if (basePath != null)
            {
                var calendarArr = new string[] { basePath.FullName, "TaskSharper.Calender.WPF", "bin", "Debug", "TaskSharper.Calender.WPF.exe" };
                var appointmentsArr = new string[] { basePath.FullName, "TaskSharper.Appointments.WPF", "bin", "Debug", "TaskSharper.Appointments.WPF.exe" };
                var tasksArr = new string[] { basePath.FullName, "TaskSharper.Tasks.WPF", "bin", "Debug", "TaskSharper.Tasks.WPF.exe" };

                _pathToCalendarApp = System.IO.Path.Combine(calendarArr);
                _pathToAppointmentsApp = System.IO.Path.Combine(appointmentsArr);
                _pathToTasksApp = System.IO.Path.Combine(tasksArr);
            }



#else
            var basePath = executingPath?.Parent?.Parent;
            if (basePath != null)
            {
                var calendarArr = new string[] { basePath.FullName, "TaskSharper.Calender.WPF", "App", "TaskSharper.Calender.WPF.exe" };
                var appointmentsArr = new string[] { basePath.FullName, "TaskSharper.Appointments.WPF", "App", "TaskSharper.Appointments.WPF.exe" };
                var tasksArr = new string[] { basePath.FullName, "TaskSharper.Tasks.WPF", "App", "TaskSharper.Tasks.WPF.exe" };

                _pathToCalendarApp = System.IO.Path.Combine(calendarArr);
                _pathToAppointmentsApp = System.IO.Path.Combine(appointmentsArr);
                _pathToTasksApp = System.IO.Path.Combine(tasksArr);
            }
#endif

            if (File.Exists(_pathToTasksApp) && File.Exists(_pathToAppointmentsApp) &&
                File.Exists(_pathToCalendarApp))
            {
                _allGood = true;
            }
            else
            {
                MessageBox.Show("Could not locate applications", "ERROR");
                _allGood = false;
            }
        }

        private void OnCalendarClick(object sender, RoutedEventArgs e)
        {
            if (_allGood)
            {
                var proc = Process.Start(_pathToCalendarApp);
            }

        }

        private void OnAppointmentsClick(object sender, RoutedEventArgs e)
        {
            if (_allGood)
            {
                var proc = Process.Start(_pathToAppointmentsApp);
            }
        }

        private void OnTasksClick(object sender, RoutedEventArgs e)
        {
            if (_allGood)
            {
                var proc = Process.Start(_pathToTasksApp);
            }
        }
    }
}
