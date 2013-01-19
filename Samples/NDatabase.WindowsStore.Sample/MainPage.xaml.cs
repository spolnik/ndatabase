using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NDatabase.Odb;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace NDatabase.WindowsStore.Sample
{
    public class Person
    {
        public string Name;
        public int Age;
    }

    public class PersonView
    {
        public long Oid { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
    }

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private const string DbName = "windowsstore.sample.ndb";

        public MainPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            using (var odb = OdbFactory.Open(DbName))
            {
                var person = new Person { Name = NameTextBox.Text, Age = Convert.ToInt32(AgeTextBox.Text) };
                odb.Store(person);
            }

            Refresh();
        }

        private void Refresh()
        {
            IEnumerable<PersonView> source;
            using (var odb = OdbFactory.Open(DbName))
            {
                var people = odb.QueryAndExecute<Person>().ToList();
                source = people.Select(x => new PersonView { Oid = odb.GetObjectId(x).ObjectId, Name = x.Name, Age = x.Age }).ToList();
            }

            PersonGrid.ItemsSource = source;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            OdbFactory.Delete(DbName);
            Refresh();
        }

        private void AddCountButton_Click(object sender, RoutedEventArgs e)
        {
            using (var odb = OdbFactory.Open(DbName))
            {
                var count = Convert.ToInt32(CountTextBox.Text);
                for (var i = 0; i < count; i++)
                {
                    var person = new Person { Name = NameTextBox.Text, Age = Convert.ToInt32(AgeTextBox.Text) };
                    odb.Store(person);
                }
            }

            Refresh();
        }
    }
}
