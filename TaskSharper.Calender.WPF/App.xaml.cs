using System.Diagnostics;
using System.Globalization;
using System.Windows;

namespace TaskSharper.Calender.WPF
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Bootstrapper bootstrapper = new Bootstrapper();
            bootstrapper.Run();

            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("dk");
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("dk");
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("dk");
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("dk");
        }
    }
}
