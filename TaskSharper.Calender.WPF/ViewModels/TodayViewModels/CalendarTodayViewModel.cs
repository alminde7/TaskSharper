using Prism.Regions;

namespace TaskSharper.Calender.WPF.ViewModels
{
    public class CalendarTodayViewModel : INavigationAware
    {
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }
    }
}
