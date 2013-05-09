using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;
using NDatabase.Studio.Infrastructure;

namespace NDatabase.Studio.Modules.Ribbon
{
    /// <summary>
    ///     Represents the DashBoardTabViewModel class which handles the interaction logic for DashBoardTabView.
    /// </summary>
    public class DashBoardTabViewModel : Control, INotifyPropertyChanged
    {
        #region Members

        private readonly IEventAggregator eventAggregator;
        private readonly IModuleManager moduleManager;
        private readonly IRegionManager regionManager;
        private readonly BackgroundWorker worker = new BackgroundWorker();
        private string animationlabel;
        private string liveButtonDetails;
        private Window win;

        #endregion

        /// <summary>
        ///     Initializes a new instance of the <see cref="DashBoardTabViewModel" /> class.
        /// </summary>
        /// <param name="eventAggregator">The event aggregator.</param>
        /// <param name="moduleManager">The module manager.</param>
        /// <param name="regionManager">The region manager.</param>
        /// <param name="unityContainer">The unity container.</param>
        public DashBoardTabViewModel(IEventAggregator eventAggregator, IModuleManager moduleManager,
                                     IRegionManager regionManager, IUnityContainer unityContainer)
        {
            this.eventAggregator = eventAggregator;
            this.moduleManager = moduleManager;
            this.regionManager = regionManager;
            LiveButtonDetails = "Live Updates";
            AnimationLabel = "Activate Animation";
            RegisterDelegates();
            InitializeWorker();
        }

        #region Implementation

        /// <summary>
        ///     Registers the delegates.
        /// </summary>
        private void RegisterDelegates()
        {
            RealTimeUpdates = new DelegateCommand<object>(StartStopRealTimeUpdates);

            //Animation Events
            CubicAnimationType =
                new DelegateCommand<object>(o => eventAggregator.GetEvent<AnimationTypesEvents>().Publish("Cubic"));
            ElasticAnimationType =
                new DelegateCommand<object>(o => eventAggregator.GetEvent<AnimationTypesEvents>().Publish("Elastic"));
            BounceAnimationType =
                new DelegateCommand<object>(o => eventAggregator.GetEvent<AnimationTypesEvents>().Publish("Bounce"));
            BackAnimationType =
                new DelegateCommand<object>(o => eventAggregator.GetEvent<AnimationTypesEvents>().Publish("Back"));
            QuinticAnimationType =
                new DelegateCommand<object>(o => eventAggregator.GetEvent<AnimationTypesEvents>().Publish("Quintic"));

            //Skin Events
            SelectBlueSkin =
                new DelegateCommand<object>(o => eventAggregator.GetEvent<SkinChangedEvent>().Publish("Blue"));
            SelectBlackSkin =
                new DelegateCommand<object>(o => eventAggregator.GetEvent<SkinChangedEvent>().Publish("Black"));
            SelectSilverSkin =
                new DelegateCommand<object>(o => eventAggregator.GetEvent<SkinChangedEvent>().Publish("Silver"));
            SelectBlendSkin =
                new DelegateCommand<object>(o => eventAggregator.GetEvent<SkinChangedEvent>().Publish("Blend"));

            SelectView =
                new DelegateCommand<object>(o => eventAggregator.GetEvent<SelectedViewEvents>().Publish("Dashboard"));
            ActivateAnimation = new DelegateCommand<object>(ActivateAnimationHandler);
            eventAggregator.GetEvent<SelectedViewEvents>().Subscribe(SelectedViewEventHandler);
        }

        /// <summary>
        ///     Initializes the background worker.
        /// </summary>
        private void InitializeWorker()
        {
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            worker.DoWork += worker_DoWork;
        }

