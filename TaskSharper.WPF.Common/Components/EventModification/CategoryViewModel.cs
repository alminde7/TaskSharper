using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using TaskSharper.Domain.Calendar;
using TaskSharper.WPF.Common.Media;

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

        public CategoryViewModel(IRegionManager regionManager, IEventAggregator eventAggregator, IEventRestClient dataService)
        {
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;
            _dataService = dataService;
        }
    }
}
