using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.Regions.Behaviors;
using Syncfusion.Windows.Tools.Controls;

namespace NDatabase.Studio.Adapters
{
    /// <summary>
    /// Docking sync behavior that helps to activate the dock windows based on view state.
    /// </summary>
    public class DockingChildrenSourceSyncBehavior : RegionBehavior, IHostAwareRegionBehavior
    {
        /// <summary>
        /// Name that identifies the SelectorItemsSourceSyncBehavior behavior in a collection of RegionsBehaviors. 
        /// </summary>
        public static readonly string BehaviorKey = "DockingChildrenSourceSyncBehavior";
        private bool _updatingActiveViewsInHostControlSelectionChanged;
        private DockingManager _hostControl;
        ObservableCollection<object> _newViews;

        /// <summary>
        /// Gets or sets the <see cref="DependencyObject"/> that the <see cref="IRegion"/> is attached to.
        /// </summary>
        /// <value>
        /// A <see cref="DependencyObject"/> that the <see cref="IRegion"/> is attached to.
        /// </value>
        /// <remarks>For this behavior, the host control must always be a <see cref="Selector"/> or an inherited class.</remarks>
        public DependencyObject HostControl
        {
            get { return _hostControl; }
            set { _hostControl = value as DockingManager; }
        }

        /// <summary>
        /// Starts to monitor the <see cref="IRegion"/> to keep it in synch with the items of the <see cref="HostControl"/>.
        /// </summary>
        protected override void OnAttach()
        {

            SynchronizeItems();
            //this.hostControl.SelectionChanged += this.HostControlSelectionChanged;
            //  this.hostControl.ActiveWindowChanged += new PropertyChangedCallback(hostControl_ActiveWindowChanged);

            Region.ActiveViews.CollectionChanged += ActiveViews_CollectionChanged;
            Region.Views.CollectionChanged += Views_CollectionChanged;

            _newViews = new ObservableCollection<object>();
        }

        //public static DependencyObject GetAttachedControl(this IRegion region)
        //{
        //    RegionManagerRegistrationBehavior behavior = (RegionManagerRegistrationBehavior)region.Behaviors.First(b => b.Key == RegionManagerRegistrationBehavior.BehaviorKey).Value;
        //    return behavior.HostControl;
        //}

        /// <summary>
        /// Hosts the control_ active window changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        void hostControl_ActiveWindowChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                // Record the fact that we are now updating active views in the HostControlSelectionChanged method. 
                // This is needed to prevent the ActiveViews_CollectionChanged() method from firing. 
                this._updatingActiveViewsInHostControlSelectionChanged = true;

                //object source;
                //source = e;


                //if (source == sender)
                //{

                // check if the view is in both Views and ActiveViews collections (there may be out of sync)
                //if (this.Region.Views.Contains(e.OldValue) && this.Region.ActiveViews.Contains(e.OldValue))
                //{
                //    this.Region.Deactivate(e.OldValue);
                //}

                // Uncomment the following lines to activate a view when you show it again
                /*else if(selector != null)
                {
                    Activate the view
                    region.Activate(view);   // It is not always needed activate the view when unhiding
                }*/

                if (e.NewValue != null)
                    _newViews.Add(e.NewValue);


                //foreach (object view in this.Region.Views)
                //{
                //    if (!this.Region.ActiveViews.Contains(view) && e.OldValue != view && !oldviews.Contains(view))
                //    {
                //        (view as UIElement).Visibility = Visibility.Collapsed;
                //    }
                //}

                //foreach (object view in this.Region.Views)
                //{
                //    object b = (view as Control).Parent;

                //    if (!_newViews.Contains(view))
                //    {
                //        (view as Control).SetValue(DockingManager.StateProperty, DockState.Hidden);
                //    }
                //    else
                //    { 
                //    (view as Control).SetValue(DockingManager.StateProperty, DockState.Dock);
                //    }
                //}
                if (this.Region.Views.Contains(e.NewValue) && !this.Region.ActiveViews.Contains(e.NewValue))
                {
                    this.Region.Activate(e.NewValue);
                }
                //}
            }
            finally
            {
                this._updatingActiveViewsInHostControlSelectionChanged = false;
            }
        }

        /// <summary>
        /// Handles the CollectionChanged event of the Views control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        private void Views_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (object newItem in e.NewItems)
                    {
                        if (newItem is FrameworkElement && !_hostControl.Children.Contains(newItem as FrameworkElement))
                            _hostControl.Children.Add(newItem as FrameworkElement);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (object oldItem in e.OldItems)
                    {
                        if (oldItem is FrameworkElement && _hostControl.Children.Contains(oldItem as FrameworkElement))
                            _hostControl.Children.Remove(oldItem as FrameworkElement);
                    }
                    break;
            }
        }

        /// <summary>
        /// Synchronizes the items.
        /// </summary>
        private void SynchronizeItems()
        {
            var existingItems = _hostControl.Children.Cast<object>().ToList();

            foreach (var view in Region.Views)
            {
                if (!_hostControl.Children.Contains(view as FrameworkElement))
                    _hostControl.Children.Add(view as FrameworkElement);
            }

            foreach (var existingItem in existingItems)
                Region.Add(existingItem as FrameworkElement);
        }

        /// <summary>
        /// Handles the CollectionChanged event of the ActiveViews control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        private void ActiveViews_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            IViewsCollection newviews = Region.ActiveViews;
            foreach (var view in Region.Views)
                ((Control) view).SetValue(DockingManager.StateProperty, DockState.Hidden);

            foreach (var view in Region.ActiveViews)
            {
                //if (!this.Region.ActiveViews.Contains(view))
                //{
                //    (view as Control).SetValue(DockingManager.StateProperty, DockState.Hidden);
                //}
                //else
                {
                    if (((Control) view).Tag == null)
                        ((Control) view).Tag = DockState.Dock;

                    ((Control) view).SetValue(DockingManager.StateProperty, ((Control) view).Tag);
                }
            }
            //if (this.updatingActiveViewsInHostControlSelectionChanged)
            //{
            //    return;
            //}

            //if (e.Action == NotifyCollectionChangedAction.Add)
            //{
            //    if (this.hostControl.ActiveWindow != null
            //        && this.hostControl.ActiveWindow != e.NewItems[0]
            //        && this.Region.ActiveViews.Contains(this.hostControl.ActiveWindow))
            //    {
            //        this.Region.Deactivate(this.hostControl.ActiveWindow);
            //    }

            //    this.hostControl.ActiveWindow = e.NewItems[0] as FrameworkElement;
            //}
            //else if (e.Action == NotifyCollectionChangedAction.Remove &&
            //         e.OldItems.Contains(this.hostControl.ActiveWindow))
            //{
            //    this.hostControl.ActiveWindow = null;
            //}
        }
    }
}