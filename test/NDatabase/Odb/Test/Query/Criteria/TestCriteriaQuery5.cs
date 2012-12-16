using NDatabase2.Odb.Core.Query.Criteria;
using NUnit.Framework;
using Test.NDatabase.Odb.Test.VO.Attribute;

namespace Test.NDatabase.Odb.Test.Query.Criteria
{
    [TestFixture]
    public class TestCriteriaQuery5 : ODBTest
    {
        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void TestCriteriaWithDate()
        {
            var baseName = GetBaseName();
            var odb = Open(baseName);
            for (var i = 0; i < 10; i++)
            {
                var tc = new TestClass();
                tc.SetInt1(i);
                odb.Store(tc);
            }
            odb.Close();
            odb = Open(baseName);
            var query = new CriteriaQuery<TestClass>();
            query.GreaterOrEqual("int1", 0);
            var os = odb.Query<TestClass>(query);
            AssertEquals(10, os.Count);
            var j = 0;
            while (os.HasNext())
            {
                var tc = os.Next();
                AssertEquals(j, tc.GetInt1());
                j++;
            }
            odb.Close();
        }

        [Test]
        public virtual void TestIntLongCriteriaQuery()
        {
            var baseName = GetBaseName();

            var odb = Open(baseName);
            var cwi = new ClassWithInt(1, "test");
            odb.Store(cwi);
            odb.Close();
            odb = Open(baseName);
            var query = new CriteriaQuery<ClassWithInt>();
            query.Equal("i", (long) 1);
            var os = odb.Query<ClassWithInt>(query);
            AssertEquals(1, os.Count);
            odb.Close();
        }

        [Test]
        public virtual void TestLongIntCriteriaQuery()
        {
            var baseName = GetBaseName();
            var odb = Open(baseName);
            var cwl = new ClassWithLong(1L, "test");
            odb.Store(cwl);
            odb.Close();
            odb = Open(baseName);
            var query = new CriteriaQuery<ClassWithLong>();
            query.Equal("i", 1L);
            var os = odb.Query<ClassWithLong>(query);
            AssertEquals(1, os.Count);
            odb.Close();
        }

        [Test]
        public virtual void TestLongIntCriteriaQueryGt()
        {
            var baseName = GetBaseName();
            var odb = Open(baseName);
            var cwl = new ClassWithLong(1L, "test");
            odb.Store(cwl);
            odb.Close();
            odb = Open(baseName);
            var criteriaQuery = new CriteriaQuery<ClassWithLong>();
            criteriaQuery.GreaterOrEqual("i", 1L);
            var os = odb.Query<ClassWithLong>(criteriaQuery);
            AssertEquals(1, os.Count);
            var query = new CriteriaQuery<ClassWithLong>();
            query.GreaterThan("i", 1L);
            os = odb.Query<ClassWithLong>(query);
            AssertEquals(0, os.Count);
            odb.Close();
        }
    }
}
