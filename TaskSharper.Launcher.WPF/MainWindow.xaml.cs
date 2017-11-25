using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using TaskSharper.Configuration.Config;
using TaskSharper.Configuration.Settings;
using TaskSharper.DataAccessLayer.Google.Authentication;
using TaskSharper.Domain.Configuration.Logging;
using TaskSharper.Shared.Logging;

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
        public bool LoggedIn = false;

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
                var credPath = Path.Combine(Shared.Configuration.Config.TaskSharperCredentialStore, "calendar.json");
                LoggedIn = Directory.GetFiles(credPath, "*.TokenResponse-user").Length > 0;
                LoginLogoutBtn.Content = LoggedIn ? "Log out" : "Log in";
                WelcomeLabel.Content = LoggedIn ? "Welcome to TaskSharper!" : "Please login";
                CalendarApplicationButton.IsEnabled = LoggedIn;
                AppointmentApplicationButton.IsEnabled = LoggedIn;
                TaskApplicationButton.IsEnabled = LoggedIn;
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

        private void LoginLogoutBtn_OnClick(object sender, RoutedEventArgs e)
        {
            var credPath = Path.Combine(Shared.Configuration.Config.TaskSharperCredentialStore, "calendar.json");
            if (LoggedIn)
            {
                var tokenFiles = Directory.GetFiles(credPath, "*.TokenResponse-user");
                if (tokenFiles.Length > 0)
                {
                    var fileToDelete = tokenFiles.FirstOrDefault();
                    if (File.Exists(fileToDelete))
                    {
                        if (fileToDelete != null) File.Delete(fileToDelete);
                    }
                }
                LoggedIn = false;
            }
            else
            {
                var authentication = new GoogleAuthentication(LogConfiguration.ConfigureWPF(new LoggingSettingsHandler().Load()));
                var credential = authentication.Authenticate();
                if (credential.Token != null)
                {
                    LoggedIn = true;
                }
            }

            LoginLogoutBtn.Content = LoggedIn ? "Log out" : "Log in";
            WelcomeLabel.Content = LoggedIn ? "Welcome to TaskSharper!" : "Please login";
            CalendarApplicationButton.IsEnabled = LoggedIn;
            AppointmentApplicationButton.IsEnabled = LoggedIn;
            TaskApplicationButton.IsEnabled = LoggedIn;
        }
    }
}
