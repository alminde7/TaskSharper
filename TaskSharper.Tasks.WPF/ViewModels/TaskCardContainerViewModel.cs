using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using TaskSharper.WPF.Common.Events.ViewEvents;

namespace TaskSharper.Tasks.WPF.ViewModels
{
    public class TaskCardContainerViewModel : BindableBase, INavigationAware
    {
        private readonly ITaskRestClient _dataService;
        private readonly IEventAggregator _eventAggregator;
        private readonly IRegionManager _regionManager;
        private readonly ILogger _logger;
        private bool _isTaskSelected;
        private Event _selectedTask;

        public ObservableCollection<TaskCardViewModel> TaskCards { get; set; }

        public DelegateCommand AddTaskCommand { get; set; }
        public DelegateCommand DeleteTaskCommand { get; set; }

        public bool IsTaskSelected
        {
            get => _isTaskSelected;
            set => SetProperty(ref _isTaskSelected, value);
        }
        public Event SelectedTask
        {
            get => _selectedTask;
            set
            {
                IsTaskSelected = value != null;
                SetProperty(ref _selectedTask, value);
            }
        }

        public TaskCardContainerViewModel(ITaskRestClient dataService, IEventAggregator eventAggregator, IRegionManager regionManager, ILogger logger)
        {
            _dataService = dataService;
            _eventAggregator = eventAggregator;
            _regionManager = regionManager;
            _logger = logger.ForContext<TaskCardContainerViewModel>();

            TaskCards = new ObservableCollection<TaskCardViewModel>();

            AddTaskCommand = new DelegateCommand(NavigateToAddTask);
            DeleteTaskCommand = new DelegateCommand(DeleteTask);

            _eventAggregator.GetEvent<TaskSelectedEvent>().Subscribe(eventObj =>
            {
                SelectedTask = eventObj;
            });
        }

        private void NavigateToAddTask()
        {
            var navigationParameters = new NavigationParameters();
            navigationParameters.Add("Type", EventType.Task);
            navigationParameters.Add("Region", ViewConstants.REGION_Main);
            _regionManager.RequestNavigate(ViewConstants.REGION_Main, ViewConstants.VIEW_ModifyTaskView, navigationParameters);
        }

        private async void DeleteTask()
        {
            try
            {
                await _dataService.DeleteAsync(SelectedTask.Id);
                await UpdateView();
                IsTaskSelected = false;
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error while deleting an appointment.");
            }
        }

        private async Task UpdateView()
        {
            var start = DateTime.Today.AddDays(-7).Date;
            var end = DateTime.Today.AddDays(7).Date;
            var events = await _dataService.GetAsync(start, end);

            TaskCards?.Clear();
            foreach (var @event in events)
            {
                TaskCards?.Add(new TaskCardViewModel(_dataService, _eventAggregator, _regionManager, _logger)
                {
                    Task = @event
                });
            }
        }

        public async void OnNavigatedTo(NavigationContext navigationContext)
        {
            _eventAggregator.GetEvent<BackButtonEvent>().Publish(BackButtonStatus.Hide);
            await UpdateView();
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            _eventAggregator.GetEvent<BackButtonEvent>().Publish(BackButtonStatus.Show);
        }
    }
}
