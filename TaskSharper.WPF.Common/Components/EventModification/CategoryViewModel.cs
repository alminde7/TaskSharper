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
    /// <inheritdoc />
    /// <summary>
    /// ViewModel for Categories used in the EventModification component.
    /// </summary>
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

        /// <summary>
        /// Id of the category.
        /// When using Google Calendar as Calendar Service Provider, this is the calendar id.
        /// </summary>
        public string Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        /// <summary>
        /// Name of the category.
        /// Converts this string to a string matching a FontAwesome icon if possible - otherwise it uses a default value.
        /// </summary>
        public string Category
        {
            get => _category;
            set
            {
                CategoryIcon = CategoryToIconConverter.ConvertToFontAwesomeIcon(value, Type);
                SetProperty(ref _category, value);
            }
        }

        /// <summary>
        /// Type of the event.
        /// </summary>
        public EventType Type { get; set; }

        /// <summary>
        /// Name of the FontAwesome icon to show in the view.
        /// </summary>
        public string CategoryIcon
        {
            get => _categoryIcon;
            set => SetProperty(ref _categoryIcon, value);
        }

        /// <summary>
        /// Binding value used for opacity of the category icon. Ranges between 0.0-1.0.
        /// Default value for when the category is selected: 1.0
        /// Default value for when the category is not selected: 0.5
        /// </summary>
        public double CategoryOpacity
        {
            get => _categoryOpacity;
            set => SetProperty(ref _categoryOpacity, value);
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="regionManager">Regionmanager used for navigation</param>
        /// <param name="eventAggregator">Event aggregator for subscribing to and publishing events</param>
        /// <param name="dataService">Data service for data management</param>
        public CategoryViewModel(IRegionManager regionManager, IEventAggregator eventAggregator, IEventRestClient dataService)
        {
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;
            _dataService = dataService;

            SetCategoryCommand = new DelegateCommand(SetCategory);
            _eventAggregator.GetEvent<CategoryClickedEvent>().Subscribe(CategoryChanged);
        }

        /// <summary>
        /// Handler for the event when a category has changed.
        /// </summary>
        /// <param name="eventCategory">Category for the event</param>
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

        /// <summary>
        /// Handler for setting a category.
        /// Method used for binding a command to the view.
        /// </summary>
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
