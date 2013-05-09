using Microsoft.Practices.Prism.Events;

namespace NDatabase.Studio.Modules.Ribbon
{
    /// <summary>
    /// Interaction logic for EmployeesView.xaml
    /// </summary>
    public partial class SelectView
    {
        IEventAggregator eventAggregator;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectView"/> class.
        /// </summary>
        /// <param name="eventAgg">The event agg.</param>
        /// <param name="viewModel">The view model.</param>
        public SelectView(IEventAggregator eventAgg, AppMenuViewModel viewModel)
        {
            this.eventAggregator = eventAgg;
            InitializeComponent();
            this.DataContext = viewModel;
        }       
    
    }
}
