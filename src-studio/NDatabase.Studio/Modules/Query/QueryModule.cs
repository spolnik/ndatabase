using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using NDatabase.Studio.Infrastructure;

namespace NDatabase.Studio.Modules.Query
{
    public class QueryModule : IModule
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IRegionManager _regionManager;

        public QueryModule(IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;
        }

        public void Initialize()
        {
            var viewModel = new QueryViewModel(_eventAggregator);
            _regionManager.Regions[RegionNames.DockingRegion].Add(new QueryView(viewModel), "queryView");
        }
    }
}