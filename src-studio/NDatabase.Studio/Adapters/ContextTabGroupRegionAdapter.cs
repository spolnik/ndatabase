using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.Practices.Prism.Regions;
using Syncfusion.Windows.Tools.Controls;

namespace NDatabase.Studio.Adapters
{
    /// <summary>
    /// ContextTabGroup Region Adapter that helps to inject Ribbon Tab.
    /// </summary>
    [Export]
    public class ContextTabGroupRegionAdapter : RegionAdapterBase<ContextTabGroup>
    {
        [ImportingConstructor]
        public ContextTabGroupRegionAdapter(IRegionBehaviorFactory regionBehaviorFactory)
            : base(regionBehaviorFactory)
        {
        }

        /// <summary>
        /// Adapts the specified region.
        /// </summary>
        /// <param name="region">The region.</param>
        /// <param name="regionTarget">The region target.</param>
        protected override void Adapt(IRegion region, ContextTabGroup regionTarget)
        {
            region.Views.CollectionChanged += (sender, args) =>
                                                  {
                                                      foreach (var tab in region.Views.Cast<RibbonTab>())
                                                      {
                                                          if (!regionTarget.RibbonTabs.Contains(tab))
                                                              regionTarget.RibbonTabs.Add(tab);
                                                      }
                                                  };
        }

        /// <summary>
        /// Creates the region.
        /// </summary>
        /// <returns></returns>
        protected override IRegion CreateRegion()
        {
            return new SingleActiveRegion();
        }
    }
}