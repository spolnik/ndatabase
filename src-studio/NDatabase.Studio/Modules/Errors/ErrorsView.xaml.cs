namespace NDatabase.Studio.Modules.Errors
{
    /// <summary>
    /// Interaction logic for ErrorsView.xaml
    /// </summary>
    public partial class ErrorsView
    {
        public ErrorsView()
        {
            InitializeComponent();
            Grid.DataContext = new ErrorsViewModel();
        }
    }
}