        /// <summary>
        ///     Shows the window.
        /// </summary>
        private void ShowWindow()
        {
            win = new Window();
            var rs = new ResourceDictionary();
            rs.Source = new Uri(@"Infrastructure\Brushes.xaml", UriKind.RelativeOrAbsolute);
            var brush = rs["background"] as DrawingBrush;
            var border = new Border();
            //border = this.Resources["border"] as Border;
            var label = new Label();
            label.Content = "Please Wait...Loading Modules...";
            label.FontWeight = FontWeights.Bold;
            label.HorizontalAlignment = HorizontalAlignment.Center;
            label.VerticalAlignment = VerticalAlignment.Center;
            label.FontSize = 14;
            label.FontFamily = new FontFamily("Verdana");
            border.Child = label;
            win.Background = brush;
            win.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            win.WindowStyle = WindowStyle.None;
            win.Height = 75;
            win.Width = 309;
            win.ShowInTaskbar = false;
            win.Content = border;
            win.Show();
        }

        #endregion

        #region EventHandlers

        /// <summary>
        ///     Handles the DoWork event of the worker control. Does the long work.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">
        ///     The <see cref="System.ComponentModel.DoWorkEventArgs" /> instance containing the event data.
        /// </param>
        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
                                                                            {
                                                                                ShowWindow();
                                                                                moduleManager.LoadModule("ConnectionsModule");
                                                                                moduleManager.LoadModule("QueryModule");
                                                                                moduleManager.LoadModule("OutputsModule");
                                                                                moduleManager.LoadModule("ErrorsModule");

                                                                                ActivateView();
                                                                            }));
        }

        /// <summary>
        ///     Handles the RunWorkerCompleted event of the worker control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">
        ///     The <see cref="System.ComponentModel.RunWorkerCompletedEventArgs" /> instance containing the event data.
        /// </param>
        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            win.Close();
        }

        /// <summary>
        ///     Handles the view selected event
        /// </summary>
        /// <param name="ViewName">Name of the view.</param>
        private void SelectedViewEventHandler(string ViewName)
        {
            if (ViewName == "Dashboard")
            {
                if (!worker.IsBusy)
                    worker.RunWorkerAsync();
            }
        }

        /// <summary>
        ///     Activates the view.
        /// </summary>
        private void ActivateView()
        {
            var ribbonRegion = regionManager.Regions[Infrastructure.RegionNames.RibbonRegion];

            var cview = ribbonRegion.GetView("contributionAnalyzerView");
            ribbonRegion.Deactivate(cview);
            var dview = ribbonRegion.GetView("dashboardView");
            ribbonRegion.Activate(dview);

            var dockingRegion = regionManager.Regions[Infrastructure.RegionNames.DockingRegion];

            var connectionsView = dockingRegion.GetView("connectionsView");
            if (connectionsView != null)
                dockingRegion.Deactivate(connectionsView);

            var connectionsViewRefreshed = dockingRegion.GetView("connectionsView");
            dockingRegion.Activate(connectionsViewRefreshed);

            var queryView = dockingRegion.GetView("queryView");
            if (queryView != null)
                dockingRegion.Deactivate(queryView);

            var queryViewRefreshed = dockingRegion.GetView("queryView");
            dockingRegion.Activate(queryViewRefreshed);

            var outputsView = dockingRegion.GetView("outputsView");
            if (outputsView != null)
                dockingRegion.Deactivate(outputsView);

            var outputsViewRefreshed = dockingRegion.GetView("outputsView");
            dockingRegion.Activate(outputsViewRefreshed);

            var errorsView = dockingRegion.GetView("errorsView");
            if (errorsView != null)
                dockingRegion.Deactivate(errorsView);

            var errorsViewRefreshed = dockingRegion.GetView("errorsView");
            dockingRegion.Activate(errorsViewRefreshed);
        }

        /// <summary>
        ///     Publishes the animation activation/deactivation event
        /// </summary>
        /// <param name="o"></param>
        private void ActivateAnimationHandler(object o)
        {
            if (AnimationLabel.Equals("Activate Animation"))
            {
                AnimationLabel = "Deactivate Animation";
                eventAggregator.GetEvent<DashboardAnimationEvents>().Publish(true);
            }
            else
            {
                AnimationLabel = "Activate Animation";
                eventAggregator.GetEvent<DashboardAnimationEvents>().Publish(false);
            }
        }

        /// <summary>
        ///     Publishes real time updates.
        /// </summary>
        /// <param name="o">The o.</param>
        private void StartStopRealTimeUpdates(object o)
        {
            if (LiveButtonDetails.Equals("Live Updates"))
            {
                LiveButtonDetails = "Stop Updates";
                eventAggregator.GetEvent<StartStopUpdateEvent>().Publish(true);
            }
            else
            {
                LiveButtonDetails = "Live Updates";
                eventAggregator.GetEvent<StartStopUpdateEvent>().Publish(false);
            }
        }

        #endregion

        #region DelegateCommands and Properties

        /// <summary>
        ///     Gets or sets the selected view.
        /// </summary>
        /// <value>The select blue skin.</value>
        public DelegateCommand<object> SelectView { get; set; }

        /// <summary>
        ///     Gets or sets the real time updates.
        /// </summary>
        /// <value>The real time updates.</value>
        public DelegateCommand<object> RealTimeUpdates { get; set; }

        /// <summary>
        ///     Gets or sets the type of the cubic animation.
        /// </summary>
        /// <value>The type of the cubic animation.</value>
        public DelegateCommand<object> CubicAnimationType { get; set; }

        /// <summary>
        ///     Gets or sets the type of the bounce animation.
        /// </summary>
        /// <value>The type of the bounce animation.</value>
        public DelegateCommand<object> BounceAnimationType { get; set; }

        /// <summary>
        ///     Gets or sets the type of the elastic animation.
        /// </summary>
        /// <value>The type of the elastic animation.</value>
        public DelegateCommand<object> ElasticAnimationType { get; set; }

        /// <summary>
        ///     Gets or sets the type of the back animation.
        /// </summary>
        /// <value>The type of the back animation.</value>
        public DelegateCommand<object> BackAnimationType { get; set; }

        /// <summary>
        ///     Gets or sets the type of the quintic animation.
        /// </summary>
        /// <value>The type of the quintic animation.</value>
        public DelegateCommand<object> QuinticAnimationType { get; set; }

        /// <summary>
        ///     Gets or sets the activate animation.
        /// </summary>
        /// <value>The activate animation.</value>
        public DelegateCommand<object> ActivateAnimation { get; set; }

        /// <summary>
        ///     Gets or sets the select blue skin.
        /// </summary>
        /// <value>The select blue skin.</value>
        public DelegateCommand<object> SelectBlueSkin { get; set; }

        /// <summary>
        ///     Gets or sets the select black skin.
        /// </summary>
        /// <value>The select black skin.</value>
        public DelegateCommand<object> SelectBlackSkin { get; set; }

        /// <summary>
        ///     Gets or sets the select silver skin.
        /// </summary>
        /// <value>The select silver skin.</value>
        public DelegateCommand<object> SelectSilverSkin { get; set; }

        /// <summary>
        ///     Gets or sets the select blend skin.
        /// </summary>
        /// <value>The select blend skin.</value>
        public DelegateCommand<object> SelectBlendSkin { get; set; }

        /// <summary>
        ///     Gets or sets the live button details.
        /// </summary>
        /// <value>The live button details.</value>
        public string LiveButtonDetails
        {
            get { return liveButtonDetails; }
            set
            {
                liveButtonDetails = value;

                OnPropertyChanged("LiveButtonDetails");
            }
        }

        /// <summary>
        ///     Gets or sets the animation label.
        /// </summary>
        /// <value>The animation label.</value>
        public string AnimationLabel
        {
            get { return animationlabel; }
            set
            {
                animationlabel = value;

                OnPropertyChanged("AnimationLabel");
            }
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        #endregion
    }
}