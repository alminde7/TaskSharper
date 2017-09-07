using Prism.Mvvm;

namespace TaskSharper.Calender.WPF.ViewModels
{
    public class CalendarTimeViewModel : BindableBase
    {
        private int _hour;

        public int Hour
        {
            get => _hour;
            set => SetProperty(ref _hour, value);
        }
    }
}
