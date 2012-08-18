using System;
using System.Collections;
using System.Collections.Generic;
using NDatabase.Odb;
using NDatabase.Odb.Core.Layers.Layer2.Instance;
using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Odb.Core.Query;
using NDatabase.Odb.Impl.Core.Layers.Layer2.Instance;
using NDatabase.Odb.Impl.Core.Layers.Layer3.Engine;
using NDatabase.Odb.Impl.Core.Query.Criteria;
using NDatabase.Odb.Impl.Core.Query.Values;
using NDatabase.Tool.Wrappers;
using NUnit.Framework;
using Test.Odb.Test;
using Test.Odb.Test.VO.Login;

namespace Query.Values
{
    /// <author>olivier</author>
    [TestFixture]
    public class TestSubList : ODBTest
    {
        private static Parameter GetParameterInstance(IInstanceBuilder instanceBuilder, object nonNativeObjectInfo)
        {
            return (Parameter)instanceBuilder.BuildOneInstance((NonNativeObjectInfo)nonNativeObjectInfo);
        }

        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void Test1()
        {
            DeleteBase("valuesSubList");
            var odb = Open("valuesSubList");
            var handler = new Handler();
            for (var i = 0; i < 10; i++)
                handler.AddParameter(new Parameter("test " + i, "value " + i));
            odb.Store(handler);
            odb.Close();
            odb = Open("valuesSubList");
            var values =
                odb.GetValues(
                    new ValuesCriteriaQuery(typeof (Handler)).Field("parameters").Sublist("parameters", "sub1", 1, 5,
                                                                                          true).Sublist("parameters",
                                                                                                        "sub2", 1, 10).
                        Size("parameters", "size"));

            Println(values);
            var ov = values.NextValues();
            var fulllist = (IList) ov.GetByAlias("parameters");
            AssertEquals(10, fulllist.Count);
            var size = (long) ov.GetByAlias("size");
            AssertEquals(10, size);

            var instanceBuilder = new InstanceBuilder(Dummy.GetEngine(odb));

            var p = GetParameterInstance(instanceBuilder, fulllist[0]);
            AssertEquals("value 0", p.GetValue());
            var p2 = GetParameterInstance(instanceBuilder, fulllist[9]);
            AssertEquals("value 9", p2.GetValue());
            var sublist = (IList) ov.GetByAlias("sub1");
            AssertEquals(5, sublist.Count);
            p = GetParameterInstance(instanceBuilder, sublist[0]);
            AssertEquals("value 1", p.GetValue());
            p2 = GetParameterInstance(instanceBuilder, sublist[4]);
            AssertEquals("value 5", p2.GetValue());
            var sublist2 = (IList) ov.GetByAlias("sub2");
            AssertEquals(9, sublist2.Count);
            odb.Close();
        }

        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void Test11()
        {
            var odb = OdbFactory.Open("valuesSubList");
            var handler = new Handler();
            for (var i = 0; i < 10; i++)
                handler.AddParameter(new Parameter("test " + i, "value " + i));
            odb.Store(handler);
            odb.Close();
            odb = Open("valuesSubList");
            var values =
                odb.GetValues(
                    new ValuesCriteriaQuery(typeof (Handler)).Field("parameters").Sublist("parameters", "sub1", 1, 5,
                                                                                          true).Sublist("parameters",
                                                                                                        "sub2", 1, 10).
                        Size("parameters", "size"));
            var ov = values.NextValues();
            // Retrieve Result values
            var fulllist = (IList) ov.GetByAlias("parameters");
            var size = (long) ov.GetByAlias("size");
            var sublist = (IList) ov.GetByAlias("sub1");

            //TODO: asserts!
            odb.Close();
        }

        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void Test2()
        {
            DeleteBase("valuesSubList2");
            var odb = Open("valuesSubList2");
            var handler = new Handler();
            for (var i = 0; i < 500; i++)
                handler.AddParameter(new Parameter("test " + i, "value " + i));
            var oid = odb.Store(handler);
            odb.Close();
            odb = Open("valuesSubList2");
            var h = (Handler) odb.GetObjectFromId(oid);
            Println("size of list = " + h.GetListOfParameters().Count);
            var start = OdbTime.GetCurrentTimeInMs();
            var values =
                odb.GetValues(
                    new ValuesCriteriaQuery(typeof (Handler)).Sublist("parameters", "sub", 490, 5, true).Size(
                        "parameters", "size"));
            var end = OdbTime.GetCurrentTimeInMs();
            Println("time to load sublist of 5 itens from 40000 : " + (end - start));
            Println(values);
            var ov = values.NextValues();
            var sublist = (IList) ov.GetByAlias("sub");
            AssertEquals(5, sublist.Count);
            var size = (long) ov.GetByAlias("size");
            AssertEquals(500, size);

            var instanceBuilder = new InstanceBuilder(Dummy.GetEngine(odb));

            var p = GetParameterInstance(instanceBuilder, sublist[0]);
            AssertEquals("value 490", p.GetValue());
            var p2 = GetParameterInstance(instanceBuilder, sublist[4]);
            AssertEquals("value 494", p2.GetValue());
            odb.Close();
        }

