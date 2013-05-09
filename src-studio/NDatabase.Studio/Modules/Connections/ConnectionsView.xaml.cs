namespace NDatabase.Studio.Modules.Connections
{
    /// <summary>
    /// Interaction logic for ConnectionsView.xaml
    /// </summary>
    public partial class ConnectionsView
    {
        public ConnectionsView(ConnectionsViewModel viewModel)
        {
            InitializeComponent();
            List.DataContext = viewModel;
        }
    }
}
