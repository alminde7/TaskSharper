using Prism.Events;
using Prism.Mvvm;
using System;
using System.Globalization;
using TaskSharper.WPF.Common.Events;

namespace TaskSharper.Calender.WPF.ViewModels.MonthViewModels
{
    public class CalendarWeekDayViewModel : BindableBase
    {
        private readonly IEventAggregator _eventAggregator;
        private string day;
        private int _dayOfWeek;

        public string Day
        {
            get => day;
            set => SetProperty(ref day, value);
        }

        public CalendarWeekDayViewModel(int dayofweek, IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _dayOfWeek = dayofweek;
            if (_dayOfWeek == 7)
                _dayOfWeek = 0;

            eventAggregator.GetEvent<CultureChangedEvent>().Subscribe(CultureChangedEventHandler);
            CultureChangedEventHandler();
        }

        private void CultureChangedEventHandler()
        {
            Day = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(DateTimeFormatInfo.CurrentInfo.GetDayName(((DayOfWeek)Enum.ToObject(typeof(DayOfWeek), _dayOfWeek))));
        }
    }
}
