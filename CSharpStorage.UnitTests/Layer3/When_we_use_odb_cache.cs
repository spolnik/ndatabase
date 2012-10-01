using NDatabase.Odb.Core.Layers.Layer1.Introspector;
using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Odb.Core.Layers.Layer3;
using NDatabase.Odb.Core.Oid;
using NUnit.Framework;

namespace NDatabase.UnitTests.Layer3
{
    public class When_we_use_odb_cache : InstanceSpecification<IOdbCache>
    {
        private class Employee
        {
            public string Name { get; set; }
        }

        private Employee _object;
        private ObjectOID _oid;
        private NonNativeObjectInfo _objectInfo;

        protected override void Establish_context()
        {
            var classIntrospector = new ClassIntrospector();

            _object = new Employee {Name = "Object"};
            _oid = new ObjectOID(1234L);

            var classInfoList = classIntrospector.Introspect(typeof (Employee), true);

            _objectInfo = new NonNativeObjectInfo(_object, classInfoList.GetMainClassInfo());

        }

        protected override IOdbCache Create_subject_under_test()
        {
            return new OdbCache();
        }

        protected override void Because()
        {
            SubjectUnderTest.AddObject(_oid, _object, _objectInfo.GetHeader());
        }

        [Test]
        public void It_should_properly_store_object_with_oid_in_memory()
        {
            Assert.That(SubjectUnderTest.GetOid(_object), Is.EqualTo(_oid));
            Assert.That(SubjectUnderTest.GetObject(_oid), Is.EqualTo(_object));
        }

        [Test]
        public void It_should_properly_store_object_info_header_in_memory()
        {
            Assert.That(SubjectUnderTest.GetObjectInfoHeaderFromObject(_object), Is.EqualTo(_objectInfo.GetHeader()));
            Assert.That(SubjectUnderTest.GetObjectInfoHeaderFromOid(_oid, false), Is.EqualTo(_objectInfo.GetHeader()));
        }

        //TODO: continue with tests
    }
}
