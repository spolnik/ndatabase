using NDatabase.Odb;
using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Odb.Impl.Core.Oid;
using NUnit.Framework;

namespace NDatabase.UnitTests.Serialization.Identification
{
    public class When_we_are_using_db_object_oid : InstanceSpecification<OdbObjectOID>
    {
        private long _oid;
        private long _returnValue;

        protected override void Establish_context()
        {
            _oid = 12345678L;
        }

        protected override OdbObjectOID Create_subject_under_test()
        {
            return new OdbObjectOID(_oid);
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

        [Test]
        public void It_should_return_class_oid_type()
        {
            Assert.That(SubjectUnderTest.GetType(), Is.EqualTo(OdbType.ObjectOidId));
        }

        [Test]
        public void It_should_return_class_oid_type_name()
        {
            Assert.That(SubjectUnderTest.GetTypeName(), Is.EqualTo(OIDTypes.TypeNameObjectOid));
        }

        [Test]
        public void It_should_return_correct_string_representation_of_state()
        {
            var oidStringRepresentation = SubjectUnderTest.OidToString();

            Assert.That(oidStringRepresentation, Is.EqualTo("object-oid:12345678"));
        }

        [Test]
        public void It_should_create_string_which_will_help_to_create_equal_second_instance()
        {
            var oidStringRepresentation = SubjectUnderTest.OidToString();
            var secondInstance = OdbObjectOID.OidFromString(oidStringRepresentation);

            Assert.That(secondInstance, Is.EqualTo(SubjectUnderTest));
            Assert.That(secondInstance.GetHashCode(), Is.EqualTo(SubjectUnderTest.GetHashCode()));
        }
    }
}