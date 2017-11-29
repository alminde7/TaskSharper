using System;
using System.Diagnostics;
using System.Globalization;
using WPFLocalizeExtension.Engine;

namespace TaskSharper.WPF.Common.Components.SetCulture
{
    public class Culture
    {
        public void Set(string culture)
        {
            try
            {
                LocalizeDictionary.Instance.Culture = new CultureInfo(culture);
            }
            catch (Exception e)
            {

            }
            
        }
    }
}
