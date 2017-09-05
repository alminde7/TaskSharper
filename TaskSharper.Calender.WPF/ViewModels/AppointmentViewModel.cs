using System;
using Prism.Mvvm;

namespace TaskSharper.Calender.WPF.ViewModels
{
    public class AppointmentViewModel : BindableBase
    {
        private string _headline;
        private string _description;
        private DateTime _from;
        private DateTime _to;

        public string Headline
        {
            get => _headline;
            set => SetProperty(ref _headline, value);
        }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        public DateTime From
        {
            get => _from;
            set => SetProperty(ref _from, value);
        }

        public DateTime To
        {
            get => _to;
            set => SetProperty(ref _to, value);
        }
        
    }
}