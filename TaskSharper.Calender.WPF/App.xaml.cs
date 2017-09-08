using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Markup;

namespace TaskSharper.Calender.WPF
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Bootstrapper bootstrapper = new Bootstrapper();
            bootstrapper.Run();
        }
    }
}
