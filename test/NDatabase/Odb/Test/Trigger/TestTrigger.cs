using NDatabase2.Odb;
using NUnit.Framework;
using Test.NDatabase.Odb.Test.VO.Login;

namespace Test.NDatabase.Odb.Test.Trigger
{
    [TestFixture]
    public class TestTrigger : ODBTest
    {
        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void Test1()
        {
            IOdb odb = null;
            DeleteBase("trigger.neodatis");
            var myTrigger = new MyTrigger();
            try
            {
                odb = Open("trigger.neodatis");
                odb.AddInsertTrigger<User>(myTrigger);
                var f1 = new VO.Login.Function("function1");
                var f2 = new VO.Login.Function("function2");
                var profile = new Profile("profile1", f1);
                var user = new User("oli", "oli@neodatis.com", profile);
                odb.Store(user);
            }
            finally
            {
                if (odb != null)
                    odb.Close();
            }
            odb = Open("trigger.neodatis");
            odb.Close();
            DeleteBase("trigger.neodatis");
            AssertEquals(1, myTrigger.nbInsertsBefore);
            AssertEquals(1, myTrigger.nbInsertsAfter);
        }

        // To test if triggers are called on recursive objects
        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void Test2()
        {
            IOdb odb = null;
            DeleteBase("trigger.neodatis");
            var myTrigger = new MyTrigger();
            try
            {
                odb = Open("trigger.neodatis");
                odb.AddInsertTrigger<VO.Login.Function>(myTrigger);
                var f1 = new VO.Login.Function("function1");
                var f2 = new VO.Login.Function("function2");
                var profile = new Profile("profile1", f1);
                var user = new User("oli", "oli@neodatis.com", profile);
                odb.Store(user);
                odb.Store(f2);
            }
            finally
            {
                if (odb != null)
                    odb.Close();
            }
            odb = Open("trigger.neodatis");
            odb.Close();
            DeleteBase("trigger.neodatis");
            AssertEquals(2, myTrigger.nbInsertsBefore);
            AssertEquals(2, myTrigger.nbInsertsAfter);
        }

        // To test select triggers
        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void TestSelectTrigger()
        {
            IOdb odb = null;
            DeleteBase("trigger.neodatis");
            var myTrigger = new MySelectTrigger();
            try
            {
                odb = Open("trigger.neodatis");
                var f1 = new VO.Login.Function("function1");
                var f2 = new VO.Login.Function("function2");
                var profile = new Profile("profile1", f1);
                var user = new User("oli", "oli@neodatis.com", profile);
                odb.Store(user);
                odb.Store(f2);
            }
            finally
            {
                if (odb != null)
                    odb.Close();
            }
            odb = Open("trigger.neodatis");
            odb.AddSelectTrigger<VO.Login.Function>(myTrigger);
            var functions = odb.Query<VO.Login.Function>();
            odb.Close();
            DeleteBase("trigger.neodatis");
            AssertEquals(2, functions.Count);
            AssertEquals(2, myTrigger.nbCalls);
        }
    }
}
