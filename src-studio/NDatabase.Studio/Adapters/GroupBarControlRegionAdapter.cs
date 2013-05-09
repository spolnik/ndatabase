using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.Practices.Prism.Regions;
using Syncfusion.Windows.Tools.Controls;

namespace NDatabase.Studio.Adapters
{
    [Export]
    public class GroupBarControlRegionAdapter : RegionAdapterBase<GroupBar>
    {
        [ImportingConstructor]
        public GroupBarControlRegionAdapter(IRegionBehaviorFactory regionBehaviorFactory)
            : base(regionBehaviorFactory)
        {

        }

        protected override void Adapt(IRegion region, GroupBar regionTarget)
        {
            region.Views.CollectionChanged += (sender, args) =>
                                                  {
                                                      foreach (var item in region.Views.Cast<GroupBarItem>())
                                                      {
                                                          if (!regionTarget.Items.Contains(item))
                                                              regionTarget.Items.Add(item);
                                                      }
                                                  };
        }

        protected override IRegion CreateRegion()
        {
            return new Region();
        }
    }
}