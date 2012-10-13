using System;
using NDatabase.Odb;
using NDatabase2.Odb;
using NDatabase2.Odb.Core.Query.Criteria;
using NDatabase2.Tool.Wrappers;
using NUnit.Framework;

namespace Test.NDatabase.Odb.Test.List
{
    [TestFixture]
    public class TestFieldComparatorSort : ODBTest
    {
        [Test]
        public virtual void Test1()
        {
            var baseName = GetBaseName();
            IOdb odb = null;
            var k = 10;
            var t1 = OdbTime.GetCurrentTimeInMs();
            odb = Open(baseName);
            for (var i = 0; i < k; i++)
            {
                odb.Store(new User("john" + (k - i), "john@ibm.com", "ny 875", k - i + 1, new DateTime(t1 - i * 1000),
                                   i % 2 == 0));
                odb.Store(new User("john" + (k - i), "john@ibm.com", "ny 875", k - i, new DateTime(t1 - i * 1000),
                                   (i + 1) % 2 == 0));
            }
            odb.Close();
            odb = Open(baseName);
            var q = new CriteriaQuery<User>().OrderByAsc("name,id");
            var users = odb.GetObjects<User>(q);
            odb.Close();
            if (k < 11)
            {
                //NDatabase.Tool.DisplayUtility.Display("test1", users);
            }
            var user = users.GetFirst();
            AssertTrue(user.GetName().StartsWith("john1"));
            AssertEquals(1, user.GetId());
        }

        [Test]
        public virtual void Test1_2()
        {
            var baseName = GetBaseName();
            IOdb odb = null;
            var k = 10;
            var t1 = OdbTime.GetCurrentTimeInMs();
            odb = Open(baseName);
            for (var i = 0; i < k; i++)
            {
                odb.Store(new User("john" + (k - i), "john@ibm.com", "ny 875", k - i + 1, new DateTime(t1 - i * 1000),
                                   i % 2 == 0));
                odb.Store(new User("john" + (k - i), "john@ibm.com", "ny 875", k - i, new DateTime(t1 - i * 1000),
                                   (i + 1) % 2 == 0));
            }
            odb.Close();
            odb = Open(baseName);
            var q = new CriteriaQuery<User>().OrderByDesc("name,id");
            var users = odb.GetObjects<User>(q);
            odb.Close();
            if (k < 11)
            {
                //NDatabase.Tool.DisplayUtility.Display("test1", users);
            }
            var user = users.GetFirst();
            AssertTrue(user.GetName().StartsWith("john9"));
            AssertEquals(10, user.GetId());
        }

        [Test]
        public virtual void Test2()
        {
            var baseName = GetBaseName();
            IOdb odb = null;
            var k = 10;
            var t1 = OdbTime.GetCurrentTimeInMs();
            var fields = new[] {"ok", "id", "name"};
            odb = Open(baseName);
            for (var i = 0; i < k; i++)
            {
                odb.Store(new User("john" + (k - i), "john@ibm.com", "ny 875", k - i + 1, new DateTime(t1 - i * 1000),
                                   i % 2 == 0));
                odb.Store(new User("john" + (k - i), "john@ibm.com", "ny 875", k - i, new DateTime(t1 - i * 1000),
                                   (i + 1) % 2 == 0));
            }
            odb.Close();
            odb = Open(baseName);
            var q = new CriteriaQuery<User>().OrderByAsc("ok,id,name");
            var users = odb.GetObjects<User>(q);
            odb.Close();
            if (k < 11)
            {
                //NDatabase.Tool.DisplayUtility.Display("test1", users);
            }
            var user = users.GetFirst();
            AssertTrue(user.GetName().StartsWith("john1"));
            AssertEquals(2, user.GetId());
        }

        [Test]
        public virtual void Test2_2()
        {
            var baseName = GetBaseName();
            IOdb odb = null;
            var k = 10;
            var t1 = OdbTime.GetCurrentTimeInMs();
            var fields = new[] {"ok", "id", "name"};
            odb = Open(baseName);
            for (var i = 0; i < k; i++)
            {
                odb.Store(new User("john" + (k - i), "john@ibm.com", "ny 875", k - i + 1, new DateTime(t1 - i * 1000),
                                   i % 2 == 0));
                odb.Store(new User("john" + (k - i), "john@ibm.com", "ny 875", k - i, new DateTime(t1 - i * 1000),
                                   (i + 1) % 2 == 0));
            }
            odb.Close();
            odb = Open(baseName);
            var q = new CriteriaQuery<User>().OrderByDesc("ok,id,name");
            var users = odb.GetObjects<User>(q);
            odb.Close();
            if (k < 11)
            {
                //NDatabase.Tool.DisplayUtility.Display("test1", users);
            }
            var user = users.GetFirst();
            AssertTrue(user.GetName().StartsWith("john10"));
            AssertEquals(11, user.GetId());
        }
    }
}
