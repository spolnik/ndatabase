using NDatabase.Odb;
using NUnit.Framework;
using Test.Odb.Test;

namespace Trigger
{
	[TestFixture]
    public class TestAutoIncrementTrigger : ODBTest
	{
		public static readonly string Base = "trigger-auto-increment.neodatis";

		/// <exception cref="System.Exception"></exception>
		[Test]
        public virtual void Test1()
		{
			IOdb odb = null;
			DeleteBase(Base);
			try
			{
				odb = Open(Base);
				odb.AddInsertTrigger(typeof(Trigger.ObjectWithAutoIncrementId), new LocalAutoIncrementTrigger());
				Trigger.ObjectWithAutoIncrementId o = new Trigger.ObjectWithAutoIncrementId
					("Object 1");
				odb.Store(o);
				AssertEquals(1, o.GetId());
			}
			finally
			{
				if (odb != null)
				{
					odb.Close();
				}
			}
		}

		/// <exception cref="System.Exception"></exception>
		[Test]
        public virtual void Test2Objects()
		{
			IOdb odb = null;
			DeleteBase(Base);
			try
			{
				odb = Open(Base);
				odb.AddInsertTrigger(typeof(Trigger.ObjectWithAutoIncrementId), 
					new Trigger.LocalAutoIncrementTrigger());
				Trigger.ObjectWithAutoIncrementId o = new Trigger.ObjectWithAutoIncrementId
					("Object 1");
				odb.Store(o);
				AssertEquals(1, o.GetId());
				odb.Close();
				odb = Open(Base);
				odb.AddInsertTrigger(typeof(Trigger.ObjectWithAutoIncrementId), 
					new Trigger.LocalAutoIncrementTrigger());
				o = new Trigger.ObjectWithAutoIncrementId("Object 2");
				odb.Store(o);
				AssertEquals(2, o.GetId());
			}
			finally
			{
				if (odb != null)
				{
					odb.Close();
				}
			}
		}

		/// <exception cref="System.Exception"></exception>
		[Test]
        public virtual void Test1000Objects()
		{
			IOdb odb = null;
			DeleteBase(Base);
			try
			{
				odb = Open(Base);
				odb.AddInsertTrigger(typeof(Trigger.ObjectWithAutoIncrementId), 
					new Trigger.LocalAutoIncrementTrigger());
				for (int i = 0; i < 1000; i++)
				{
					Trigger.ObjectWithAutoIncrementId o = new Trigger.ObjectWithAutoIncrementId
						("Object " + (i + 1));
					odb.Store(o);
					AssertEquals(i + 1, o.GetId());
				}
				odb.Close();
				odb = Open(Base);
				odb.AddInsertTrigger(typeof(Trigger.ObjectWithAutoIncrementId), 
					new Trigger.LocalAutoIncrementTrigger());
				for (int i = 0; i < 1000; i++)
				{
					Trigger.ObjectWithAutoIncrementId o = new Trigger.ObjectWithAutoIncrementId
						("Object - bis - " + (i + 1));
					odb.Store(o);
					AssertEquals(1000 + i + 1, o.GetId());
				}
				odb.Close();
			}
			finally
			{
			}
		}
	}
}
