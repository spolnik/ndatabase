using NDatabase.Odb.Core.Layers.Layer3;
using NDatabase.Odb.Core.Layers.Layer3.IO;
using NDatabase.Odb.Impl.Core.Layers.Layer3.Buffer;
using NUnit.Framework;

namespace NDatabase.UnitTests.Layer3
{
    public class When_we_use_multi_buffered_file_io : InstanceSpecification<IMultiBufferedFileIO>
    {
        private const long StartWritePosition = 10L;
        private string _name;
        private string _fileName;

        protected override void Establish_context()
        {
            _name = "Dummy name";
            _fileName = "multibufferedfileio.ndb";
        }

        protected override IMultiBufferedFileIO Create_subject_under_test()
        {
            return new MultiBufferedFileIO(_name, _fileName, MultiBuffer.DefaultBufferSizeForData);
        }

        protected override void Because()
        {
            SubjectUnderTest.SetCurrentWritePosition(StartWritePosition);
            SubjectUnderTest.WriteBytes(new byte[] {1, 2, 3, 4, 5});
        }

        [Test]
        public void It_should_be_able_to_read_all_written_bytes()
        {
            SubjectUnderTest.SetCurrentReadPosition(StartWritePosition);
            var bytes = SubjectUnderTest.ReadBytes(5);

            Assert.That(bytes[0], Is.EqualTo(1));
            Assert.That(bytes[1], Is.EqualTo(2));
            Assert.That(bytes[2], Is.EqualTo(3));
            Assert.That(bytes[3], Is.EqualTo(4));
            Assert.That(bytes[4], Is.EqualTo(5));
        }
    }
}