using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Tool.Wrappers;
using NUnit.Framework;

namespace Test.Odb.Test.Performance
{
	[TestFixture]
    public class TestInstanceOf : ODBTest
	{
		private const int size = 100000000;

		[Test]
        public virtual void TestIfInstanceOf()
		{
			long start = OdbTime.GetCurrentTimeInTicks();
			NonNativeObjectInfo nnoi = new NonNativeObjectInfo
				(null);
			for (int i = 0; i < size; i++)
			{
				if (nnoi is NonNativeObjectInfo)
				{
				}
			}
			// println("time instance of=" + (OdbTime.getCurrentTimeInMs()-start));
			AssertTrue(true);
		}

		[Test]
        public virtual void TestIf()
		{
			long start = OdbTime.GetCurrentTimeInTicks();
			NonNativeObjectInfo nnoi = new NonNativeObjectInfo
				(null);
			for (int i = 0; i < size; i++)
			{
				if (nnoi.IsCollectionObject())
				{
				}
			}
			// println("time if=" + (OdbTime.getCurrentTimeInMs()-start));
			AssertTrue(true);
		}
	}
}
