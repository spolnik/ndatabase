using NDatabase.Odb;
using NDatabase2.Odb;
using NUnit.Framework;

namespace Test.NDatabase.Odb.Test.Resistance
{
    [TestFixture]
    public class TestResistance : ODBTest
    {
        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void Test8()
        {
            var size1 = 10;
            var size2 = 100;

            var baseName = GetBaseName();
            IOdb odb = null;
            IObjects<VO.Login.Function> os = null;
            for (var i = 0; i < size1; i++)
            {
                odb = Open(baseName);
                for (var j = 0; j < size2; j++)
                {
                    var f = new VO.Login.Function("function " + j);
                    odb.Store(f);
                }
                odb.Close();
                odb = Open(baseName);
                os = odb.GetObjects<VO.Login.Function>();
                while (os.HasNext())
                {
                    var f = os.Next();
                    odb.Delete(f);
                }
                odb.Close();
                if (i % 100 == 0)
                    Println(i + "/" + size1);
            }
            odb = Open(baseName);
            os = odb.GetObjects<VO.Login.Function>();
            AssertEquals(0, os.Count);
            odb.Close();
            Println("step2");
            for (var i = 0; i < size1; i++)
            {
                odb = Open(baseName);
                os = odb.GetObjects<VO.Login.Function>();
                while (os.HasNext())
                {
                    var f = os.Next();
                    odb.Delete(f);
                }
                odb.Close();
                odb = Open(baseName);
                for (var j = 0; j < size2; j++)
                {
                    var f = new VO.Login.Function("function " + j);
                    odb.Store(f);
                }
                odb.Close();
                if (i % 100 == 0)
                    Println(i + "/" + size1);
            }
            odb = Open(baseName);
            os = odb.GetObjects<VO.Login.Function>();
            AssertEquals(size2, os.Count);
            odb.Close();
            DeleteBase(baseName);
        }
    }
}
