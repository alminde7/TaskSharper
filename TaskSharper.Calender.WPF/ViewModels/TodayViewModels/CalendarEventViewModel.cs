using Prism.Mvvm;

namespace TaskSharper.Calender.WPF.ViewModels
{
    public class CalendarEventViewModel : BindableBase
    {
        private string _title;
        private string _description;

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        public CalendarEventViewModel()
        {
             
        }
    }
}
