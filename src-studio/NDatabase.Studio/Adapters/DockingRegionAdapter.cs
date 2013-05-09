using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using Microsoft.Practices.Prism.Regions;
using Syncfusion.Windows.Tools.Controls;

namespace NDatabase.Studio.Adapters
{
    /// <summary>
    /// Docking Region Adapter that helps to inject Dock windows.
    /// </summary>
    [Export]
    public class DockingRegionAdapter : RegionAdapterBase<DockingManager>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DockingRegionAdapter"/> class.
        /// </summary>
        /// <param name="regionBehaviorFactory">The region behavior factory.</param>
        [ImportingConstructor]
        public DockingRegionAdapter(IRegionBehaviorFactory regionBehaviorFactory)
            : base(regionBehaviorFactory)
        {
        }

        /// <summary>
        /// Adapts the specified region.
        /// </summary>
        /// <param name="region">The region.</param>
        /// <param name="regionTarget">The region target.</param>
        protected override void Adapt(IRegion region, DockingManager regionTarget)
        {
            region.Views.CollectionChanged += (sender, args) =>
                                                  {
                                                      foreach (var child in region.Views.Cast<FrameworkElement>().Where(child => !regionTarget.Children.Contains(child)))
                                                      {
                                                          regionTarget.BeginInit();
                                                          regionTarget.Children.Add(child);
                                                          regionTarget.EndInit();
                                                      }
                                                  };
        }

        /// <summary>
        /// Attaches the behaviors.
        /// </summary>
        /// <param name="region">The region.</param>
        /// <param name="regionTarget">The region target.</param>
        protected override void AttachBehaviors(IRegion region, DockingManager regionTarget)
        {
            region.Behaviors.Add(DockingChildrenSourceSyncBehavior.BehaviorKey, new DockingChildrenSourceSyncBehavior()
                                                                                    {
                                                                                        HostControl = regionTarget
                                                                                    });

            base.AttachBehaviors(region, regionTarget);
        }

        /// <summary>
        /// Creates the region.
        /// </summary>
        /// <returns></returns>
        protected override IRegion CreateRegion()
        {
            return new Region();
        }
    }
}