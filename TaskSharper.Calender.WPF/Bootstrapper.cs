using System.Windows;
using Prism.Unity;
using Microsoft.Practices.Unity;
using TaskSharper.Calender.WPF.ViewComponents;

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

            Container.RegisterType(typeof(object), typeof(CalendarAppointmentsComponent), "CalendarAppointmentsComponent");

            Container.RegisterTypeForNavigation<CalendarAppointmentsComponent>("CalendarAppointmentsComponent");
        }
    }
    public static class UnityExtensions
    {
        public static void RegisterTypeForNavigation<T>(this IUnityContainer container, string name)
        {
            container.RegisterType(typeof(object), typeof(T), name);
        }
    }
}
