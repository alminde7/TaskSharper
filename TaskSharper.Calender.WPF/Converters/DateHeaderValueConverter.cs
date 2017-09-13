using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using TaskSharper.Calender.WPF.ViewModels;

namespace TaskSharper.Calender.WPF.Converters
{
    public class DateHeaderValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var dateViewModel = value as CalendarDateViewModel;
            int number = 0;
            if (parameter != null)
            {
                number = (int)parameter;
                return null;
            }

            var dayOffset = number - (int)DateTime.Now.DayOfWeek;

            var dateTime = DateTime.Now.AddDays(dayOffset);

            if (dateViewModel != null)
            {
                dateViewModel.DayOfMonth = dateTime.Day;

                return dateViewModel;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
