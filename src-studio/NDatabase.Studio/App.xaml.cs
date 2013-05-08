using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace NDatabase.Studio
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private Window _window;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            ShowWindow();
            var bootstrapper = new Bootstrapper();
            bootstrapper.Run();
            _window.Close();
        }

        private void ShowWindow()
        {
            var rs = new ResourceDictionary
                         {
                             Source =
                                 new Uri(@"Infrastructure\Brushes.xaml", UriKind.RelativeOrAbsolute)
                         };

            var brush = rs["blackbackground"] as DrawingBrush;

            _window = new Window
                          {
                              WindowStartupLocation = WindowStartupLocation.CenterScreen,
                              WindowStyle = WindowStyle.None,
                              Background = brush,
                              Height = 75,
                              Width = 309,
                              ShowInTaskbar = false,
                              Topmost = true,
                              Content = GetLabel()
                          };

            _window.Show();
        }

        private static Label GetLabel()
        {
            return new Label
                       {
                           Content = "Please Wait...Loading Modules...",
                           FontWeight = FontWeights.Bold,
                           HorizontalAlignment = HorizontalAlignment.Center,
                           VerticalAlignment = VerticalAlignment.Center,
                           FontSize = 14,
                           Foreground = Brushes.White,
                           FontFamily = new FontFamily("Verdana")
                       };
        }
    }
}
