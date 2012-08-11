using NDatabase.Odb;
using NUnit.Framework;

namespace Test.Odb.Test.Buffer
{
	[TestFixture]
    public class TestBigData : ODBTest
	{
		[Test]
        public void Test1()
		{
            
			IOdb odb = Open("big-data.neodatis");
			System.Text.StringBuilder buffer = new System.Text.StringBuilder();
			for (int i = 0; i < 30000; i++)
			{
				buffer.Append('a');
			}
			VO.Login.Function function = new VO.Login.Function
				(buffer.ToString());
			odb.Store(function);
			odb.Close();
			odb = Open("big-data.neodatis");
			VO.Login.Function f2 = (VO.Login.Function)odb
				.GetObjects<VO.Login.Function>().GetFirst();
			AssertEquals(30000, f2.GetName().Length);
			odb.Close();
			odb = Open("big-data.neodatis");
			f2 = (VO.Login.Function)odb.GetObjects<VO.Login.Function>().GetFirst();
			f2.SetName(f2.GetName() + "ola chico");
			int newSize = f2.GetName().Length;
			odb.Store(f2);
			odb.Close();
			odb = Open("big-data.neodatis");
			f2 = (VO.Login.Function)odb.GetObjects<VO.Login.Function>().GetFirst();
			AssertEquals(newSize, f2.GetName().Length);
			AssertEquals(buffer.ToString() + "ola chico", f2.GetName());
			odb.Close();
		}

		[SetUp]
		public override void SetUp()
		{
			DeleteBase("big-data.neodatis");
		}

		[TearDown]
		public override void TearDown()
		{
			DeleteBase("big-data.neodatis");
		}
	}
}
