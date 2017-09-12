using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Threading;
using System.Windows.Markup;

namespace TaskSharper.Calender.WPF
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {

            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;
            FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.Name)));
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;

            base.OnStartup(e);
            Bootstrapper bootstrapper = new Bootstrapper();
            bootstrapper.Run();
        }
    }
}
