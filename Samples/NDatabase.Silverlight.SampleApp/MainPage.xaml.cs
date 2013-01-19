using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using NDatabase.Odb;
using NDatabase.Odb.Core.Layers.Layer3.IO;

namespace NDatabase.Silverlight.SampleApp
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

    public partial class MainPage : UserControl
    {
        private const string DbName = "silverlight.sample.ndb";

        public MainPage()
        {
            InitializeComponent();

            PersonGrid.Columns.Add(
                        new DataGridTextColumn
                        {
                            Header = "ID",
                            Binding = new Binding("Oid")
                        });
            PersonGrid.Columns.Add(
                        new DataGridTextColumn
                        {
                            Header = "Name",
                            Binding = new Binding("Name")
                        });
            PersonGrid.Columns.Add(
                        new DataGridTextColumn
                        {
                            Header = "Age",
                            Binding = new Binding("Age")
                        });
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

        private void AddCountButtonClick(object sender, RoutedEventArgs e)
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

        private void IncreaseQuotaButtonClick(object sender, RoutedEventArgs e)
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
    }
}
