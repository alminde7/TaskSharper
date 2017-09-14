using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

namespace TaskSharper.Calender.WPF.ViewModels
{
    public class CalendarEventDetailsViewModel : BindableBase
    {
        public DelegateCommand BackCommand { get; set; }
        private readonly IRegionManager _regionManager;
        public CalendarEventDetailsViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            BackCommand = new DelegateCommand(Back);
        }

        private void Back()
        {
            _regionManager.Regions["CalendarRegion"].NavigationService.Journal.GoBack();
        }
    }
}
