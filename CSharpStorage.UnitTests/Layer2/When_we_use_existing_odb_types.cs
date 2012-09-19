using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NUnit.Framework;

namespace NDatabase.UnitTests.Layer2
{
    public class When_we_use_existing_odb_types
    {
        [Test]
        public void It_should_contain_valid_null_type()
        {
            var odbType = OdbType.Null;

            Assert.That(odbType.BaseClass, Is.Null);
            Assert.That(odbType.Id, Is.EqualTo(OdbType.NullId));
            Assert.That(odbType.Name, Is.EqualTo("null"));
            Assert.That(odbType.Size, Is.EqualTo(1));
            Assert.That(odbType.SubType, Is.Null);
            Assert.That(odbType.GetNativeClass(), Is.Null);
            Assert.That(odbType.HasFixSize(), Is.False);

            Assert.That(odbType.IsArray(), Is.False);
            Assert.That(odbType.IsArrayOrCollection(), Is.False);
            Assert.That(odbType.IsAtomicNative(), Is.False);
            Assert.That(odbType.IsBoolean(), Is.False);
            Assert.That(odbType.IsCollection(), Is.False);
            Assert.That(odbType.IsDate(), Is.False);
            Assert.That(odbType.IsEnum(), Is.False);
            Assert.That(odbType.IsMap(), Is.False);
            Assert.That(odbType.IsNative(), Is.True);
            Assert.That(odbType.IsNonNative(), Is.False);
            Assert.That(odbType.IsNull(), Is.True);
            Assert.That(odbType.IsStringOrBigDecimal(), Is.False);
        }
    }
}