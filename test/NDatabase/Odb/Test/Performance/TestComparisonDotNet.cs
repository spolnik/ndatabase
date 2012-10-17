using NDatabase2.Odb;
using NDatabase2.Tool.Wrappers;
using NUnit.Framework;

namespace Test.NDatabase.Odb.Test.Performance
{
    [TestFixture]
    public class TestComparisonDotNet : ODBTest
    {
        [Test]
        public virtual void Test1()
        {
            IOdb odb = null;
            try
            {
                DeleteBase("mydb.neodatis");
                // Open the database
                odb = Open("mydb.neodatis");
                var t0 = OdbTime.GetCurrentTimeInTicks();
                var nRecords = 10000;
                for (var i = 0; i < nRecords; i++)
                {
                    var ao = new Class1(189, "csdcsdc");
                    odb.Store(ao);
                }
                odb.Close();
                var t1 = OdbTime.GetCurrentTimeInTicks();
                odb = Open("mydb.neodatis");
                var ssss = odb.GetObjects<Class1>();
                var t2 = OdbTime.GetCurrentTimeInTicks();
                Println("Elapsed time for inserting " + nRecords + " records: " + (t1 - t0) + " / select = " + (t2 - t1));
            }
            finally
            {
                if (odb != null)
                {
                    // Close the database
                    odb.Close();
                }
            }
        }
    }
}
