using System;
using System.Collections;
using NDatabase2.Odb.Core.Query.Values;
using NUnit.Framework;

namespace Test.NDatabase.Odb.Test.Query.Values
{
    [TestFixture]
    public class TestGetValuesHandlerParameter : ODBTest
    {
        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void Test1()
        {
            DeleteBase("valuesA1");
            var odb = Open("valuesA1");
            var handler = new Handler();
            for (var i = 0; i < 10; i++)
                handler.AddParameter(new Parameter("test " + i, "value" + i));
            odb.Store(handler);
            odb.Close();
            odb = Open("valuesA1");
            var values = odb.GetValues<Handler>(new ValuesCriteriaQuery<Handler>().Field("parameters"));
            Println(values);
            var ov = values.NextValues();
            var l = (IList) ov.GetByAlias("parameters");
            AssertEquals(10, l.Count);
            odb.Close();
        }

        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void Test2()
        {
            DeleteBase("valuesA1");
            var odb = Open("valuesA1");
            var handler = new Handler();
            for (var i = 0; i < 10; i++)
                handler.AddParameter(new Parameter("test " + i, "value" + i));
            odb.Store(handler);
            odb.Close();
            odb = Open("valuesA1");
            // ValuesQuery in getObjects
            try
            {
                var objects = odb.Query<Handler>(new ValuesCriteriaQuery<Handler>().Field("parameters"));
                Fail("Should throw exception");
            }
            catch (Exception)
            {
            }
            // TODO: handle exception
            odb.Close();
        }
    }
}
