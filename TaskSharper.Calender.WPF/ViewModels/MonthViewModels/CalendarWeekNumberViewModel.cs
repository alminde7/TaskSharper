using System;
using System.Globalization;
using Prism.Events;
using Prism.Mvvm;
using TaskSharper.WPF.Common.Events;
using WPFLocalizeExtension.Engine;

namespace TaskSharper.Calender.WPF.ViewModels
{
    /// <summary>
    /// This is the ViewModel of the CalendarWeekNumberView. 
    /// It handles showcasing the correct weeknumber in the view. 
    /// </summary>
    public class CalendarWeekNumberViewModel : BindableBase
    {
        private DateTime _date;
        private int _week;
        private string _weekText;

        public string WeekText
        {
            get => _weekText;
            set => SetProperty(ref _weekText, value);
        }

        public DateTime Date
        {
            get => _date;
            set => SetProperty(ref _date, value);
        }

        public int Week
        {
            get => _week;
            set => SetProperty(ref _week, value);
        }

        public CalendarWeekNumberViewModel(DateTime date, IEventAggregator eventAggregator)
        {
            SetDate(date);
            eventAggregator.GetEvent<CultureChangedEvent>().Subscribe(UpdateCultureHandler);
        }

        private void UpdateCultureHandler()
        {
            SetDate(Date);
        }

        /// <summary>
        /// From the date the week of the year is found, it takes into account that the day of week must be monday.
        /// this makes sure that it is always the correct Week number that is found.
        /// </summary>
        /// <param name="date">The datetime object of the date</param>
        public void SetDate(DateTime date)
        {
            Week = DateTimeFormatInfo.CurrentInfo.Calendar.GetWeekOfYear(date, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday);
            Date = date;
            WeekText = LocalizeDictionary.Instance
                .GetLocalizedObject("Week", null, LocalizeDictionary.Instance.Culture)
                .ToString();
        }
    }
}
