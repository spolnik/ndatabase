using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using NDatabase.Odb;
using NUnit.Framework;

namespace NDatabase.Silverlight.UnitTests
{
    public class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }

    public class When_we_use_ndatabase_on_silverlight
    {
        public void It_should_pass_basic_sample()
        {
            var person = new Person {Name = "Julia", Age = 3};

            const string dbName = "silverlight.ndb";

            using (var odb = OdbFactory.Open(dbName))
            {
                odb.Store(person);
            }

            Person storedPerson;
            using (var odb = OdbFactory.Open(dbName))
            {
                storedPerson = odb.QueryAndExecute<Person>().GetFirst();
            }

            Assert.That(storedPerson, Is.Not.Null);
            Assert.That(storedPerson.Name, Is.EqualTo(person.Name));
            Assert.That(storedPerson.Age, Is.EqualTo(person.Age));
        }
    }
}
