using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using Microsoft.Practices.Prism.Regions;
using Syncfusion.Windows.Tools.Controls;

namespace NDatabase.Studio.Infrastructure
{
    [Export]
    public class DockingAdapter : RegionAdapterBase<DockingManager>
    {
        [ImportingConstructor]
        public DockingAdapter(IRegionBehaviorFactory regionBehaviorFactory)
            : base(regionBehaviorFactory)
        {

        }

        protected override void Adapt(IRegion region, DockingManager regionTarget)
        {
            region.Views.CollectionChanged += (sender, args) =>
                                                  {
                                                      foreach (var child in region.Views.Cast<FrameworkElement>())
                                                      {
                                                          if (regionTarget.Children.Contains(child))
                                                              continue;

                                                          regionTarget.BeginInit();
                                                          regionTarget.Children.Add(child);
                                                          regionTarget.EndInit();
                                                      }
                                                  };
        }

        protected override IRegion CreateRegion()
        {
            return new AllActiveRegion();
        }
    }
}