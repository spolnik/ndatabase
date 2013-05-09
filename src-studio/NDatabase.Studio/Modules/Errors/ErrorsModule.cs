using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using NDatabase.Studio.Infrastructure;

namespace NDatabase.Studio.Modules.Errors
{
    public class ErrorsModule : IModule
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IRegionManager _regionManager;

        public ErrorsModule(IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;
        }

        public void Initialize()
        {
            _regionManager.Regions[RegionNames.DockingRegion].Add(new ErrorsView(), "errorsView");
        } 
    }
}