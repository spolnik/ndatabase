using System.Windows;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.UnityExtensions;
using Microsoft.Practices.Unity;
using NDatabase.Studio.Adapters;
using NDatabase.Studio.Modules.Connections;
using NDatabase.Studio.Modules.Errors;
using NDatabase.Studio.Modules.Outputs;
using NDatabase.Studio.Modules.Query;
using NDatabase.Studio.Modules.Ribbon;
using Syncfusion.Windows.Tools.Controls;

namespace NDatabase.Studio
{
    public class Bootstrapper : UnityBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            var shell = Container.Resolve<Shell>();
            shell.Show();

            return shell;
        }

        protected override RegionAdapterMappings ConfigureRegionAdapterMappings()
        {
            var mappings = base.ConfigureRegionAdapterMappings();

            if (mappings != null)
            {
                mappings.RegisterMapping(typeof (Ribbon), Container.TryResolve<RibbonControlRegionAdapter>());
                mappings.RegisterMapping(typeof (DockingManager), Container.TryResolve<DockingRegionAdapter>());
                mappings.RegisterMapping(typeof (RibbonTab), Container.TryResolve<RibbonTabRegionAdapter>());
                mappings.RegisterMapping(typeof (ApplicationMenu), Container.TryResolve<ApplicationMenuRegionAdapter>());
            }

            return mappings;
        }

        protected override IModuleCatalog CreateModuleCatalog()
        {
            var catalog = new ModuleCatalog();
            catalog.AddModule(typeof(ConnectionsModule))
                   .AddModule(typeof(RibbonModule))
                   .AddModule(typeof(QueryModule))
                   .AddModule(typeof(OutputsModule))
                   .AddModule(typeof(ErrorsModule));
            return catalog;
        }
    }
}