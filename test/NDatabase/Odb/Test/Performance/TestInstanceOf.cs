using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Tool.Wrappers;
using NUnit.Framework;

namespace Test.NDatabase.Odb.Test.Performance
{
    [TestFixture]
    public class TestInstanceOf : ODBTest
    {
        private const int size = 100000000;

        [Test]
        public virtual void TestIf()
        {
            var start = OdbTime.GetCurrentTimeInTicks();
            var nnoi = new NonNativeObjectInfo(null);
            for (var i = 0; i < size; i++)
            {
                if (nnoi.IsCollectionObject())
                {
                }
            }
            // println("time if=" + (OdbTime.getCurrentTimeInMs()-start));
            AssertTrue(true);
        }

        [Test]
        public virtual void TestIfInstanceOf()
        {
            var start = OdbTime.GetCurrentTimeInTicks();
            var nnoi = new NonNativeObjectInfo(null);
            for (var i = 0; i < size; i++)
            {
                if (nnoi is NonNativeObjectInfo)
                {
                }
            }
            // println("time instance of=" + (OdbTime.getCurrentTimeInMs()-start));
            AssertTrue(true);
        }
    }
}
