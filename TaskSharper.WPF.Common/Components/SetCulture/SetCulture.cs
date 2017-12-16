using System;
using System.Diagnostics;
using System.Globalization;
using WPFLocalizeExtension.Engine;

namespace TaskSharper.WPF.Common.Components.SetCulture
{
    /// <summary>
    /// LocalizeDictionary can throw an unexpected exception, it have no effect on the performance 
    /// of changing the culture resources. Therefore a helper class is made to catch the unexpected exception.
    /// </summary>
    public class Culture
    {
        /// <summary>
        /// The Set() method receives the culture string and sets the culture string into the LocalizeDictionary object.
        /// Thereby changeing the culture of the application. 
        /// </summary>
        /// <param name="culture"></param>
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
