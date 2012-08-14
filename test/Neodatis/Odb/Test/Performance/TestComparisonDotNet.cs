using NDatabase.Odb;
using NDatabase.Tool.Wrappers;
using NUnit.Framework;

namespace Test.Odb.Test.Performance
{
	[TestFixture]
    public class TestComparisonDotNet : ODBTest
	{
		/// <exception cref="System.Exception"></exception>
		[Test]
        public virtual void Test1()
		{
			IOdb odb = null;
			try
			{
				DeleteBase("mydb.neodatis");
				// Open the database
				odb = Open("mydb.neodatis");
				long t0 = OdbTime.GetCurrentTimeInTicks();
				int nRecords = 10000;
				for (int i = 0; i < nRecords; i++)
				{
					Class1 ao = new Class1
						(189, "csdcsdc");
					odb.Store(ao);
				}
				odb.Close();
				long t1 = OdbTime.GetCurrentTimeInTicks();
				odb = Open("mydb.neodatis");
				IObjects<Class1> ssss = odb.GetObjects<Class1>();
				long t2 = OdbTime.GetCurrentTimeInTicks();
				Println("Elapsed time for inserting " + nRecords + " records: " + (t1 - t0) + " / select = "
					 + (t2 - t1));
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
