using System.Collections;
using NDatabase.Odb.Impl.Core.Query.Values;
using NUnit.Framework;
using Test.Odb.Test;

namespace Query.Values
{
    /// <author>olivier</author>
    [TestFixture]
    public class TestValuesQueryWithOid : ODBTest
    {
        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void Test1()
        {
            var baseName = GetBaseName();
            DeleteBase(baseName);
            var odb = Open(baseName);
            var handler = new Handler();
            for (var i = 0; i < 10; i++)
                handler.AddParameter(new Parameter("test " + i, "value " + i));
            var oid = odb.Store(handler);
            odb.Close();
            odb = Open(baseName);
            var values =
                odb.GetValues(
                    new ValuesCriteriaQuery(typeof (Handler), oid).Field("parameters").Sublist("parameters", "sub1", 1,
                                                                                               5, true).Sublist("parameters", "sub2",
                                                                                                   1, 10).Size("parameters", "size"));
            Println(values);
            var ov = values.NextValues();
            var fulllist = (IList) ov.GetByAlias("parameters");
            AssertEquals(10, fulllist.Count);
            var size = (long) ov.GetByAlias("size");
            AssertEquals(10, size);
            var p = (Parameter) fulllist[0];
            AssertEquals("value 0", p.GetValue());
            var p2 = (Parameter) fulllist[9];
            AssertEquals("value 9", p2.GetValue());
            var sublist = (IList) ov.GetByAlias("sub1");
            AssertEquals(5, sublist.Count);
            p = (Parameter) sublist[0];
            AssertEquals("value 1", p.GetValue());
            p2 = (Parameter) sublist[4];
            AssertEquals("value 5", p2.GetValue());
            var sublist2 = (IList) ov.GetByAlias("sub2");
            AssertEquals(9, sublist2.Count);
            odb.Close();
        }
    }
}
