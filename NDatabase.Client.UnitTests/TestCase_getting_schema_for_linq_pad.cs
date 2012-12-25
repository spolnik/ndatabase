using NDatabase2.Odb;
using NDatabase2.Odb.Core.Layers.Layer2.Meta;
using NUnit.Framework;

namespace NDatabase.Client.UnitTests
{
    public class TestCase_getting_schema_for_linq_pad
    {
        [Test] 
        public void Test_if_code_could_easily_get_the_schema()
        {
            using (var odb = OdbFactory.Open("working_with_linq.ndb"))
            {
                var schema = odb.Ext().GetSchema();
                Assert.That(schema.GetNumberOfClasses(), Is.EqualTo(3));
            }
        }
    }
}