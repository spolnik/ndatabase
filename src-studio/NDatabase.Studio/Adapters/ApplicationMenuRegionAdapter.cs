using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using Microsoft.Practices.Prism.Regions;
using Syncfusion.Windows.Tools.Controls;

namespace NDatabase.Studio.Adapters
{
    /// <summary>
    /// Application Menu Region Adapter that helps to inject Ribbon MenuItems.
    /// </summary>
    [Export]
    public class ApplicationMenuRegionAdapter : RegionAdapterBase<ApplicationMenu>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationMenuRegionAdapter"/> class.
        /// </summary>
        /// <param name="regionBehaviorFactory">The region behavior factory.</param>
        [ImportingConstructor]
        public ApplicationMenuRegionAdapter(IRegionBehaviorFactory regionBehaviorFactory)
            : base(regionBehaviorFactory)
        {

        }

        /// <summary>
        /// Adapts the specified region.
        /// </summary>
        /// <param name="region">The region.</param>
        /// <param name="regionTarget">The region target.</param>
        protected override void Adapt(IRegion region, ApplicationMenu regionTarget)
        {
            region.Views.CollectionChanged += (sender, args) =>
                                                  {
                                                      foreach (var element in region.Views.Cast<FrameworkElement>())
                                                      {
                                                          if (regionTarget.Items.Contains(element))
                                                              continue;

                                                          var i = 0;
                                                          if (element.Tag != null)
                                                              i = Convert.ToInt32(element.Tag);

                                                          regionTarget.Items.Insert(i, element);
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
