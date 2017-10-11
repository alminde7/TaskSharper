using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TaskSharper.Calender.WPF.Views.Components
{
    /// <summary>
    /// Interaction logic for DateTimePickerView.xaml
    /// </summary>
    public partial class DateTimePickerView : UserControl
    {
        public DateTimePickerView()
        {
            InitializeComponent();
        }

        public object Date
        {
            get => GetValue(DateProperty);
            set => SetValue(DateProperty, value);
        }

        public static readonly DependencyProperty DateProperty =
            DependencyProperty.Register("Date", typeof(object), typeof(DateTimePickerView), new PropertyMetadata(null));
        

        private void DateChanged(object sender, CalendarDateChangedEventArgs e)
        {
            Date = e.AddedDate;
        }

        private void SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            var calendar = (Calendar) sender;
            Date = calendar.SelectedDate;
        }
    }
}
