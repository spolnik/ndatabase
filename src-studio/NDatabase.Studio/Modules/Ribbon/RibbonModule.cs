using System;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;

namespace NDatabase.Studio.Modules.Ribbon
{
    public class RibbonModule : IModule
    {
        private readonly IUnityContainer _container;
        private readonly IEventAggregator _eventAggregator;
        private readonly IRegionManager _regionManager;
        private IModuleManager _moduleManager;

        /// <summary>
        ///     Initializes a new instance of the <see cref="RibbonModule" /> class.
        /// </summary>
        /// <param name="eventAgg">The event agg.</param>
        /// <param name="container">The container.</param>
        /// <param name="regionManager">The region manager.</param>
        public RibbonModule(IEventAggregator eventAgg, IUnityContainer container, IRegionManager regionManager)
        {
            _eventAggregator = eventAgg;
            _container = container;
            _regionManager = regionManager;
        }

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        public void Initialize()
        {
            try
            {
                _moduleManager = _container.Resolve<IModuleManager>();
            }
            catch (ResolutionFailedException ex)
            {
                if (ex.Message.Contains("IModuleCatalog"))
                    throw new InvalidOperationException("NullModuleCatalogExceptio");

                throw;
            }
            RegisterViewsWithRegions();
        }

        /// <summary>
        ///     Registers the views with regions.
        /// </summary>
        private void RegisterViewsWithRegions()
        {
            var dmodel = new DashBoardTabViewModel(_eventAggregator, _moduleManager, _regionManager, _container);
            _regionManager.Regions[Infrastructure.RegionNames.RibbonRegion].Add(
                new DashboardTabView(_eventAggregator, _moduleManager, _regionManager, _container, dmodel), "dashboardView");
            var cmodel = new ContributionAnalyzerTabViewModel(_eventAggregator, _moduleManager, _regionManager, _container);
            _regionManager.Regions[Infrastructure.RegionNames.RibbonRegion].Add(
                new ContributionAnalyzerTabView(_eventAggregator, _moduleManager, _regionManager, _container, cmodel),
                "contributionAnalyzerView");
//            _regionManager.RegisterViewWithRegion(RegionNames.RibbonRegion, typeof(DashboardTabView));
//            _regionManager.RegisterViewWithRegion(RegionNames.RibbonRegion, typeof(ContributionAnalyzerTabView));
            _regionManager.RegisterViewWithRegion(Infrastructure.RegionNames.AppMenuRegion, typeof (SelectView));
            _regionManager.RegisterViewWithRegion(Infrastructure.RegionNames.AppMenuRegion, typeof (SelectSkin));
        }
    }
}