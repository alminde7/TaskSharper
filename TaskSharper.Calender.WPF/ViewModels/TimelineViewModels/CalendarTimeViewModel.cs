using System.Windows.Media;
using Prism.Mvvm;

namespace TaskSharper.Calender.WPF.ViewModels
{
    public class CalendarTimeViewModel : BindableBase
    {
        private int _hour;
        private Brush _backgroundColor;

        public int Hour
        {
            get => _hour;
            set => SetProperty(ref _hour, value);
        }

        public Brush BackgroundColor
        {
            get => _backgroundColor;
            set => SetProperty(ref _backgroundColor, value);
        }
    }
}