        /// <summary>
        ///   Using Object representation instead of real object
        /// </summary>
        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void Test3()
        {
            var sublistSize = 10000;
            DeleteBase("valuesSubList3");
            var odb = Open("valuesSubList3");
            var handler = new Handler();
            for (var i = 0; i < sublistSize; i++)
                handler.AddParameter(new Parameter("test " + i, "value " + i));
            odb.Store(handler);
            odb.Close();
            odb = Open("valuesSubList3");
            var start = OdbTime.GetCurrentTimeInMs();
            var q = new ValuesCriteriaQuery(typeof (Handler)).Sublist("parameters", "sub", 9990, 5, true);
            q.SetReturnInstance(false);
            var values = odb.GetValues(q);
            var end = OdbTime.GetCurrentTimeInMs();
            Println("time to load sublist of 5 itens from 40000 : " + (end - start));
            Println(values);
            var ov = values.NextValues();
            var sublist = (IList) ov.GetByAlias("sub");
            AssertEquals(5, sublist.Count);
            var nnoi = (NonNativeObjectInfo) sublist[0];
            AssertEquals("value 9990", nnoi.GetValueOf("value"));
            var nnoi2 = (NonNativeObjectInfo) sublist[4];
            AssertEquals("value 9994", nnoi2.GetValueOf("value"));
            odb.Close();
        }

        [Test]
        public virtual void Test4()
        {
            DeleteBase("sublist4");
            var odb = Open("sublist4");
            var i = 0;
            IList<Function> functions1 = new List<Function>();
            for (i = 0; i < 30; i++)
                functions1.Add(new Function("f1-" + i));
            IList<Function> functions2 = new List<Function>();
            for (i = 0; i < 60; i++)
                functions2.Add(new Function("f2-" + i));
            IList<Function> functions3 = new List<Function>();
            for (i = 0; i < 90; i++)
                functions3.Add(new Function("f3-" + i));
            var user1 = new User("User1", "user1@neodtis.org", new Profile("profile1", functions1));
            var user2 = new User("User2", "user1@neodtis.org", new Profile("profile2", functions2));
            var user3 = new User("User3", "user1@neodtis.org", new Profile("profile3", functions3));
            odb.Store(user1);
            odb.Store(user2);
            odb.Store(user3);
            odb.Close();
            odb = Open("sublist4");
            var u = odb.GetObjects<User>().GetFirst();
            Console.Out.WriteLine(u);
            var q =
                new ValuesCriteriaQuery(typeof (Profile)).Field("name").Sublist("functions", 1, 2, false).Size(
                    "functions", "fsize");
            var v = odb.GetValues(q);
            i = 0;
            while (v.HasNext())
            {
                var ov = v.NextValues();
                var profileName = (string) ov.GetByAlias("name");
                Println(profileName);
                AssertEquals("profile" + (i + 1), profileName);
                AssertEquals(Convert.ToInt64(30 * (i + 1)), ov.GetByAlias("fsize"));
                var l = (IList) ov.GetByAlias("functions");
                Println(l);
                AssertEquals(2, l.Count);
                i++;
            }
            odb.Close();
        }

        /// <summary>
        ///   Using Object representation instead of real object
        /// </summary>
        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void Test5()
        {
            var sublistSize = 400;

            var baseName = GetBaseName();
            DeleteBase(baseName);
            var odb = Open(baseName);
            var handler = new Handler();
            for (var i = 0; i < sublistSize; i++)
                handler.AddParameter(new Parameter("test " + i, "value " + i));
            odb.Store(handler);
            odb.Close();
            odb = Open("valuesSubList3");
            var start = OdbTime.GetCurrentTimeInMs();
            var q = new ValuesCriteriaQuery(typeof (Handler)).Sublist("parameters", "sub", 0, 2, true);
            var values = odb.GetValues(q);
            var end = OdbTime.GetCurrentTimeInMs();
            Println("time to load sublist of 5 itens for " + sublistSize + " : " + (end - start));
            Println(values);
            var ov = values.NextValues();
            var sublist = (IList) ov.GetByAlias("sub");
            AssertEquals(2, sublist.Count);

            var instanceBuilder = new InstanceBuilder(Dummy.GetEngine(odb));

            var parameter = GetParameterInstance(instanceBuilder, sublist[1]);
            AssertEquals("value 1", parameter.GetValue());
            var oid = odb.GetObjectId(parameter);
            Println(oid);
            odb.Close();
        }

        /// <summary>
        ///   Check if objects of list are known by ODB
        /// </summary>
        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void Test6()
        {
            const int sublistSize = 400;

            var baseName = GetBaseName();
            DeleteBase(baseName);
            var odb = Open(baseName);
            var handler = new Handler();
            for (var i = 0; i < sublistSize; i++)
                handler.AddParameter(new Parameter("test " + i, "value " + i));
            odb.Store(handler);
            odb.Close();
            odb = Open("valuesSubList3");
            var start = OdbTime.GetCurrentTimeInMs();
            IQuery q = new CriteriaQuery(typeof (Handler));
            var objects = odb.GetObjects<Handler>(q);
            var end = OdbTime.GetCurrentTimeInMs();

            Console.WriteLine("Query time: {0} ms", end - start);
            var h = objects.GetFirst();
            var parameter = (Parameter) h.GetListOfParameters()[0];
            AssertEquals("value 0", parameter.GetValue());
            var oid = odb.GetObjectId(parameter);
            AssertNotNull(oid);
            odb.Close();
        }
    }
}
