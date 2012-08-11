using NDatabase.Tool.Wrappers;
using NUnit.Framework;

namespace Test.Odb.Test.Performance
{
	[TestFixture]
    public class TestArrayCopy : ODBTest
	{
		[Test]
        public virtual void Test1()
		{
			int size = 1000;
			int arraySize = 100000;
			byte[] bs1 = new byte[arraySize];
			byte[] bs2 = new byte[arraySize];
			long start = OdbTime.GetCurrentTimeInTicks();
			for (int i = 0; i < size; i++)
			{
				System.Array.Copy(bs1, 0, bs2, 0, arraySize);
			}
			long step1 = OdbTime.GetCurrentTimeInTicks();
			long time1 = step1 - start;
			for (int i = 0; i < size; i++)
			{
				for (int j = 0; j < arraySize; j++)
				{
					bs2[j] = bs1[j];
				}
			}
			long step2 = OdbTime.GetCurrentTimeInTicks();
			long time2 = step2 - step1;
			for (int i = 0; i < size; i++)
			{
				bs2 = (byte[])bs1;
			}
			long step3 = OdbTime.GetCurrentTimeInTicks();
			long time3 = step3 - step2;
			Println("ArraySize=" + arraySize + " : arraycopy=" + time1 + " - loop copy=" + time2
				 + " - clone=" + time3);
			AssertTrue(time1 <= time2);
		}
	}
}
