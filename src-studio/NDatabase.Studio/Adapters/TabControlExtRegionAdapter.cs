using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using Microsoft.Practices.Prism.Regions;
using Syncfusion.Windows.Tools.Controls;

namespace NDatabase.Studio.Adapters
{
    [Export]
    public class TabControlExtRegionAdapter : RegionAdapterBase<TabControlExt>
    {
        [ImportingConstructor]
        public TabControlExtRegionAdapter(IRegionBehaviorFactory regionBehaviorFactory)
            : base(regionBehaviorFactory)
        {
        }

        protected override void Adapt(IRegion region, TabControlExt regionTarget)
        {
            region.Views.CollectionChanged += (sender, args) =>
                                                  {
                                                      foreach (var tab in region.Views.Cast<FrameworkElement>())
                                                      {
                                                          if (!regionTarget.Items.Contains(tab))
                                                              regionTarget.Items.Add(tab);
                                                      }
                                                  };
        }

        protected override IRegion CreateRegion()
        {
            return new Region();
        }
    }
}