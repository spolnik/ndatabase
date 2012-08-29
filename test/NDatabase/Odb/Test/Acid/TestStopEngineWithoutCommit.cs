using System;
using System.Reflection;
using NDatabase.Odb;
using NDatabase.Odb.Core.Query.Criteria;
using NDatabase.Odb.Impl.Core.Query.Criteria;
using NUnit.Framework;
using Test.NDatabase.Odb.Test.VO.Login;

namespace Test.NDatabase.Odb.Test.Acid
{
    //TODO: fix tests
    [TestFixture]
    public class TestStopEngineWithoutCommit : ODBTest
    {
        private bool simpleObject;

        private readonly ODBTest test = new ODBTest();

        // just to avoid junit warning

        public virtual void T1estA1()
        {
            test.DeleteBase("acid1");
            var odb = test.Open("acid1");
            odb.Store(GetInstance("f1"));
        }

        public virtual void T1estA2()
        {
            var odb = test.Open("acid1");
            AssertEquals(0, odb.GetObjects<VO.Login.Function>().Count);
        }

        public virtual void T1estB1()
        {
            var odb = test.Open("acid1");
            odb.Store(GetInstance("f1"));
            odb.Commit();
        }

        public virtual void T1estB2()
        {
            var odb = test.Open("acid1");
            var size = 0;
            if (simpleObject)
                size = odb.GetObjects<VO.Login.Function>().Count;
            else
                size = odb.GetObjects<User>().Count;
            if (size != 1)
                throw new Exception("Size should be " + 1 + " and it is " + size);
        }

        public virtual void T1estC1()
        {
            test.DeleteBase("acid1");
            var odb = test.Open("acid1");
            var size = 1;
            var oids = new OID[size];
            for (var i = 0; i < size; i++)
                oids[i] = odb.Store(GetInstance("f" + i));
            for (var i = 0; i < size; i++)
                odb.DeleteObjectWithId(oids[i]);
        }

        private object GetInstance(string @string)
        {
            if (simpleObject)
                return new VO.Login.Function(@string);
            var p = new Profile(@string);
            p.AddFunction(new VO.Login.Function("function " + @string + "1"));
            p.AddFunction(new VO.Login.Function("function " + @string + "2"));
            var user = new User(@string, "email" + @string, p);
            return user;
        }

        public virtual void T1estC2()
        {
            var odb = test.Open("acid1");
            AssertEquals(0, odb.GetObjects<VO.Login.Function>().Count);
        }

        public virtual void T1estD1()
        {
            test.DeleteBase("acid1");
            var odb = test.Open("acid1");
            var size = 1000;
            var oids = new OID[size];
            for (var i = 0; i < size; i++)
                oids[i] = odb.Store(GetInstance("f" + i));
            for (var i = 0; i < size; i++)
                odb.DeleteObjectWithId(oids[i]);
        }

        public virtual void T1estD2()
        {
            var odb = test.Open("acid1");
            AssertEquals(0, odb.GetObjects<VO.Login.Function>().Count);
        }

        public virtual void T1estE1()
        {
            test.DeleteBase("acid1");
            var odb = test.Open("acid1");
            var size = 1000;
            var oids = new OID[size];
            for (var i = 0; i < size; i++)
            {
                oids[i] = odb.Store(GetInstance("f" + i));
                if (simpleObject)
                {
                    var f = (VO.Login.Function) odb.GetObjectFromId(oids[i]);
                    f.SetName("function " + i);
                    odb.Store(f);
                }
                else
                {
                    var f = (User) odb.GetObjectFromId(oids[i]);
                    f.SetName("function " + i);
                    odb.Store(f);
                }
                odb.DeleteObjectWithId(oids[i]);
            }
        }

        public virtual void T1estE2()
        {
            var odb = test.Open("acid1");
            AssertEquals(0, odb.GetObjects<VO.Login.Function>().Count);
        }

        public virtual void T1estF1()
        {
            test.DeleteBase("acid1");
            var odb = test.Open("acid1");
            var size = 1000;
            var oids = new OID[size];
            for (var i = 0; i < size; i++)
            {
                oids[i] = odb.Store(GetInstance("f" + i));
                if (simpleObject)
                {
                    var f = (VO.Login.Function) odb.GetObjectFromId(oids[i]);
                    f.SetName("function " + i);
                    odb.Store(f);
                    odb.Store(f);
                    odb.Store(f);
                    odb.Store(f);
                }
                else
                {
                    var f = (User) odb.GetObjectFromId(oids[i]);
                    f.SetName("function " + i);
                    odb.Store(f);
                    odb.Store(f);
                    odb.Store(f);
                    odb.Store(f);
                }
            }
            for (var i = 0; i < size; i++)
            {
                var o = odb.GetObjectFromId(oids[i]);
                odb.Delete(o);
            }
        }

