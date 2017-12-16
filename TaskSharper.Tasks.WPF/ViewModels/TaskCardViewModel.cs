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
using TaskSharper.Domain.Calendar;
using TaskSharper.Domain.Models;
using TaskSharper.Domain.RestClient;
using TaskSharper.Tasks.WPF.Config;
using TaskSharper.Tasks.WPF.Events;
using TaskSharper.WPF.Common.Events;
using TaskSharper.WPF.Common.Media;

namespace TaskSharper.Tasks.WPF.ViewModels
{
    /// <inheritdoc />
    /// <summary>
    /// ViewModel for the task card.
    /// </summary>
    public class TaskCardViewModel : BindableBase
    {
        private readonly ITaskRestClient _dataService;
        private readonly IEventAggregator _eventAggregator;
        private readonly IRegionManager _regionManager;
        private readonly ILogger _logger;

        private Event _task;
        private string _category;
        private bool _isSelected;
        private double _backgroundOpacity;

        public DelegateCommand SelectTaskCommand { get; set; }
        public DelegateCommand EditTaskCommand { get; set; }
        public DelegateCommand SaveTaskStateCommand { get; set; }

        /// <summary>
        /// Holds the task used for data binding.
        /// </summary>
        public Event Task
        {
            get => _task;
            set
            {
                Category = CategoryToIconConverter.ConvertToFontAwesomeIcon(value?.Category.Name, (EventType)value?.Type);
                SetProperty(ref _task, value);
            }
        }

        /// <summary>
        /// Category of the task as a FontAwesome valid value.
        /// </summary>
        public string Category
        {
            get => _category;
            set => SetProperty(ref _category, value);
        }

        /// <summary>
        /// Determines whether or not the task is selected, and adds a background color if it is selected.
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
        /// Default value for when task is selected: 0.5
        /// Default value for when task is not selected: 0
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
        public TaskCardViewModel(ITaskRestClient dataService, IEventAggregator eventAggregator, IRegionManager regionManager, ILogger logger)
        {
            _dataService = dataService;
            _eventAggregator = eventAggregator;
            _regionManager = regionManager;
            _logger = logger.ForContext<TaskCardViewModel>();

            SelectTaskCommand = new DelegateCommand(SelectTask);
            EditTaskCommand = new DelegateCommand(EditTask);
            SaveTaskStateCommand = new DelegateCommand(SaveTaskState);

            _eventAggregator.GetEvent<TaskSelectedEvent>().Subscribe(eventObj =>
            {
                if (eventObj == null)
                {
                    IsSelected = false;
                }
                else
                {
                    if (eventObj.Id != Task.Id)
                    {
                        IsSelected = false;
                    }
                }
            });

            _eventAggregator.GetEvent<EventChangedEvent>().Subscribe(eventObj =>
            {
                if (eventObj == null) return;
                if (eventObj.Id == Task.Id)
                {
                    Task = eventObj;
                }
            });
        }

        /// <summary>
        /// Updates the task.
        /// Used when marking a task as completed.
        /// </summary>
        private async void SaveTaskState()
        {
            Task = await _dataService.UpdateAsync(Task);
        }

        /// <summary>
        /// Handler for when clicking the Edit button in the view.
        /// </summary>
        private void EditTask()
        {
            var navigationParameters = new NavigationParameters();
            navigationParameters.Add("Id", Task.Id);
            navigationParameters.Add("Type", EventType.Task);
            navigationParameters.Add("Region", ViewConstants.REGION_Main);
            _regionManager.RequestNavigate(ViewConstants.REGION_Main, ViewConstants.VIEW_ModifyTaskView, navigationParameters);
        }

        /// <summary>
        /// Handler for selecting a task in the view.
        /// </summary>
        private void SelectTask()
        {
            _eventAggregator.GetEvent<TaskSelectedEvent>().Publish(Task);
            IsSelected = true;
        }
    }
}
