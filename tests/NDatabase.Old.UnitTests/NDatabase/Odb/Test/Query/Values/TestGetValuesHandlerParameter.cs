using System;
using System.Collections;
using NUnit.Framework;

namespace Test.NDatabase.Odb.Test.Query.Values
{
    public class TestGetValuesHandlerParameter : ODBTest
    {
        [Test]
        public virtual void Test1()
        {
            DeleteBase("valuesA1");
            using (var odb = Open("valuesA1"))
            {
                var handler = new Handler();

                for (var i = 0; i < 10; i++)
                    handler.AddParameter(new Parameter("test " + i, "value" + i));

                odb.Store(handler);
            }

            using (var odb = Open("valuesA1"))
            {
                var values = odb.ValuesQuery<Handler>().Field("parameters").Execute();
                Println(values);
                var ov = values.NextValues();
                var l = (IList) ov.GetByAlias("parameters");
                AssertEquals(10, l.Count);
            }
        }

        [Test]
        public void Test2()
        {
            DeleteBase("valuesA1");
            var odb = Open("valuesA1");
            var handler = new Handler();

            for (var i = 0; i < 10; i++)
                handler.AddParameter(new Parameter("test " + i, "value" + i));

            odb.Store(handler);
            odb.Close();
            odb = Open("valuesA1");
            
            try
            {
                odb.ValuesQuery<Handler>().Field("parameters").Execute<Handler>();
                Fail("Should throw exception");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Assert.Pass();
            }
            
            odb.Close();
        }
    }
}
