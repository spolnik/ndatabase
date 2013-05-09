using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using NDatabase.Studio.Infrastructure;

namespace NDatabase.Studio.Modules.Connections
{
    public class ConnectionsModule : IModule
    {
        private readonly IRegionManager _regionManager;
        private readonly IEventAggregator _eventAggregator;
        
        public ConnectionsModule(IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;
        }

        public void Initialize()
        {
            var viewModel = new ConnectionsViewModel(_eventAggregator);
            _regionManager.Regions[RegionNames.DockingRegion].Add(new ConnectionsView(viewModel), "connectionsView");
        }
    }
}