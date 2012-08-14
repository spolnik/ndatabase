using System;
using NDatabase.Odb;
using NDatabase.Odb.Impl.Core.Query.Criteria;
using NUnit.Framework;
using Test.Odb.Test;
using Test.Odb.Test.VO.Login;

namespace Defragment
{
    [TestFixture]
    public class TestDefragment : ODBTest
    {
        /// <summary>
        ///   The name of the database file
        /// </summary>
        public static readonly string OdbFileName1 = "defrag1.neodatis";

        public static readonly string OdbFileName2 = "defrag1-bis.neodatis";

        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void Test1()
        {
            DeleteBase(OdbFileName1);
            DeleteBase(OdbFileName2);
            var odb = Open(OdbFileName1);
            var user = new User("olivier", "olivier@neodatis.com", null);
            odb.Store(user);
            odb.Close();
            odb = Open(OdbFileName1);
            odb.DefragmentTo(OdbFileName2);
            var newOdb = Open(OdbFileName2);
            // int n = odb.getObjects(User.class).size();
            // println("n="+n);
            Decimal nbUser = odb.Count(new CriteriaQuery(typeof (User)));
            Decimal nbNewUser = newOdb.Count(new CriteriaQuery(typeof (User)));
            AssertEquals(nbUser, nbNewUser);
            AssertEquals(odb.Count(new CriteriaQuery(typeof (Profile))),
                         newOdb.Count(new CriteriaQuery(typeof (Profile))));
            odb.Close();
            newOdb.Close();
            DeleteBase(OdbFileName1);
            DeleteBase(OdbFileName2);
        }

        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void Test2()
        {
            DeleteBase(OdbFileName1);
            DeleteBase(OdbFileName2);
            var odb = Open(OdbFileName1);
            var p = new Profile("profile");
            for (var i = 0; i < 500; i++)
            {
                var user = new User("olivier " + i, "olivier@neodatis.com " + i, p);
                odb.Store(user);
            }
            odb.Close();
            odb = Open(OdbFileName1);
            odb.DefragmentTo(OdbFileName2);
            var newOdb = Open(OdbFileName2);
            AssertEquals(odb.Count(new CriteriaQuery(typeof (User))), newOdb.Count(new CriteriaQuery(typeof (User))));
            AssertEquals(odb.Count(new CriteriaQuery(typeof (Profile))),
                         newOdb.Count(new CriteriaQuery(typeof (Profile))));
            odb.Close();
            newOdb.Close();
            DeleteBase(OdbFileName1);
            DeleteBase(OdbFileName2);
        }

        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void Test3()
        {
            DeleteBase(OdbFileName1);
            DeleteBase(OdbFileName2);
            OdbConfiguration.SetAutomaticallyIncreaseCacheSize(true);
            var odb = Open(OdbFileName1);
            for (var i = 0; i < 15000; i++)
            {
                var user = new User("olivier " + i, "olivier@neodatis.com " + i, new Profile("profile" + i));
                odb.Store(user);
            }
            odb.Close();
            odb = Open(OdbFileName1);
            odb.DefragmentTo(OdbFileName2);
            var newOdb = Open(OdbFileName2);
            AssertEquals(odb.Count(new CriteriaQuery(typeof (User))), newOdb.Count(new CriteriaQuery(typeof (User))));
            odb.Close();
            newOdb.Close();
            DeleteBase(OdbFileName1);
            DeleteBase(OdbFileName2);
        }
    }
}
