using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace NDatabase.CF.Sample
{
    public class SampleClass
    {
        public string Name;
        public int Age;
    }
    
    class Program
    {
        static void Main(string[] args)
        {
            NDatabase.Odb.OdbFactory.Delete("cf.ndb");

            using (var odb = NDatabase.Odb.OdbFactory.Open("cf.ndb"))
            {
                var item = new SampleClass();
                item.Age = 3;
                item.Name = "Julia";

                odb.Store<SampleClass>(item);
            }

            using (var odb = NDatabase.Odb.OdbFactory.Open("cf.ndb"))
            {
                var storedItem = odb.QueryAndExecute<SampleClass>().GetFirst();
                Console.Write("Name: " + storedItem.Name);
                Console.Write("Age: " + storedItem.Age);
            }

            Console.WriteLine("Press key...");
            Console.ReadLine();
        }
    }
}
