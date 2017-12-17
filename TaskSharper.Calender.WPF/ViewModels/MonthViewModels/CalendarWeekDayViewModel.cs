using Prism.Events;
using Prism.Mvvm;
using System;
using System.Globalization;
using TaskSharper.WPF.Common.Events;

namespace TaskSharper.Calender.WPF.ViewModels.MonthViewModels
{
    /// <summary>
    /// This is the ViewModel for the view that handles showing the weekdays in the top of the calendar. 
    /// 
    /// </summary>
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

        /// <summary>
        /// The cosntructor receives which day of the week the object is. so monday is 0 and sunday is 7. 
        /// </summary>
        /// <param name="dayofweek">Day of the week going from 0-7</param>
        /// <param name="eventAggregator"></param>
        public CalendarWeekDayViewModel(int dayofweek, IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _dayOfWeek = dayofweek;
            if (_dayOfWeek == 7)
                _dayOfWeek = 0;

            eventAggregator.GetEvent<CultureChangedEvent>().Subscribe(CultureChangedEventHandler);
            CultureChangedEventHandler();
        }

        /// <summary>
        /// Depending on the dayofweek number and the culture, then the correct string will be set to the day property 
        /// a dayofweek number with a 0 and culture english will become Monday. 
        /// </summary>
        private void CultureChangedEventHandler()
        {
            Day = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(DateTimeFormatInfo.CurrentInfo.GetDayName(((DayOfWeek)Enum.ToObject(typeof(DayOfWeek), _dayOfWeek))));
        }
    }
}
