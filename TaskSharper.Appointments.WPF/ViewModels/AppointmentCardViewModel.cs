using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Serilog;
using TaskSharper.Appointments.WPF.Events;
using TaskSharper.Domain.Calendar;

namespace TaskSharper.Appointments.WPF.ViewModels
{
    public class AppointmentCardViewModel : BindableBase
    {
        private readonly IAppointmentRestClient _dataService;
        private readonly IEventAggregator _eventAggregator;
        private readonly IRegionManager _regionManager;
        private readonly ILogger _logger;

        private Event _event;
        private bool _isSelected;
        private double _backgroundOpacity;

        public DelegateCommand SelectAppointmentCommand { get; set; }

        public Event Event
        {
            get => _event;
            set => SetProperty(ref _event, value);
        }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                BackgroundOpacity = value ? 0.5 : 0;
                SetProperty(ref _isSelected, value);
            }
        }

        public double BackgroundOpacity
        {
            get => _backgroundOpacity;
            set => SetProperty(ref _backgroundOpacity, value);
        }

        public AppointmentCardViewModel(IAppointmentRestClient dataService, IEventAggregator eventAggregator, IRegionManager regionManager, ILogger logger)
        {
            _dataService = dataService;
            _eventAggregator = eventAggregator;
            _regionManager = regionManager;
            _logger = logger.ForContext<AppointmentCardViewModel>();

            SelectAppointmentCommand = new DelegateCommand(SelectAppointment);

            _eventAggregator.GetEvent<AppointmentSelectedEvent>().Subscribe(eventObj =>
            {
                if (eventObj == null)
                {
                    IsSelected = false;
                }
                else
                {
                    if (eventObj.Id != Event.Id)
                    {
                        IsSelected = false;
                    }
                }
            });
        }

        private void SelectAppointment()
        {
            _eventAggregator.GetEvent<AppointmentSelectedEvent>().Publish(Event);
            IsSelected = true;
        }
    }
}
