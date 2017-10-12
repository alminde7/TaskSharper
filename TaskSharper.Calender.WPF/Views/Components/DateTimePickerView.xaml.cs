using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using TaskSharper.Shared.Constants;

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

            for (int hour = 0; hour < Time.HoursInADay; hour++)
            {
                HourListBox.Items.Add(hour);
            }

            for (int minute = 0; minute < Time.MinutesInAnHour; minute++)
            {
                MinuteListBox.Items.Add(minute);
            }
        }

        public object Date
        {
            get => GetValue(DateProperty);
            set => SetValue(DateProperty, value);
        }

        public static readonly DependencyProperty DateProperty =
            DependencyProperty.Register("Date", typeof(object), typeof(DateTimePickerView), new PropertyMetadata(null));
        
        private void SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            var calendar = (Calendar) sender;
            var selectedDate = calendar.SelectedDate.Value;
            
            Date = new DateTime(selectedDate.Year, selectedDate.Month, selectedDate.Day) + ((DateTime)Date).TimeOfDay;
        }

        private void CalendarLoaded(object sender, RoutedEventArgs e)
        {
            DatePickerCalendar.SelectedDate = (DateTime) Date;
            DatePickerCalendar.DisplayDate = ((DateTime) Date).Date;

            HourListBox.SelectedIndex = ((DateTime)Date).Hour;
            MinuteListBox.SelectedIndex = ((DateTime) Date).Minute;

            HourListBox.ScrollToCenterOfView(HourListBox.Items[HourListBox.SelectedIndex]);
            MinuteListBox.ScrollToCenterOfView(MinuteListBox.Items[MinuteListBox.SelectedIndex]);
        }

        private void HourChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedHour = (int) ((ListBox) sender).SelectedItem;
            var currentDate = (DateTime) Date;

            Date = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, selectedHour, currentDate.Minute, currentDate.Second);
            HourListBox.ScrollToCenterOfView(HourListBox.SelectedItem);
        }

        private void MinuteChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedMinute = (int)((ListBox)sender).SelectedItem;
            var currentDate = (DateTime)Date;

            Date = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, currentDate.Hour, selectedMinute, currentDate.Second);
            MinuteListBox.ScrollToCenterOfView(MinuteListBox.SelectedItem);
        }

        private void HourScrollUp(object sender, MouseButtonEventArgs e)
        {
            HourListBox.DecrementIndex(0, 23, 1);
        }

        private void HourScrollDown(object sender, MouseButtonEventArgs e)
        {
            HourListBox.IncrementIndex(0, 23, 1);
        }

        private void MinuteScrollUp(object sender, MouseButtonEventArgs e)
        {
            MinuteListBox.DecrementIndex(0, 59, 5);
        }

        private void MinuteScrollDown(object sender, MouseButtonEventArgs e)
        {
            MinuteListBox.IncrementIndex(0, 59, 5);
        }

        void ScrollViewer_ManipulationBoundaryFeedback(object sender, ManipulationBoundaryFeedbackEventArgs e)
        {
            e.Handled = true;
        }
    }

    
    public static class ItemsControlExtensions
    {
        public static void IncrementIndex(this ListBox listBox, int minValue, int maxValue, int step)
        {
            listBox.SelectedIndex = listBox.SelectedIndex == maxValue ? minValue : (listBox.SelectedIndex + step > maxValue ? maxValue : listBox.SelectedIndex + step);
            
        }

        public static void DecrementIndex(this ListBox listBox, int minValue, int maxValue, int step)
        {
            listBox.SelectedIndex = listBox.SelectedIndex == minValue ? maxValue : (listBox.SelectedIndex - step < minValue ? minValue : listBox.SelectedIndex - step);
        }

        // From https://stackoverflow.com/a/3002013/6796072
        public static void ScrollToCenterOfView(this ItemsControl itemsControl, object item)
        {
            // Scroll immediately if possible
            if (!itemsControl.TryScrollToCenterOfView(item))
            {
                // Otherwise wait until everything is loaded, then scroll
                if (itemsControl is ListBox) ((ListBox)itemsControl).ScrollIntoView(item);
                itemsControl.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(() =>
                {
                    itemsControl.TryScrollToCenterOfView(item);
                }));
            }
        }

        private static bool TryScrollToCenterOfView(this ItemsControl itemsControl, object item)
        {
            // Find the container
            var container = itemsControl.ItemContainerGenerator.ContainerFromItem(item) as UIElement;
            if (container == null) return false;

            // Find the ScrollContentPresenter
            ScrollContentPresenter presenter = null;
            for (Visual vis = container; vis != null && vis != itemsControl; vis = VisualTreeHelper.GetParent(vis) as Visual)
                if ((presenter = vis as ScrollContentPresenter) != null)
                    break;
            if (presenter == null) return false;

            // Find the IScrollInfo
            var scrollInfo =
                !presenter.CanContentScroll ? presenter :
                presenter.Content as IScrollInfo ??
                FirstVisualChild(presenter.Content as ItemsPresenter) as IScrollInfo ??
                presenter;

            // Compute the center point of the container relative to the scrollInfo
            Size size = container.RenderSize;
            Point center = container.TransformToAncestor((Visual)scrollInfo).Transform(new Point(size.Width / 2, size.Height / 2));
            center.Y += scrollInfo.VerticalOffset;
            center.X += scrollInfo.HorizontalOffset;

            // Adjust for logical scrolling
            if (scrollInfo is StackPanel || scrollInfo is VirtualizingStackPanel)
            {
                double logicalCenter = itemsControl.ItemContainerGenerator.IndexFromContainer(container) + 0.5;
                Orientation orientation = scrollInfo is StackPanel ? ((StackPanel)scrollInfo).Orientation : ((VirtualizingStackPanel)scrollInfo).Orientation;
                if (orientation == Orientation.Horizontal)
                    center.X = logicalCenter;
                else
                    center.Y = logicalCenter;
            }

            // Scroll the center of the container to the center of the viewport
            if (scrollInfo.CanVerticallyScroll) scrollInfo.SetVerticalOffset(CenteringOffset(center.Y, scrollInfo.ViewportHeight, scrollInfo.ExtentHeight));
            if (scrollInfo.CanHorizontallyScroll) scrollInfo.SetHorizontalOffset(CenteringOffset(center.X, scrollInfo.ViewportWidth, scrollInfo.ExtentWidth));
            return true;
        }

        private static double CenteringOffset(double center, double viewport, double extent)
        {
            return Math.Min(extent - viewport, Math.Max(0, center - viewport / 2));
        }
        private static DependencyObject FirstVisualChild(Visual visual)
        {
            if (visual == null) return null;
            if (VisualTreeHelper.GetChildrenCount(visual) == 0) return null;
            return VisualTreeHelper.GetChild(visual, 0);
        }
    }
}
