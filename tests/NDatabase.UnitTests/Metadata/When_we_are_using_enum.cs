using System;
using Moq;
using NDatabase.Odb;
using NDatabase.Odb.Core;
using NDatabase.Odb.Core.Layers.Layer1.Introspector;
using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Odb.Core.Layers.Layer3;
using NDatabase.Odb.Core.Transaction;
using NDatabase.UnitTests.TestData;
using NUnit.Framework;

namespace NDatabase.UnitTests.Metadata
{
    internal class When_we_are_using_enum : InstanceSpecification<EnumNativeObjectInfo>
    {
        private DayOfWeek _day;

        private Mock<IObjectWriter> _objectWriterMock;
        private Mock<IMetaModel> _metaModelMock;
        private Mock<IOdbCache> _cacheMock;
        private Mock<ISession> _session;

        private SessionDataProvider _classInfoProvider;
        private readonly OID _sampleOid = OIDFactory.BuildObjectOID(1L);

        protected override void Establish_context()
        {
            _day = DayOfWeek.Saturday;

            _objectWriterMock = new Mock<IObjectWriter>();
            _objectWriterMock.Setup(x => x.AddClasses(It.IsAny<ClassInfoList>())).Verifiable();

            _metaModelMock = new Mock<IMetaModel>();
            _metaModelMock.Setup(x => x.ExistClass(typeof(Person))).Returns(false).Verifiable();

            var objectInfoHeader = new ObjectInfoHeader(1, null, null, null, null, null);

            _cacheMock = new Mock<IOdbCache>();
            _cacheMock.Setup(x => x.GetOid(_day)).Returns(_sampleOid).Verifiable();
            _cacheMock.Setup(x => x.GetObjectInfoHeaderByOid(_sampleOid, true)).Returns(objectInfoHeader).Verifiable();

            _session = new Mock<ISession>();
            _session.Setup(x => x.GetMetaModel()).Returns(_metaModelMock.Object).Verifiable();
            _session.Setup(x => x.GetObjectWriter()).Returns(_objectWriterMock.Object).Verifiable();
            _session.Setup(x => x.GetCache()).Returns(_cacheMock.Object).Verifiable();

            _classInfoProvider = new SessionDataProvider(_session.Object);
        }

        protected override EnumNativeObjectInfo Create_subject_under_test()
        {
            var introspector = (IObjectIntrospector)new ObjectIntrospector(_classInfoProvider);
            return
                introspector.GetMetaRepresentation(_day, true, null, new DefaultInstrumentationCallback()) as
                EnumNativeObjectInfo;
        }

        [Test]
        public void It_should_be_represented_by_enum_native_object_info()
        {
            Assert.That(SubjectUnderTest, Is.Not.Null);
            Assert.That(SubjectUnderTest.GetEnumValue(), Is.EqualTo(DayOfWeek.Saturday.ToString()));
            
            throw new NotImplementedException();
        }
    }
}