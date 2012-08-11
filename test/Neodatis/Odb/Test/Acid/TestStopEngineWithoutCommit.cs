using NDatabase.Odb;
using NDatabase.Odb.Core.Query.Criteria;
using NDatabase.Odb.Impl.Core.Query.Criteria;
using NUnit.Framework;
using Test.Odb.Test.VO.Login;

namespace Test.Odb.Test.Acid
{
	[TestFixture]
    public class TestStopEngineWithoutCommit : ODBTest
	{
		private bool simpleObject;

		private ODBTest test = new ODBTest();

		[Test]
        public virtual void Test1()
		{
		}

		// just to avoid junit warning
		/// <exception cref="System.Exception"></exception>
		public virtual void T1estA1()
		{
			test.DeleteBase("acid1");
			IOdb odb = test.Open("acid1");
			odb.Store(GetInstance("f1"));
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void T1estA2()
		{
			IOdb odb = test.Open("acid1");
			AssertEquals(0, odb.GetObjects<VO.Login.Function>().Count
				);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void T1estB1()
		{
			IOdb odb = test.Open("acid1");
			odb.Store(GetInstance("f1"));
			odb.Commit();
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void T1estB2()
		{
			IOdb odb = test.Open("acid1");
			int size = 0;
			if (simpleObject)
			{
				size = odb.GetObjects<VO.Login.Function>().Count;
			}
			else
			{
				size = odb.GetObjects<User>().Count;
			}
			if (size != 1)
			{
				throw new System.Exception("Size should be " + 1 + " and it is " + size);
			}
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void T1estC1()
		{
			test.DeleteBase("acid1");
			IOdb odb = test.Open("acid1");
			int size = 1;
			OID[] oids = new OID[size];
			for (int i = 0; i < size; i++)
			{
				oids[i] = odb.Store(GetInstance("f" + i));
			}
			for (int i = 0; i < size; i++)
			{
				odb.DeleteObjectWithId(oids[i]);
			}
		}

		private object GetInstance(string @string)
		{
			if (simpleObject)
			{
				return new VO.Login.Function(@string);
			}
			Profile p = new Profile(@string
				);
			p.AddFunction(new VO.Login.Function("function " + @string + "1"
				));
			p.AddFunction(new VO.Login.Function("function " + @string + "2"
				));
			User user = new User(@string
				, "email" + @string, p);
			return user;
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void T1estC2()
		{
			IOdb odb = test.Open("acid1");
			AssertEquals(0, odb.GetObjects<VO.Login.Function>().Count
				);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void T1estD1()
		{
			test.DeleteBase("acid1");
			IOdb odb = test.Open("acid1");
			int size = 1000;
			OID[] oids = new OID[size];
			for (int i = 0; i < size; i++)
			{
				oids[i] = odb.Store(GetInstance("f" + i));
			}
			for (int i = 0; i < size; i++)
			{
				odb.DeleteObjectWithId(oids[i]);
			}
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void T1estD2()
		{
			IOdb odb = test.Open("acid1");
			AssertEquals(0, odb.GetObjects<VO.Login.Function>().Count
				);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void T1estE1()
		{
			test.DeleteBase("acid1");
			IOdb odb = test.Open("acid1");
			int size = 1000;
			OID[] oids = new OID[size];
			for (int i = 0; i < size; i++)
			{
				oids[i] = odb.Store(GetInstance("f" + i));
				if (simpleObject)
				{
					VO.Login.Function f = (VO.Login.Function)odb.
						GetObjectFromId(oids[i]);
					f.SetName("function " + i);
					odb.Store(f);
				}
				else
				{
					User f = (User)odb.GetObjectFromId
						(oids[i]);
					f.SetName("function " + i);
					odb.Store(f);
				}
				odb.DeleteObjectWithId(oids[i]);
			}
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void T1estE2()
		{
			IOdb odb = test.Open("acid1");
			AssertEquals(0, odb.GetObjects<VO.Login.Function>().Count
				);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void T1estF1()
		{
			test.DeleteBase("acid1");
			IOdb odb = test.Open("acid1");
			int size = 1000;
			OID[] oids = new OID[size];
			for (int i = 0; i < size; i++)
			{
				oids[i] = odb.Store(GetInstance("f" + i));
				if (simpleObject)
				{
					VO.Login.Function f = (VO.Login.Function)odb.
						GetObjectFromId(oids[i]);
					f.SetName("function " + i);
					odb.Store(f);
					odb.Store(f);
					odb.Store(f);
					odb.Store(f);
				}
				else
				{
					User f = (User)odb.GetObjectFromId
						(oids[i]);
					f.SetName("function " + i);
					odb.Store(f);
					odb.Store(f);
					odb.Store(f);
					odb.Store(f);
				}
			}
			for (int i = 0; i < size; i++)
			{
				object o = odb.GetObjectFromId(oids[i]);
				odb.Delete(o);
			}
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void T1estF2()
		{
			IOdb odb = test.Open("acid1");
			if (simpleObject)
			{
				AssertEquals(0, odb.GetObjects<VO.Login.Function>().Count
					);
			}
			else
			{
				AssertEquals(0, odb.GetObjects<User>().Count);
			}
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void T1estG1()
		{
			test.DeleteBase("acid1");
			IOdb odb = test.Open("acid1");
			int size = 1000;
			OID[] oids = new OID[size];
			for (int i = 0; i < size; i++)
			{
				oids[i] = odb.Store(GetInstance("f" + i));
				if (simpleObject)
				{
					VO.Login.Function f = (VO.Login.Function)odb.
						GetObjectFromId(oids[i]);
					f.SetName("function " + i);
					odb.Store(f);
					odb.Store(f);
					odb.Store(f);
					odb.Store(f);
				}
				else
				{
					User f = (User)odb.GetObjectFromId
						(oids[i]);
					f.SetName("function " + i);
					odb.Store(f);
					odb.Store(f);
					odb.Store(f);
					odb.Store(f);
				}
			}
			odb.Commit();
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void T1estG2()
		{
			IOdb odb = test.Open("acid1");
			int size = 1000;
			OID[] oids = new OID[size];
			for (int i = 0; i < size; i++)
			{
				oids[i] = odb.Store(GetInstance("f" + i));
				if (simpleObject)
				{
					VO.Login.Function f = (VO.Login.Function)odb.
						GetObjectFromId(oids[i]);
					f.SetName("function " + i);
					odb.Store(f);
					odb.Store(f);
					odb.Store(f);
					odb.Store(f);
				}
				else
				{
					User f = (User)odb.GetObjectFromId
						(oids[i]);
					f.SetName("function " + i);
					odb.Store(f);
					odb.Store(f);
					odb.Store(f);
					odb.Store(f);
				}
			}
			for (int i = 0; i < size; i++)
			{
				object o = null;
				o = odb.GetObjectFromId(oids[i]);
				odb.Delete(o);
			}
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void T1estG3()
		{
			IOdb odb = test.Open("acid1");
			if (simpleObject)
			{
				AssertEquals(1000, odb.GetObjects<VO.Login.Function>().Count
					);
			}
			else
			{
				AssertEquals(1000, odb.GetObjects<User>().Count);
			}
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void T1estH1()
		{
			test.DeleteBase("acid1");
			IOdb odb = test.Open("acid1");
			int size = 1000;
			OID[] oids = new OID[size];
			for (int i = 0; i < size; i++)
			{
				oids[i] = odb.Store(GetInstance("f" + i));
				if (simpleObject)
				{
					VO.Login.Function f = (VO.Login.Function)odb.
						GetObjectFromId(oids[i]);
					f.SetName("function " + i);
					odb.Store(f);
					odb.Delete(f);
					oids[i] = odb.Store(f);
					odb.Delete(f);
					oids[i] = odb.Store(f);
					odb.Delete(f);
					oids[i] = odb.Store(f);
				}
				else
				{
					User f = (User)odb.GetObjectFromId
						(oids[i]);
					f.SetName("function " + i);
					odb.Store(f);
					odb.Delete(f);
					oids[i] = odb.Store(f);
					odb.Delete(f);
					oids[i] = odb.Store(f);
					odb.Delete(f);
					oids[i] = odb.Store(f);
				}
			}
			for (int i = 0; i < size; i++)
			{
				object o = odb.GetObjectFromId(oids[i]);
				odb.Delete(o);
			}
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void T1estH2()
		{
			IOdb odb = test.Open("acid1");
			if (simpleObject)
			{
				AssertEquals(0, odb.GetObjects<VO.Login.Function>().Count
					);
			}
			else
			{
				AssertEquals(0, odb.GetObjects<User>().Count);
			}
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void T1estI1()
		{
			test.DeleteBase("acid1");
			IOdb odb = test.Open("acid1");
			odb.Store(GetInstance("f1"));
			odb.Store(GetInstance("f2"));
			odb.Store(GetInstance("f3"));
			odb.Close();
			odb = test.Open("acid1");
			object o = GetInstance("f4");
			odb.Store(o);
			odb.Delete(o);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void T1estI2()
		{
			IOdb odb = test.Open("acid1");
			if (simpleObject)
			{
				AssertEquals(3, odb.GetObjects<VO.Login.Function>().Count
					);
			}
			else
			{
				AssertEquals(3, odb.GetObjects<User>().Count);
			}
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void T1estJ1()
		{
			test.DeleteBase("acid1");
			IOdb odb = test.Open("acid1");
			odb.Store(GetInstance("f1"));
			odb.Store(GetInstance("f2"));
			odb.Store(GetInstance("f3"));
			odb.Commit();
			object o = GetInstance("f4");
			odb.Store(o);
			odb.Delete(o);
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void T1estJ2()
		{
			IOdb odb = test.Open("acid1");
			if (simpleObject)
			{
				AssertEquals(3, odb.GetObjects<VO.Login.Function>().Count
					);
			}
			else
			{
				AssertEquals(3, odb.GetObjects<User>().Count);
			}
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void T1estK1()
		{
			test.DeleteBase("acid1");
			IOdb odb = test.Open("acid1");
			odb.Store(GetInstance("f1"));
			odb.Store(GetInstance("f2"));
			OID oid = odb.Store(GetInstance("f3"));
			odb.Commit();
			object o = odb.GetObjectFromId(oid);
			odb.Delete(o);
			odb.Rollback();
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void T1estK2()
		{
			IOdb odb = test.Open("acid1");
			if (simpleObject)
			{
				AssertEquals(3, odb.GetObjects<VO.Login.Function>().Count
					);
			}
			else
			{
				AssertEquals(3, odb.GetObjects<User>().Count);
			}
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void T1estL1()
		{
			test.DeleteBase("acid1");
			IOdb odb = test.Open("acid1");
			odb.Store(GetInstance("f1"));
			odb.Store(GetInstance("f2"));
			OID oid = odb.Store(GetInstance("f3"));
			odb.Commit();
			object o = odb.GetObjectFromId(oid);
			if (simpleObject)
			{
				VO.Login.Function f = (VO.Login.Function)o;
				f.SetName("flksjdfjs;dfsljflsjflksjfksjfklsdjfksjfkalsjfklsdjflskd");
				odb.Store(f);
			}
			else
			{
				User f = (User)o;
				f.SetName("flksjdfjs;dfsljflsjflksjfksjfklsdjfksjfkalsjfklsdjflskd");
				odb.Store(f);
			}
			odb.Rollback();
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void T1estL2()
		{
			IOdb odb = test.Open("acid1");
			if (simpleObject)
			{
				AssertEquals(3, odb.GetObjects<VO.Login.Function>().Count
					);
			}
			else
			{
				AssertEquals(3, odb.GetObjects<User>().Count);
			}
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void T1estM1()
		{
			test.DeleteBase("acid1");
			IOdb odb = test.Open("acid1");
			int size = 1;
			OID[] oids = new OID[size];
			for (int i = 0; i < size; i++)
			{
				oids[i] = odb.Store(GetInstance("f" + i));
			}
			for (int i = 0; i < size; i++)
			{
				odb.DeleteObjectWithId(oids[i]);
			}
			odb.Rollback();
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void T1estM2()
		{
			IOdb odb = test.Open("acid1");
			if (simpleObject)
			{
				AssertEquals(0, odb.GetObjects<VO.Login.Function>().Count
					);
			}
			else
			{
				AssertEquals(0, odb.GetObjects<User>().Count);
			}
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void T1estN1()
		{
			test.DeleteBase("acid1");
			IOdb odb = test.Open("acid1");
			for (int i = 0; i < 10; i++)
			{
				odb.Store(GetInstance("f" + i));
			}
			odb.Close();
			odb = test.Open("acid1");
			odb.Store(GetInstance("f1000"));
			odb.Commit();
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void T1estN2()
		{
			IOdb odb = test.Open("acid1");
			if (simpleObject)
			{
				IObjects<VO.Login.Function> objects = odb.GetObjects<VO.Login.Function>(new CriteriaQuery(Where.Equal("name", "f1000")));
				VO.Login.Function f = objects.GetFirst();
				f.SetName("new name");
				odb.Store(f);
			}
			else
			{
				IObjects<User> objects = odb.GetObjects<User>(new CriteriaQuery(Where.Equal("name", "f1000")));
				User f = objects.GetFirst();
				f.SetName("new name");
				odb.Store(f);
			}
			odb.Commit();
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void T1estN3()
		{
			IOdb odb = test.Open("acid1");
			if (simpleObject)
			{
				IObjects<VO.Login.Function> objects = odb.GetObjects<VO.Login.Function>(new CriteriaQuery(Where.Equal("name", "new name")));
				odb.Delete(objects.GetFirst());
			}
			else
			{
				IObjects<User> objects = odb.GetObjects<User>(new CriteriaQuery(Where.Equal("name", "new name")));
				odb.Delete(objects.GetFirst());
			}
			odb.Commit();
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void T1estN4()
		{
			IOdb odb = test.Open("acid1");
			int nb = 0;
			if (simpleObject)
			{
				IObjects<VO.Login.Function> objects = odb.GetObjects<VO.Login.Function>(new CriteriaQuery(Where.Equal("name", "f1000")));
				nb = objects.Count;
			}
			else
			{
				IObjects<User> objects = odb.GetObjects<User>(new CriteriaQuery(Where.Equal("name", "f1000")));
				nb = objects.Count;
			}
			if (nb != 0)
			{
				throw new System.Exception("Object f1000 still exist :-(");
			}
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void Execute(string[] args)
		{
			string step = args[0];
			simpleObject = args[1].Equals("simple");
            System.Reflection.MethodInfo method = null;// OdbReflection.GetMethods(this.GetType(), step, new System.Type[0]);
			try
			{
				method.Invoke(this, new object[0]);
				TestOk(step);
			}
			catch (System.Exception e)
			{
				// println("Error while calling " + step);
				TestBad(step, e);
			}
		}

		// e.printStackTrace();
		private void TestBad(string step, System.Exception e)
		{
			Println(step + " Not ok " + e.InnerException.Message);
			
		}

		private void TestOk(string step)
		{
			Println(step + " Ok ");
		}

		/// <exception cref="System.Exception"></exception>
		public static void Main2(string[] args)
		{
			TestStopEngineWithoutCommit tf = new TestStopEngineWithoutCommit
				();
			try
			{
				tf.Execute(args);
			}
			catch (System.Exception e)
			{
			}
		}
	}
}
