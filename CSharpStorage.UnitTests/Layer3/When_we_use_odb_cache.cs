using NDatabase.Odb.Core.Layers.Layer1.Introspector;
using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Odb.Core.Layers.Layer3;
using NDatabase.Odb.Core.Layers.Layer3.Engine;
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

            Assert.That(SubjectUnderTest.Contains(_object), Is.True);
        }

        [Test]
        public void It_should_properly_store_object_info_header_in_memory()
        {
            Assert.That(SubjectUnderTest.GetObjectInfoHeaderFromObject(_object), Is.EqualTo(_objectInfo.GetHeader()));
            Assert.That(SubjectUnderTest.GetObjectInfoHeaderFromOid(_oid, false), Is.EqualTo(_objectInfo.GetHeader()));
        }

        [Test]
        public void It_should_have_meaningful_string_representation()
        {
            Assert.That(SubjectUnderTest.ToString(),
                        Is.EqualTo("Cache=1 objects 1 oids 1 object headers 0 positions by oid"));
        }

        [Test]
        public void It_should_allow_on_removing_already_added_object()
        {
            SubjectUnderTest.RemoveObject(_object);

            Assert.That(SubjectUnderTest.GetOid(_object), Is.EqualTo(StorageEngineConstant.NullObjectId));
            Assert.That(SubjectUnderTest.GetObject(_oid), Is.Null);

            Assert.That(SubjectUnderTest.GetObjectInfoHeaderFromObject(_object), Is.Null);
            Assert.That(SubjectUnderTest.GetObjectInfoHeaderFromOid(_oid, false), Is.Null);
        }

        [Test]
        public void It_should_allow_on_cache_cleanup()
        {
            SubjectUnderTest.Clear(false);

            Assert.That(SubjectUnderTest.GetOid(_object), Is.EqualTo(StorageEngineConstant.NullObjectId));
            Assert.That(SubjectUnderTest.GetObject(_oid), Is.Null);

            Assert.That(SubjectUnderTest.GetObjectInfoHeaderFromObject(_object), Is.Null);
            Assert.That(SubjectUnderTest.GetObjectInfoHeaderFromOid(_oid, false), Is.Null);
        }

        [Test]
        public void It_should_show_our_object_as_not_deleted_one()
        {
            Assert.That(SubjectUnderTest.IsDeleted(_oid), Is.False);
        }

        [Test]
        public void It_should_allow_for_marking_object_as_deleted()
        {
            SubjectUnderTest.MarkIdAsDeleted(_oid);

            Assert.That(SubjectUnderTest.IsDeleted(_oid), Is.True);
        }

        [Test]
        public void It_should_allow_us_to_assign_position_to_added_object()
        {
            SubjectUnderTest.SavePositionOfObjectWithOid(_oid, 123L);

            Assert.That(SubjectUnderTest.GetObjectPositionByOid(_oid), Is.EqualTo(123L));
        }

        [Test]
        public void It_should_allow_on_inserting_object_in_more_than_one_step()
        {
            SubjectUnderTest.RemoveObject(_object);
            SubjectUnderTest.StartInsertingObjectWithOid(_object, _oid, _objectInfo);

            Assert.That(SubjectUnderTest.IdOfInsertingObject(_object), Is.EqualTo(_oid));
        }
    }
}
