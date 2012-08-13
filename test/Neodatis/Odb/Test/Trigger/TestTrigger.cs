using NDatabase.Odb;
using Test.Odb.Test;
using NUnit.Framework;
using Test.Odb.Test.VO.Login;

namespace Trigger
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
			Trigger.MyTrigger myTrigger = new Trigger.MyTrigger
				();
			try
			{
				odb = Open("trigger.neodatis");
				odb.AddInsertTrigger(typeof(User), myTrigger);
				Function f1 = new Function(
					"function1");
				Function f2 = new Function(
					"function2");
				Profile profile = new Profile
					("profile1", f1);
				User user = new User("oli", 
					"oli@neodatis.com", profile);
				odb.Store(user);
			}
			finally
			{
				if (odb != null)
				{
					odb.Close();
				}
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
			Trigger.MyTrigger myTrigger = new Trigger.MyTrigger
				();
			try
			{
				odb = Open("trigger.neodatis");
				odb.AddInsertTrigger(typeof(Function), myTrigger);
				Function f1 = new Function(
					"function1");
				Function f2 = new Function(
					"function2");
				Profile profile = new Profile
					("profile1", f1);
				User user = new User("oli", 
					"oli@neodatis.com", profile);
				odb.Store(user);
				odb.Store(f2);
			}
			finally
			{
				if (odb != null)
				{
					odb.Close();
				}
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
			Trigger.MySelectTrigger myTrigger = new Trigger.MySelectTrigger
				();
			try
			{
				odb = Open("trigger.neodatis");
				Function f1 = new Function(
					"function1");
				Function f2 = new Function(
					"function2");
				Profile profile = new Profile
					("profile1", f1);
				User user = new User("oli", 
					"oli@neodatis.com", profile);
				odb.Store(user);
				odb.Store(f2);
			}
			finally
			{
				if (odb != null)
				{
					odb.Close();
				}
			}
			odb = Open("trigger.neodatis");
			odb.AddSelectTrigger(typeof(Function), myTrigger);
			var functions = odb.GetObjects<Function>();
			odb.Close();
			DeleteBase("trigger.neodatis");
			AssertEquals(2, functions.Count);
			AssertEquals(2, myTrigger.nbCalls);
		}
	}
}
