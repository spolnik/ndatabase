using System.IO;
using NDatabase.Odb;
using NDatabase.Odb.Core.Layers.Layer3;
using NDatabase.Odb.Core.Layers.Layer3.IO;
using NDatabase.Odb.Impl.Core.Layers.Layer3.Buffer;
using NUnit.Framework;

namespace NDatabase.UnitTests.Layer3
{
    public class When_we_use_multi_buffered_file_io_with_turned_off_buffer : InstanceSpecification<IMultiBufferedFileIO>
    {
        private const long StartWritePosition = 10L;
        private string _fileName;

        protected override void Establish_context()
        {
            _fileName = "multibufferedfileio.ndb";

            OdbFactory.Delete(_fileName);
        }

        protected override IMultiBufferedFileIO Create_subject_under_test()
        {
            return new MultiBufferedFileIO(_fileName, MultiBuffer.DefaultBufferSizeForData);
        }

        protected override void Because()
        {
            SubjectUnderTest.SetUseBuffer(false);
            SubjectUnderTest.SetCurrentWritePosition(StartWritePosition);
            SubjectUnderTest.WriteBytes(new byte[] { 1, 2, 3, 4, 5 });
        }

        protected override void Dispose_context()
        {
            SubjectUnderTest.Dispose();
        }

        [Test]
        public void It_should_create_or_open_existing_file_with_the_given_name()
        {
            Assert.That(File.Exists(_fileName), Is.True);
        }

        [Test]
        public void It_should_be_able_to_read_all_written_bytes()
        {
            SubjectUnderTest.SetCurrentReadPosition(StartWritePosition);
            Assert.That(SubjectUnderTest.CurrentPosition, Is.EqualTo(StartWritePosition));

            var bytes = SubjectUnderTest.ReadBytes(5);

            Assert.That(bytes[0], Is.EqualTo(1));
            Assert.That(bytes[1], Is.EqualTo(2));
            Assert.That(bytes[2], Is.EqualTo(3));
            Assert.That(bytes[3], Is.EqualTo(4));
            Assert.That(bytes[4], Is.EqualTo(5));
        }

        [Test]
        public void It_should_return_correct_length_of_the_file()
        {
            Assert.That(SubjectUnderTest.Length, Is.EqualTo(StartWritePosition + 5));
        }

        [Test]
        public void It_should_return_current_position_to_the_free_byte_space_after_last_write_position()
        {
            Assert.That(SubjectUnderTest.CurrentPosition, Is.EqualTo(StartWritePosition + 5));
        }

        [Test]
        public void It_should_be_able_to_write_another_byte_and_read_it()
        {
            const byte value = 16;
            SubjectUnderTest.WriteByte(value);

            SubjectUnderTest.SetCurrentReadPosition(StartWritePosition + 5);
            var result = SubjectUnderTest.ReadByte();
            Assert.That(result, Is.EqualTo(value));
        }

        [Test]
        public void It_should_remove_file_after_closing_new_multi_buffered_file_io_with_auto_remove_option_set_to_true()
        {
            const string fileName = "autodelete.ndb";
            using (var multiBufferedFileIo = new MultiBufferedFileIO(fileName, MultiBuffer.DefaultBufferSizeForData))
            {
                multiBufferedFileIo.EnableAutomaticDelete(true);
                Assert.That(File.Exists(fileName), Is.True);
            }

            Assert.That(File.Exists(fileName), Is.False);
        }

        //TODO: check bytes with bigger size than buffer, and than buffer x 5
    }
}