        public virtual void T1estF2()
        {
            var odb = test.Open("acid1");
            if (simpleObject)
                AssertEquals(0, odb.GetObjects<VO.Login.Function>().Count);
            else
                AssertEquals(0, odb.GetObjects<User>().Count);
        }

        public virtual void T1estG1()
        {
            test.DeleteBase("acid1");
            var odb = test.Open("acid1");
            var size = 1000;
            var oids = new OID[size];
            for (var i = 0; i < size; i++)
            {
                oids[i] = odb.Store(GetInstance("f" + i));
                if (simpleObject)
                {
                    var f = (VO.Login.Function) odb.GetObjectFromId(oids[i]);
                    f.SetName("function " + i);
                    odb.Store(f);
                    odb.Store(f);
                    odb.Store(f);
                    odb.Store(f);
                }
                else
                {
                    var f = (User) odb.GetObjectFromId(oids[i]);
                    f.SetName("function " + i);
                    odb.Store(f);
                    odb.Store(f);
                    odb.Store(f);
                    odb.Store(f);
                }
            }
            odb.Commit();
        }

        public virtual void T1estG2()
        {
            var odb = test.Open("acid1");
            var size = 1000;
            var oids = new OID[size];
            for (var i = 0; i < size; i++)
            {
                oids[i] = odb.Store(GetInstance("f" + i));
                if (simpleObject)
                {
                    var f = (VO.Login.Function) odb.GetObjectFromId(oids[i]);
                    f.SetName("function " + i);
                    odb.Store(f);
                    odb.Store(f);
                    odb.Store(f);
                    odb.Store(f);
                }
                else
                {
                    var f = (User) odb.GetObjectFromId(oids[i]);
                    f.SetName("function " + i);
                    odb.Store(f);
                    odb.Store(f);
                    odb.Store(f);
                    odb.Store(f);
                }
            }
            for (var i = 0; i < size; i++)
            {
                object o = null;
                o = odb.GetObjectFromId(oids[i]);
                odb.Delete(o);
            }
        }

        public virtual void T1estG3()
        {
            var odb = test.Open("acid1");
            if (simpleObject)
                AssertEquals(1000, odb.GetObjects<VO.Login.Function>().Count);
            else
                AssertEquals(1000, odb.GetObjects<User>().Count);
        }

