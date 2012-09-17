using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Odb.Core.Oid;
using NUnit.Framework;

namespace NDatabase.UnitTests.Serialization.Identification
{
    public class When_we_are_using_db_class_oid : InstanceSpecification<OdbClassOID>
    {
        private long _oid;
        private long _returnValue;

        protected override void Establish_context()
        {
            _oid = 12345678L;
        }

        protected override OdbClassOID Create_subject_under_test()
        {
            return new OdbClassOID(_oid);
        }

        protected override void Because()
        {
            _returnValue = SubjectUnderTest.ObjectId;
        }

        [Test]
        public void It_should_contain_object_id_equal_to_inserted_value()
        {
            Assert.That(_returnValue, Is.EqualTo(_oid));
        }
    }
}