using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using TaskSharper.Domain.Calendar;
using TaskSharper.WPF.Common.Events;
using TaskSharper.WPF.Common.Media;
using TaskSharper.WPF.Common.Properties;

namespace TaskSharper.WPF.Common.Components.EventModification
{
    public class CategoryViewModel : BindableBase
    {
        private readonly IRegionManager _regionManager;
        private readonly IEventAggregator _eventAggregator;
        private readonly IEventRestClient _dataService;

        private string _id;
        private string _category;
        private string _categoryIcon;
        private double _categoryOpacity = Settings.Default.NotSelectedOpacity;

        public DelegateCommand SetCategoryCommand { get; set; }

        public string Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        public string Category
        {
            get => _category;
            set
            {
                CategoryIcon = CategoryToIconConverter.ConvertToFontAwesomeIcon(value, Type);
                SetProperty(ref _category, value);
            }
        }

        public EventType Type { get; set; }

        public string CategoryIcon
        {
            get => _categoryIcon;
            set => SetProperty(ref _categoryIcon, value);
        }

        public double CategoryOpacity
        {
            get => _categoryOpacity;
            set => SetProperty(ref _categoryOpacity, value);
        }

        public CategoryViewModel(IRegionManager regionManager, IEventAggregator eventAggregator, IEventRestClient dataService)
        {
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;
            _dataService = dataService;

            SetCategoryCommand = new DelegateCommand(SetCategory);
            _eventAggregator.GetEvent<CategoryClickedEvent>().Subscribe(CategoryChanged);
        }

        private void CategoryChanged(EventCategory eventCategory)
        {
            if (eventCategory.Id == Id)
            {
                CategoryOpacity = Settings.Default.SelectedOpacity;
            }
            else
            {
                CategoryOpacity = Settings.Default.NotSelectedOpacity;
            }
        }

        private void SetCategory()
        {
            var category = new EventCategory
            {
                Id = Id,
                Name = Category
            };

            _eventAggregator.GetEvent<CategoryClickedEvent>().Publish(category);
        }
    }
}
