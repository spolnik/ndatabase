using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.Practices.Prism.Regions;
using Syncfusion.Windows.Tools.Controls;

namespace NDatabase.Studio.Adapters
{
    /// <summary>
    /// Ribbon Tab Region Adapter that helps to host Ribbon Bars.
    /// </summary>
    [Export]
    public class RibbonTabRegionAdapter : RegionAdapterBase<RibbonTab>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RibbonTabRegionAdapter"/> class.
        /// </summary>
        /// <param name="regionBehaviorFactory">The region behavior factory.</param>
        [ImportingConstructor]
        public RibbonTabRegionAdapter(IRegionBehaviorFactory regionBehaviorFactory)
            : base(regionBehaviorFactory)
        {

        }

        /// <summary>
        /// Adapts the specified region.
        /// </summary>
        /// <param name="region">The region.</param>
        /// <param name="regionTarget">The region target.</param>
        protected override void Adapt(IRegion region, RibbonTab regionTarget)
        {
            region.Views.CollectionChanged += (sender, args) =>
                                                  {
                                                      foreach (var bar in region.Views.Cast<RibbonBar>())
                                                      {
                                                          if (!regionTarget.Items.Contains(bar))
                                                              regionTarget.Items.Add(bar);
                                                      }
                                                  };
        }

        /// <summary>
        /// Creates the region.
        /// </summary>
        /// <returns></returns>
        protected override IRegion CreateRegion()
        {
            return new AllActiveRegion();
        }
    }
}