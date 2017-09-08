using System.Windows;
using Prism.Unity;
using Microsoft.Practices.Unity;
using Prism.Regions;
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

            // Set default Calendar on start.
            var regionManager = Container.Resolve<IRegionManager>();
            regionManager.RequestNavigate("CalendarRegion", "CalendarWeekView");

        }
        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();

            Container.RegisterTypeForNavigation<CalendarTodayView>("CalendarTodayView");
            Container.RegisterTypeForNavigation<CalendarWeekView>("CalendarWeekView");
            Container.RegisterTypeForNavigation<CalendarMonthView>("CalendarMonthView");
        }
    }
}
