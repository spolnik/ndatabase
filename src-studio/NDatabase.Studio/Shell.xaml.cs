using System.ComponentModel.Composition;
using System.Windows;
using Syncfusion.Windows.Tools.Controls;

namespace NDatabase.Studio
{
    /// <summary>
    /// Interaction logic for Shell.xaml
    /// </summary>
    [Export(typeof(Shell))]
    public partial class Shell
    {
        public Shell()
        {
            InitializeComponent();
        }

        private void DockingRegion_OnDockStateChanged(FrameworkElement sender, DockStateEventArgs e)
        {
            throw new System.NotImplementedException();
        }
    }
}
