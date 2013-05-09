using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using NDatabase.Studio.Infrastructure;

namespace NDatabase.Studio.Modules.Outputs
{
    public class OutputsModule : IModule
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IRegionManager _regionManager;

        public OutputsModule(IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;
        }

        public void Initialize()
        {
            _regionManager.Regions[RegionNames.DockingRegion].Add(new OutputsView(), "outputsView");
        }
    }
}