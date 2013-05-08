using System.ComponentModel.Composition.Hosting;
using System.Windows;
using Microsoft.Practices.Prism.MefExtensions;
using Microsoft.Practices.Prism.Regions;
using NDatabase.Studio.Infrastructure;
using Syncfusion.Windows.Tools.Controls;

namespace NDatabase.Studio
{
    public class Bootstrapper : MefBootstrapper
    {
        protected override void ConfigureAggregateCatalog()
        {
            AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(Bootstrapper).Assembly));
            
            //Modules not refered in the Shell assmebly. Just importing all modules through this directory catalog. Ensure the build o/p path set properly for module projects.
            AggregateCatalog.Catalogs.Add(new DirectoryCatalog("../../Modules")); // add all assemblies in the modules
        }

        protected override RegionAdapterMappings ConfigureRegionAdapterMappings()
        {
            var mappings = base.ConfigureRegionAdapterMappings();

            if (mappings != null)
                mappings.RegisterMapping(typeof (DockingManager), Container.GetExportedValue<DockingAdapter>());

            return mappings;
        }

        protected override void InitializeShell()
        {
            base.InitializeShell();

            Application.Current.MainWindow = (Shell)Shell;
            Application.Current.MainWindow.Show();
        }

        protected override DependencyObject CreateShell()
        {
            return Container.GetExportedValue<Shell>();
        }
    }
}