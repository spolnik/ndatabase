using System.ComponentModel;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using NDatabase.Studio.Infrastructure;

namespace NDatabase.Studio.Modules.Ribbon
{
    public class AppMenuViewModel : INotifyPropertyChanged
    {
        IEventAggregator eventAggregator;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppMenuViewModel"/> class.
        /// </summary>
        /// <param name="eventAggregator">The event aggregator.</param>
        public AppMenuViewModel(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
            this.SelectDashboardView = new DelegateCommand<object>(SelectDashboardViewHandler);
            this.SelectContributionAnalyzerView = new DelegateCommand<object>(SelectContributionAnalyzerViewHandler);
            this.SelectBlueSkin = new DelegateCommand<object>(SelectBlueSkinHandler);
            this.SelectBlackSkin = new DelegateCommand<object>(SelectBlackSkinHandler);
            this.SelectSilverSkin = new DelegateCommand<object>(SelectSilverSkinHandler);
            this.SelectBlendSkin = new DelegateCommand<object>(SelectBlendSkinHandler);
        }

        #region EventHandlers
        /// <summary>
        /// Publishes the Dashboard View selected event.
        /// </summary>
        /// <param name="o">The o.</param>
        void SelectDashboardViewHandler(object o)
        {
            this.eventAggregator.GetEvent<SelectedViewEvents>().Publish("Dashb oard");            
        }

        /// <summary>
        /// Publishes the Dashboard View selected event.
        /// </summary>
        /// <param name="o"></param>
        void SelectContributionAnalyzerViewHandler(object o)
        {
            this.eventAggregator.GetEvent<SelectedViewEvents>().Publish("ContributionAnalyzer");
        }

        /// <summary>
        /// Publishes the Blue Skin selected event.
        /// </summary>
        /// <param name="o"></param>
        void SelectBlueSkinHandler(object o)
        {
            this.eventAggregator.GetEvent<SkinChangedEvent>().Publish("Blue");
        }

        /// <summary>
        /// Publishes the Black Skin selected event.
        /// </summary>
        /// <param name="o"></param>
        void SelectBlackSkinHandler(object o)
        {
            this.eventAggregator.GetEvent<SkinChangedEvent>().Publish("Black");
        }

        /// <summary>
        /// Publishes the Silver Skin selected event.
        /// </summary>
        /// <param name="o"></param>
        void SelectSilverSkinHandler(object o)
        {
            this.eventAggregator.GetEvent<SkinChangedEvent>().Publish("Silver");
        }

        /// <summary>
        /// Publishes the Blend Skin selected event.
        /// </summary>
        /// <param name="o"></param>
        void SelectBlendSkinHandler(object o)
        {
            this.eventAggregator.GetEvent<SkinChangedEvent>().Publish("Blend");
        }

        #endregion

        #region Delegate Commands
        /// <summary>
        /// Gets or sets the select dashboard view .
        /// </summary>
        /// <value>The select dashboard view.</value>
        public DelegateCommand<object> SelectDashboardView { get; set; }

        /// <summary>
        /// Gets or sets the select contribution analyzer view.
        /// </summary>
        /// <value>The select contribution analyzer view.</value>
        public DelegateCommand<object> SelectContributionAnalyzerView { get; set; }

        /// <summary>
        /// Gets or sets the select blue skin.
        /// </summary>
        /// <value>The select blue skin.</value>
        public DelegateCommand<object> SelectBlueSkin { get; set; }

        /// <summary>
        /// Gets or sets the select black skin.
        /// </summary>
        /// <value>The select black skin.</value>
        public DelegateCommand<object> SelectBlackSkin { get; set; }

        /// <summary>
        /// Gets or sets the select silver skin.
        /// </summary>
        /// <value>The select silver skin.</value>
        public DelegateCommand<object> SelectSilverSkin { get; set; }

        /// <summary>
        /// Gets or sets the select blend skin.
        /// </summary>
        /// <value>The select blend skin.</value>
        public DelegateCommand<object> SelectBlendSkin { get; set; }
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
