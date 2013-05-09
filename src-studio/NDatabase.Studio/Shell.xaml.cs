using System.Windows;
using System.Windows.Controls;
using Microsoft.Practices.Prism.Events;
using NDatabase.Studio.Infrastructure;
using NDatabase.Studio.Modules.Ribbon;
using Syncfusion.Windows.Shared;
using Syncfusion.Windows.Tools.Controls;

namespace NDatabase.Studio
{
    /// <summary>
    /// Interaction logic for Shell.xaml
    /// </summary>
    public partial class Shell
    {
        /// <summary>
        /// The IEventAggregator object. 
        /// </summary>
        private readonly IEventAggregator _eventAggregator;

        public Shell(IEventAggregator eventAggregatorr, AppMenuViewModel viewModel)
        {
            InitializeComponent();
            SkinStorage.SetVisualStyle(this, "Blend");
            _eventAggregator = eventAggregatorr;
            
            //Subscribe to the skin changed event.
            DataContext = viewModel;
            _eventAggregator.GetEvent<SkinChangedEvent>().Subscribe(SkinChangedHandler);
        }

        /// <summary>
        /// Represents the Skin changed event handler. Selects the appropriate skin based on the parameter passed while publishing the event.
        /// </summary>
        /// <param name="skinName">The Skin Name</param>
        void SkinChangedHandler(string skinName)
        {
            switch (skinName)
            {
                case "Blue":
                    SkinStorage.SetVisualStyle(this, "Office2007Blue");
                    break;
                case "Black":
                    SkinStorage.SetVisualStyle(this, "Office2007Black");
                    break;
                case "Silver":
                    SkinStorage.SetVisualStyle(this, "Office2007Silver");
                    break;
                case "Blend":
                    SkinStorage.SetVisualStyle(this, "Blend");
                    break;
            }
        }

        private void DockingRegion_OnDockStateChanged(FrameworkElement sender, DockStateEventArgs e)
        {
            if (e.NewState != DockState.Hidden)
                sender.Tag = e.NewState;
        }
    }
}
