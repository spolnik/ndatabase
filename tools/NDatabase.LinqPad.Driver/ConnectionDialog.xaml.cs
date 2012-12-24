using System.Windows;
using LINQPad.Extensibility.DataContext;
using Microsoft.Win32;

namespace NDatabase.LinqPad.Driver
{
    /// <summary>
    /// Interaction logic for ConnectionDialog.xaml
    /// </summary>
    public partial class ConnectionDialog : Window
    {
        private readonly NDatabaseDynamicDriverProperties _properties;

        public ConnectionDialog(IConnectionInfo cxInfo)
        {
            DataContext = _properties = new NDatabaseDynamicDriverProperties(cxInfo);
            Background = SystemColors.ControlBrush;
            InitializeComponent();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            var fileDialog = new OpenFileDialog {DefaultExt = ".ndb", Filter = "NDatabase db file (.ndb)|*.ndb"};

            var result = fileDialog.ShowDialog();
            if (result == true)
                _properties.DbFilePath = fileDialog.FileName;
        }
    }
}
