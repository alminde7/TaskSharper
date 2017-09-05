using System.Windows;
using Prism.Unity;
using Microsoft.Practices.Unity;
using TaskSharper.Calender.WPF.Views;

namespace TaskSharper.Calender.WPF
{
    public class Bootstrapper : UnityBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void InitializeShell()
        {
            Application.Current.MainWindow.Show();
        }
        protected override void ConfigureModuleCatalog()
        {
            base.ConfigureModuleCatalog();

            Container.RegisterTypeForNavigation<CalendarAppointmentsView>("CalendarAppointmentsView");
        }
    }
}
