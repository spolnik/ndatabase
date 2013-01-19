using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using NDatabase.Odb;
using NDatabase.Odb.Core.Layers.Layer3.IO;

namespace NDatabase.WindowsPhone7.Sample
{
    public class Person
    {
        public string Name;
        public int Age;
    }

    public partial class MainPage
    {
        private const string DbName = "windowsphone7.sample.ndb";

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            Refresh();
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
            IList<string> source;
            using (var odb = OdbFactory.Open(DbName))
            {
                var people = odb.QueryAndExecute<Person>().ToList();
                source = people.Select(person => string.Format("[{0}]: {1}, {2}", odb.GetObjectId(person).ObjectId, person.Name, person.Age)).ToList();
            }

            PersonGrid.ItemsSource = source;
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

        private void IncreaseQuotaButton_Click(object sender, RoutedEventArgs e)
        {
            // Request 5MB more space in bytes.
            const long spaceToAdd = 5242880;
            var curAvail = OdbStore.Instance.AvailableFreeSpace;

            // If available space is less than
            // what is requested, try to increase.
            if (curAvail >= spaceToAdd) return;
            // Request more quota space.

            if (!OdbStore.Instance.IncreaseQuotaTo(OdbStore.Instance.Quota + spaceToAdd))
            {
                // The user clicked NO to the
                // host's prompt to approve the quota increase.
            }
            else
            {
                // The user clicked YES to the
                // host's prompt to approve the quota increase.
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            OdbFactory.Delete(DbName);
            Refresh();
        }

    }
}