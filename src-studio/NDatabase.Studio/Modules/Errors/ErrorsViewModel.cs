using System.Collections.ObjectModel;

namespace NDatabase.Studio.Modules.Errors
{
    internal class ErrorsViewModel
    {
        public ErrorsViewModel()
        {
            Items = new ObservableCollection<Error>
                        {
                            new Error
                                {
                                    Number = "1",
                                    Description = "Method must have a return type",
                                    File = "MainWindow.xaml.cs",
                                    Column = "66",
                                    Line = "16",
                                    Project = "VS2010DockingManagerDemo_2010"
                                },
                            new Error
                                {
                                    Number = "2",
                                    Description = "Key Value not found",
                                    File = "MainWindow.xaml",
                                    Column = "26",
                                    Line = "23",
                                    Project = "VS2010DockingManagerDemo_2010"
                                },
                        };
        }

        public ObservableCollection<Error> Items { get; set; }
    }
}