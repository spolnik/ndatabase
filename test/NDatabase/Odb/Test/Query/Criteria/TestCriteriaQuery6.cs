using System.Collections.Generic;
using NDatabase2.Odb;
using NDatabase2.Odb.Core.Query;
using NDatabase2.Odb.Core.Query.Criteria;
using NUnit.Framework;
using Test.NDatabase.Odb.Test.VO.Login;

namespace Test.NDatabase.Odb.Test.Query.Criteria
{
    [TestFixture]
    public class TestCriteriaQuery6 : ODBTest
    {
        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void Test1()
        {
            var baseName = GetBaseName();
            var odb = Open(baseName);
            IList<Profile> profiles = new List<Profile>();
            profiles.Add(new Profile("p1", new VO.Login.Function("f1")));
            profiles.Add(new Profile("p2", new VO.Login.Function("f2")));
            var cb = new ClassB("name", profiles);
            odb.Store(cb);
            odb.Close();
            odb = Open(baseName);
            // this object is not known y NDatabase so the query will not return anything
            var p = new Profile("p1", (IList<VO.Login.Function>) null);
            var query = odb.CreateCriteriaQuery<ClassB>();
            query.Descend("profiles").Contain(p);
            var l = query.Execute<ClassB>();
            odb.Close();
            AssertEquals(0, l.Count);
        }

        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void Test2()
        {
            var baseName = GetBaseName();
            var odb = Open(baseName);
            IList<Profile> profiles = new List<Profile>();
            profiles.Add(new Profile("p1", new VO.Login.Function("f1")));
            profiles.Add(new Profile("p2", new VO.Login.Function("f2")));
            var cb = new ClassB("name", profiles);
            odb.Store(cb);
            odb.Close();
            odb = Open(baseName);
            var p = odb.Query<Profile>().GetFirst();
            var query = odb.CreateCriteriaQuery<ClassB>();
            query.Descend("profiles").Contain(p);
            var l = query.Execute<ClassB>();
            odb.Close();
            AssertEquals(1, l.Count);
        }

        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void TestReuse()
        {
            var baseName = GetBaseName();
            var odb = Open(baseName);
            IList<Profile> profiles = new List<Profile>();
            profiles.Add(new Profile("p1", new VO.Login.Function("f1")));
            profiles.Add(new Profile("p2", new VO.Login.Function("f2")));
            var cb = new ClassB("name", profiles);
            odb.Store(cb);
            odb.Close();
            odb = Open(baseName);
            var query = odb.CreateCriteriaQuery<ClassB>();
            query.Descend("name").Equal("name");
            var l = query.Execute<ClassB>();
            odb.Close();
            AssertEquals(1, l.Count);
        }
    }
}
