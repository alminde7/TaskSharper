using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;

namespace TaskSharper.Calender.WPF.ViewModels
{
    public class CalendarWeekViewModel : BindableBase
    {
        public ObservableCollection<CalendarDateViewModel> DateHeaders { get; set; }

        public CalendarWeekViewModel()
        {
            DateHeaders = new ObservableCollection<CalendarDateViewModel>();

            for (int i = 0; i < 7; i++)
            {
                DateHeaders.Add(new CalendarDateViewModel(i));
            }
        }
    }
}
