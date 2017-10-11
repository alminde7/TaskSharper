using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using TaskSharper.Domain.Calendar;
using TaskSharper.Shared.Constants;

namespace TaskSharper.Calender.WPF.ViewModels.Components
{
    public class DateTimePickerViewModel : BindableBase
    {
        private List<int> _hours;
        private List<int> _minutes;
        private Event _event;
        private string _startOrEnd;

        public List<int> Hours
        {
            get => _hours;
            set => SetProperty(ref _hours, value);
        }

        public List<int> Minutes
        {
            get => _minutes;
            set => SetProperty(ref _minutes, value);
        }

        public DateTime SelectedDateTime
        {
            get
            {
                switch (_startOrEnd.ToLower())
                {
                    case "start":
                        return _event.Start.Value;
                    case "end":
                        return _event.End.Value;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            set
            {
                switch (_startOrEnd.ToLower())
                {
                    case "start":
                        _event.Start = value;
                        break;
                    case "end":
                        _event.End = value;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public DateTimePickerViewModel(ref Event eventObj, string startOrEnd)
        {
            _startOrEnd = startOrEnd;
            _event = eventObj;

            InitializeView();
        }

        void InitializeView()
        {
            Hours = new List<int>();
            Minutes = new List<int>();
            for (var hour = 0; hour < Time.HoursInADay; hour++)
            {
                Hours.Add(hour);
            }

            for (int minute = 0; minute < Time.MinutesInAnHour; minute++)
            {
                Minutes.Add(minute);
            }
        }
    }
}
