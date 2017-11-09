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
using TaskSharper.Appointments.WPF.Config;
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

        private Event _appointment;
        private bool _isSelected;
        private double _backgroundOpacity;

        public DelegateCommand SelectAppointmentCommand { get; set; }
        public DelegateCommand EditAppointmentCommand { get; set; }

        public Event Appointment
        {
            get => _appointment;
            set => SetProperty(ref _appointment, value);
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
            EditAppointmentCommand = new DelegateCommand(EditAppointment);

            _eventAggregator.GetEvent<AppointmentSelectedEvent>().Subscribe(eventObj =>
            {
                if (eventObj == null)
                {
                    IsSelected = false;
                }
                else
                {
                    if (eventObj.Id != Appointment.Id)
                    {
                        IsSelected = false;
                    }
                }
            });
        }

        private void EditAppointment()
        {
            var navigationParameters = new NavigationParameters();
            navigationParameters.Add("Id", Appointment.Id);
            navigationParameters.Add("Type", EventType.Appointment);
            navigationParameters.Add("Region", ViewConstants.REGION_Main);
            _regionManager.RequestNavigate(ViewConstants.REGION_Main, ViewConstants.VIEW_ModifyAppointmentView, navigationParameters);
        }

        private void SelectAppointment()
        {
            _eventAggregator.GetEvent<AppointmentSelectedEvent>().Publish(Appointment);
            IsSelected = true;
        }
    }
}
