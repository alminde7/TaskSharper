using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Threading;
using System.Windows.Markup;
using WPFLocalizeExtension.Engine;

namespace TaskSharper.Calender.WPF
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            
            // https://github.com/SeriousM/WPFLocalizationExtension

            LocalizeDictionary.Instance.SetCurrentThreadCulture = true;
            LocalizeDictionary.Instance.Culture = new CultureInfo("da-DK");

            // Use this to check specific culture settings - check culture code here: https://msdn.microsoft.com/en-us/library/ee825488(v=cs.20).aspx
            
            base.OnStartup(e);
            Bootstrapper bootstrapper = new Bootstrapper();
            bootstrapper.Run();
        }
    }
}
