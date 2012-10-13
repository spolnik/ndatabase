using NDatabase2.Odb.Core.Query;
using NDatabase2.Odb.Core.Query.NQ;
using NUnit.Framework;
using Test.NDatabase.Odb.Test.Tutorial;
using Test.NDatabase.Odb.Test.VO;

namespace Test.NDatabase.Odb.Test.Query.NQ
{
    [TestFixture]
    public class TestNativeQuery : ODBTest
    {
        public class Query1 : SimpleNativeQuery<ClassWithArrayOfBoolean>
        {
            public override bool Match(ClassWithArrayOfBoolean o)
            {
                return true;
            }
        }

        public class Query2 : SimpleNativeQuery<Player>
        {
            public override bool Match(Player player)
            {
                return player.GetFavoriteSport().GetName().ToLower().StartsWith("volley");
            }
        }

        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void Test1()
        {
            var baseName = GetBaseName();
            var odb = Open(baseName);
            var bbs1 = new bool[2];
            bbs1[0] = true;
            bbs1[1] = false;
            var bbs2 = new bool[2];
            bbs2[0] = true;
            bbs2[1] = false;
            var o = new ClassWithArrayOfBoolean("test", bbs1, bbs2);
            odb.Store(o);
            odb.Close();
            odb = Open(baseName);
            IQuery query = new Query1();
            var objects = odb.GetObjects<ClassWithArrayOfBoolean>(new Query1());
            AssertEquals(1, objects.Count);
            var o2 = objects.GetFirst();
            AssertEquals("test", o2.GetName());
            AssertEquals(true, o2.GetBools1()[0]);
            AssertEquals(false, o2.GetBools1()[1]);
            AssertEquals(true, o2.GetBools2()[0]);
            AssertEquals(false, o2.GetBools2()[1]);
        }
    }
}
