using NDatabase2.Odb;
using NUnit.Framework;

namespace NDatabase.UnitTests.Bugs
{
    public class UnsignedTypesCheck
    {
        public uint ValUInt { get; set; }
        public ulong ValULong { get; set; }
        public ushort ValUShort { get; set; }
        public sbyte ValSByte { get; set; }
    }

    public class When_storing_unsigned_int_long_short_or_signed_byte
    {
        [Test]
        public void It_should_be_stored_without_any_problem()
        {
            OdbFactory.Delete("unsigned.ndb");

            var testObject = new UnsignedTypesCheck
                                 {
                                     ValUInt = uint.MaxValue,
                                     ValULong = ulong.MaxValue,
                                     ValUShort = ushort.MaxValue,
                                     ValSByte = sbyte.MinValue
                                 };

            using (var odb = OdbFactory.Open("unsigned.ndb"))
            {
                odb.Store(testObject);
            }

            using (var odb = OdbFactory.OpenLast())
            {
                var objectToCheck = odb.Query<UnsignedTypesCheck>().GetFirst();

                Assert.That(objectToCheck.ValUInt, Is.EqualTo(uint.MaxValue));
                Assert.That(objectToCheck.ValULong, Is.EqualTo(ulong.MaxValue));
                Assert.That(objectToCheck.ValUShort, Is.EqualTo(ushort.MaxValue));
                Assert.That(objectToCheck.ValSByte, Is.EqualTo(sbyte.MinValue));
            }
        }
    }
}