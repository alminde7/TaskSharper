using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interactivity;
using System.Windows.Markup;

namespace TaskSharper.Calender.WPF.Converters
{
    // Inspired by https://stackoverflow.com/a/20326885/6796072
    public class WidthConverter : MarkupExtension, IMultiValueConverter
    {
        private static WidthConverter _instance;

        #region IValueConverter Members

        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value[0].GetType() != typeof(double)) return value[0];
            if (value[1].GetType() != typeof(double)) return value[0];

            return System.Convert.ToDouble(value[0]) / System.Convert.ToDouble(value[1]);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _instance ?? (_instance = new WidthConverter());
        }
    }
}