        public virtual void T1estH1()
        {
            test.DeleteBase("acid1");
            var odb = test.Open("acid1");
            var size = 1000;
            var oids = new OID[size];
            for (var i = 0; i < size; i++)
            {
                oids[i] = odb.Store(GetInstance("f" + i));
                if (simpleObject)
                {
                    var f = (VO.Login.Function) odb.GetObjectFromId(oids[i]);
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
                    var f = (User) odb.GetObjectFromId(oids[i]);
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
            for (var i = 0; i < size; i++)
            {
                var o = odb.GetObjectFromId(oids[i]);
                odb.Delete(o);
            }
        }

        public virtual void T1estH2()
        {
            var odb = test.Open("acid1");
            if (simpleObject)
                AssertEquals(0, odb.GetObjects<VO.Login.Function>().Count);
            else
                AssertEquals(0, odb.GetObjects<User>().Count);
        }

        public virtual void T1estI1()
        {
            test.DeleteBase("acid1");
            var odb = test.Open("acid1");
            odb.Store(GetInstance("f1"));
            odb.Store(GetInstance("f2"));
            odb.Store(GetInstance("f3"));
            odb.Close();
            odb = test.Open("acid1");
            var o = GetInstance("f4");
            odb.Store(o);
            odb.Delete(o);
        }

        public virtual void T1estI2()
        {
            var odb = test.Open("acid1");
            if (simpleObject)
                AssertEquals(3, odb.GetObjects<VO.Login.Function>().Count);
            else
                AssertEquals(3, odb.GetObjects<User>().Count);
        }

        public virtual void T1estJ1()
        {
            test.DeleteBase("acid1");
            var odb = test.Open("acid1");
            odb.Store(GetInstance("f1"));
            odb.Store(GetInstance("f2"));
            odb.Store(GetInstance("f3"));
            odb.Commit();
            var o = GetInstance("f4");
            odb.Store(o);
            odb.Delete(o);
        }

        public virtual void T1estJ2()
        {
            var odb = test.Open("acid1");
            if (simpleObject)
                AssertEquals(3, odb.GetObjects<VO.Login.Function>().Count);
            else
                AssertEquals(3, odb.GetObjects<User>().Count);
        }

        public virtual void T1estK1()
        {
            test.DeleteBase("acid1");
            var odb = test.Open("acid1");
            odb.Store(GetInstance("f1"));
            odb.Store(GetInstance("f2"));
            var oid = odb.Store(GetInstance("f3"));
            odb.Commit();
            var o = odb.GetObjectFromId(oid);
            odb.Delete(o);
            odb.Rollback();
        }

        public virtual void T1estK2()
        {
            var odb = test.Open("acid1");
            if (simpleObject)
                AssertEquals(3, odb.GetObjects<VO.Login.Function>().Count);
            else
                AssertEquals(3, odb.GetObjects<User>().Count);
        }

        public virtual void T1estL1()
        {
            test.DeleteBase("acid1");
            var odb = test.Open("acid1");
            odb.Store(GetInstance("f1"));
            odb.Store(GetInstance("f2"));
            var oid = odb.Store(GetInstance("f3"));
            odb.Commit();
            var o = odb.GetObjectFromId(oid);
            if (simpleObject)
            {
                var f = (VO.Login.Function) o;
                f.SetName("flksjdfjs;dfsljflsjflksjfksjfklsdjfksjfkalsjfklsdjflskd");
                odb.Store(f);
            }
            else
            {
                var f = (User) o;
                f.SetName("flksjdfjs;dfsljflsjflksjfksjfklsdjfksjfkalsjfklsdjflskd");
                odb.Store(f);
            }
            odb.Rollback();
        }

        public virtual void T1estL2()
        {
            var odb = test.Open("acid1");
            if (simpleObject)
                AssertEquals(3, odb.GetObjects<VO.Login.Function>().Count);
            else
                AssertEquals(3, odb.GetObjects<User>().Count);
        }

        public virtual void T1estM1()
        {
            test.DeleteBase("acid1");
            var odb = test.Open("acid1");
            var size = 1;
            var oids = new OID[size];
            for (var i = 0; i < size; i++)
                oids[i] = odb.Store(GetInstance("f" + i));
            for (var i = 0; i < size; i++)
                odb.DeleteObjectWithId(oids[i]);
            odb.Rollback();
        }

        public virtual void T1estM2()
        {
            var odb = test.Open("acid1");
            if (simpleObject)
                AssertEquals(0, odb.GetObjects<VO.Login.Function>().Count);
            else
                AssertEquals(0, odb.GetObjects<User>().Count);
        }

        public virtual void T1estN1()
        {
            test.DeleteBase("acid1");
            var odb = test.Open("acid1");
            for (var i = 0; i < 10; i++)
                odb.Store(GetInstance("f" + i));
            odb.Close();
            odb = test.Open("acid1");
            odb.Store(GetInstance("f1000"));
            odb.Commit();
        }

        public virtual void T1estN2()
        {
            var odb = test.Open("acid1");
            if (simpleObject)
            {
                var objects = odb.GetObjects<VO.Login.Function>(new CriteriaQuery(Where.Equal("name", "f1000")));
                var f = objects.GetFirst();
                f.SetName("new name");
                odb.Store(f);
            }
            else
            {
                var objects = odb.GetObjects<User>(new CriteriaQuery(Where.Equal("name", "f1000")));
                var f = objects.GetFirst();
                f.SetName("new name");
                odb.Store(f);
            }
            odb.Commit();
        }

        public virtual void T1estN3()
        {
            var odb = test.Open("acid1");
            if (simpleObject)
            {
                var objects = odb.GetObjects<VO.Login.Function>(new CriteriaQuery(Where.Equal("name", "new name")));
                odb.Delete(objects.GetFirst());
            }
            else
            {
                var objects = odb.GetObjects<User>(new CriteriaQuery(Where.Equal("name", "new name")));
                odb.Delete(objects.GetFirst());
            }
            odb.Commit();
        }

        public virtual void T1estN4()
        {
            var odb = test.Open("acid1");
            var nb = 0;
            if (simpleObject)
            {
                var objects = odb.GetObjects<VO.Login.Function>(new CriteriaQuery(Where.Equal("name", "f1000")));
                nb = objects.Count;
            }
            else
            {
                var objects = odb.GetObjects<User>(new CriteriaQuery(Where.Equal("name", "f1000")));
                nb = objects.Count;
            }
            if (nb != 0)
                throw new Exception("Object f1000 still exist :-(");
        }

        public virtual void Execute(string[] args)
        {
            var step = args[0];
            simpleObject = args[1].Equals("simple");
            MethodInfo method = null; // OdbReflection.GetMethods(this.GetType(), step, new System.Type[0]);
            try
            {
                method.Invoke(this, new object[0]);
                TestOk(step);
            }
            catch (Exception e)
            {
                // println("Error while calling " + step);
                TestBad(step, e);
            }
        }

        // e.printStackTrace();
        private void TestBad(string step, Exception e)
        {
            Println(step + " Not ok " + e.InnerException.Message);
        }

        private void TestOk(string step)
        {
            Println(step + " Ok ");
        }

        public static void Main2(string[] args)
        {
            var tf = new TestStopEngineWithoutCommit();
            try
            {
                tf.Execute(args);
            }
            catch (Exception)
            {
            }
        }

        [Test]
        public virtual void Test1()
        {
        }
    }
}
