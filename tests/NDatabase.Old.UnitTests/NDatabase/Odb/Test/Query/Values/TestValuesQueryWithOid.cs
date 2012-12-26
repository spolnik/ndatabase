using System.Collections;
using NDatabase2.Odb.Core.Layers.Layer2.Instance;
using NDatabase2.Odb.Core.Layers.Layer2.Meta;
using NDatabase2.Odb.Core.Query.Values;
using NDatabase2.Odb.Main;
using NUnit.Framework;

namespace Test.NDatabase.Odb.Test.Query.Values
{
    /// <author>olivier</author>
    [TestFixture]
    public class TestValuesQueryWithOid : ODBTest
    {
        private static Parameter GetParameterInstance(IInstanceBuilder instanceBuilder, object nonNativeObjectInfo)
        {
            return (Parameter) instanceBuilder.BuildOneInstance((NonNativeObjectInfo) nonNativeObjectInfo);
        }

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
            var valuesQuery =
                odb.ValuesQuery<Handler>(oid).Field("parameters").Sublist("parameters", "sub1", 1, 5, true).Sublist(
                    "parameters", "sub2", 1, 10).Size("parameters", "size");

            var values = odb.GetValues(valuesQuery);
            Println(values);
            var ov = values.NextValues();
            var fulllist = (IList) ov.GetByAlias("parameters");
            AssertEquals(10, fulllist.Count);
            var size = (long) ov.GetByAlias("size");
            AssertEquals(10, size);

            var instanceBuilder = new InstanceBuilder(((OdbAdapter)odb).GetStorageEngine());

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
    }
}
