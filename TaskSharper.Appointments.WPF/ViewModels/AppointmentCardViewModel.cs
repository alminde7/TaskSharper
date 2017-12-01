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
using TaskSharper.WPF.Common.Media;

namespace TaskSharper.Appointments.WPF.ViewModels
{
    /// <inheritdoc />
    /// <summary>
    /// ViewModel for the appointment card.
    /// </summary>
    public class AppointmentCardViewModel : BindableBase
    {
        private readonly IAppointmentRestClient _dataService;
        private readonly IEventAggregator _eventAggregator;
        private readonly IRegionManager _regionManager;
        private readonly ILogger _logger;

        private Event _appointment;
        private string _category;
        private bool _isSelected;
        private double _backgroundOpacity;

        public DelegateCommand SelectAppointmentCommand { get; set; }
        public DelegateCommand EditAppointmentCommand { get; set; }

        /// <summary>
        /// Holds the appointment used for data binding.
        /// </summary>
        public Event Appointment
        {
            get => _appointment;
            set
            {
                Category = CategoryToIconConverter.ConvertToFontAwesomeIcon(value?.Category.Name, (EventType)value?.Type);
                SetProperty(ref _appointment, value);
            }
        }

        /// <summary>
        /// Category of the appointment as a FontAwesome valid value.
        /// </summary>
        public string Category
        {
            get => _category;
            set => SetProperty(ref _category, value);
        }

        /// <summary>
        /// Determines whether or not the appointment is selected, and adds a background color if it is selected.
        /// </summary>
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                BackgroundOpacity = value ? 0.5 : 0;
                SetProperty(ref _isSelected, value);
            }
        }

        /// <summary>
        /// Binding value used for opacity of the background. Ranges between 0.0-1.0.
        /// Default value for when appointment is selected: 0.5
        /// Default value for when appointment is not selected: 0
        /// </summary>
        public double BackgroundOpacity
        {
            get => _backgroundOpacity;
            set => SetProperty(ref _backgroundOpacity, value);
        }


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="dataService">Data service for data management</param>
        /// <param name="eventAggregator">Event aggregator for subscribing to and publishing events</param>
        /// <param name="regionManager">Regionmanager used for navigation</param>
        /// <param name="logger">Logger for logging</param>
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

        /// <summary>
        /// Handler for when clicking the Edit button in the view.
        /// </summary>
        private void EditAppointment()
        {
            var navigationParameters = new NavigationParameters();
            navigationParameters.Add("Id", Appointment.Id);
            navigationParameters.Add("Type", EventType.Appointment);
            navigationParameters.Add("Region", ViewConstants.REGION_Main);
            _regionManager.RequestNavigate(ViewConstants.REGION_Main, ViewConstants.VIEW_ModifyAppointmentView, navigationParameters);
        }

        /// <summary>
        /// Handler for selecting a appointment in the view.
        /// </summary>
        private void SelectAppointment()
        {
            _eventAggregator.GetEvent<AppointmentSelectedEvent>().Publish(Appointment);
            IsSelected = true;
        }
    }
}
