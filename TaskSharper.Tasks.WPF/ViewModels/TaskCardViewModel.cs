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
using TaskSharper.Tasks.WPF.Config;
using TaskSharper.Tasks.WPF.Events;

namespace TaskSharper.Tasks.WPF.ViewModels
{
    public class TaskCardViewModel : BindableBase
    {
        private readonly ITaskRestClient _dataService;
        private readonly IEventAggregator _eventAggregator;
        private readonly IRegionManager _regionManager;
        private readonly ILogger _logger;

        private Event _task;
        private bool _isSelected;
        private double _backgroundOpacity;

        public DelegateCommand SelectTaskCommand { get; set; }
        public DelegateCommand EditTaskCommand { get; set; }
        public DelegateCommand SaveTaskStateCommand { get; set; }

        public Event Task
        {
            get => _task;
            set => SetProperty(ref _task, value);
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
        }

        private async void SaveTaskState()
        {
            Task = await _dataService.UpdateAsync(Task);
        }

        private void EditTask()
        {
            var navigationParameters = new NavigationParameters();
            navigationParameters.Add("Id", Task.Id);
            navigationParameters.Add("Type", EventType.Task);
            navigationParameters.Add("Region", ViewConstants.REGION_Main);
            _regionManager.RequestNavigate(ViewConstants.REGION_Main, ViewConstants.VIEW_ModifyTaskView, navigationParameters);
        }

        private void SelectTask()
        {
            _eventAggregator.GetEvent<TaskSelectedEvent>().Publish(Task);
            IsSelected = true;
        }
    }
}
