using System;
using NDatabase.Odb;
using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Odb.Core.Oid;
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
            
            Assert.That(odbType.IsArray(), Is.False);
            Assert.That(odbType.IsAtomicNative(), Is.False);
            Assert.That(odbType.IsBoolean(), Is.False);
            Assert.That(odbType.IsDate(), Is.False);
            Assert.That(odbType.IsEnum(), Is.False);
            Assert.That(odbType.IsNative(), Is.True);
            Assert.That(odbType.IsNonNative(), Is.False);
            Assert.That(odbType.IsNull(), Is.True);
        }

        [Test]
        public void It_should_contain_valid_boolean_type()
        {
            var odbType = OdbType.Boolean;

            Assert.That(odbType.BaseClass, Is.Null);
            Assert.That(odbType.Id, Is.EqualTo(OdbType.BooleanId));
            Assert.That(odbType.Name, Is.StringStarting("System.Boolean"));
            Assert.That(odbType.Size, Is.EqualTo(1));
            Assert.That(odbType.SubType, Is.Null);
            Assert.That(odbType.GetNativeClass(), Is.EqualTo(typeof(bool)));

            Assert.That(odbType.IsArray(), Is.False);
            Assert.That(odbType.IsAtomicNative(), Is.True);
            Assert.That(odbType.IsBoolean(), Is.True);
            Assert.That(odbType.IsDate(), Is.False);
            Assert.That(odbType.IsEnum(), Is.False);
            Assert.That(odbType.IsNative(), Is.True);
            Assert.That(odbType.IsNonNative(), Is.False);
            Assert.That(odbType.IsNull(), Is.False);
        }

        [Test]
        public void It_should_contain_valid_byte_type()
        {
            var odbType = OdbType.Byte;

            Assert.That(odbType.BaseClass, Is.Null);
            Assert.That(odbType.Id, Is.EqualTo(OdbType.ByteId));
            Assert.That(odbType.Name, Is.StringStarting("System.Byte"));
            Assert.That(odbType.Size, Is.EqualTo(1));
            Assert.That(odbType.SubType, Is.Null);
            Assert.That(odbType.GetNativeClass(), Is.EqualTo(typeof(byte)));

            Assert.That(odbType.IsArray(), Is.False);
            Assert.That(odbType.IsAtomicNative(), Is.True);
            Assert.That(odbType.IsBoolean(), Is.False);
            Assert.That(odbType.IsDate(), Is.False);
            Assert.That(odbType.IsEnum(), Is.False);
            Assert.That(odbType.IsNative(), Is.True);
            Assert.That(odbType.IsNonNative(), Is.False);
            Assert.That(odbType.IsNull(), Is.False);
        }

        [Test]
        public void It_should_contain_valid_short_type()
        {
            var odbType = OdbType.Short;

            Assert.That(odbType.BaseClass, Is.Null);
            Assert.That(odbType.Id, Is.EqualTo(OdbType.ShortId));
            Assert.That(odbType.Name, Is.StringStarting("System.Int16"));
            Assert.That(odbType.Size, Is.EqualTo(2));
            Assert.That(odbType.SubType, Is.Null);
            Assert.That(odbType.GetNativeClass(), Is.EqualTo(typeof(short)));

            Assert.That(odbType.IsArray(), Is.False);
            Assert.That(odbType.IsAtomicNative(), Is.True);
            Assert.That(odbType.IsBoolean(), Is.False);
            Assert.That(odbType.IsDate(), Is.False);
            Assert.That(odbType.IsEnum(), Is.False);
            Assert.That(odbType.IsNative(), Is.True);
            Assert.That(odbType.IsNonNative(), Is.False);
            Assert.That(odbType.IsNull(), Is.False);
        }

        [Test]
        public void It_should_contain_valid_integer_type()
        {
            var odbType = OdbType.Integer;

            Assert.That(odbType.BaseClass, Is.Null);
            Assert.That(odbType.Id, Is.EqualTo(OdbType.IntegerId));
            Assert.That(odbType.Name, Is.StringStarting("System.Int32"));
            Assert.That(odbType.Size, Is.EqualTo(4));
            Assert.That(odbType.SubType, Is.Null);
            Assert.That(odbType.GetNativeClass(), Is.EqualTo(typeof(int)));

            Assert.That(odbType.IsArray(), Is.False);
            Assert.That(odbType.IsAtomicNative(), Is.True);
            Assert.That(odbType.IsBoolean(), Is.False);
            Assert.That(odbType.IsDate(), Is.False);
            Assert.That(odbType.IsEnum(), Is.False);
            Assert.That(odbType.IsNative(), Is.True);
            Assert.That(odbType.IsNonNative(), Is.False);
            Assert.That(odbType.IsNull(), Is.False);
        }

        [Test]
        public void It_should_contain_valid_long_type()
        {
            var odbType = OdbType.Long;

            Assert.That(odbType.BaseClass, Is.Null);
            Assert.That(odbType.Id, Is.EqualTo(OdbType.LongId));
            Assert.That(odbType.Name, Is.StringStarting("System.Int64"));
            Assert.That(odbType.Size, Is.EqualTo(8));
            Assert.That(odbType.SubType, Is.Null);
            Assert.That(odbType.GetNativeClass(), Is.EqualTo(typeof(long)));

            Assert.That(odbType.IsArray(), Is.False);
            Assert.That(odbType.IsAtomicNative(), Is.True);
            Assert.That(odbType.IsBoolean(), Is.False);
            Assert.That(odbType.IsDate(), Is.False);
            Assert.That(odbType.IsEnum(), Is.False);
            Assert.That(odbType.IsNative(), Is.True);
            Assert.That(odbType.IsNonNative(), Is.False);
            Assert.That(odbType.IsNull(), Is.False);
        }

        [Test]
        public void It_should_contain_valid_character_type()
        {
            var odbType = OdbType.Character;

            Assert.That(odbType.BaseClass, Is.Null);
            Assert.That(odbType.Id, Is.EqualTo(OdbType.CharacterId));
            Assert.That(odbType.Name, Is.StringStarting("System.Char"));
            Assert.That(odbType.Size, Is.EqualTo(2));
            Assert.That(odbType.SubType, Is.Null);
            Assert.That(odbType.GetNativeClass(), Is.EqualTo(typeof(char)));

            Assert.That(odbType.IsArray(), Is.False);
            Assert.That(odbType.IsAtomicNative(), Is.True);
            Assert.That(odbType.IsBoolean(), Is.False);
            Assert.That(odbType.IsDate(), Is.False);
            Assert.That(odbType.IsEnum(), Is.False);
            Assert.That(odbType.IsNative(), Is.True);
            Assert.That(odbType.IsNonNative(), Is.False);
            Assert.That(odbType.IsNull(), Is.False);
        }

        [Test]
        public void It_should_contain_valid_decimal_type()
        {
            var odbType = OdbType.Decimal;

            Assert.That(odbType.BaseClass, Is.Null);
            Assert.That(odbType.Id, Is.EqualTo(OdbType.DecimalId));
            Assert.That(odbType.Name, Is.StringStarting("System.Decimal"));
            Assert.That(odbType.Size, Is.EqualTo(16));
            Assert.That(odbType.SubType, Is.Null);
            Assert.That(odbType.GetNativeClass(), Is.EqualTo(typeof(decimal)));

            Assert.That(odbType.IsArray(), Is.False);
            Assert.That(odbType.IsAtomicNative(), Is.True);
            Assert.That(odbType.IsBoolean(), Is.False);
            Assert.That(odbType.IsDate(), Is.False);
            Assert.That(odbType.IsEnum(), Is.False);
            Assert.That(odbType.IsNative(), Is.True);
            Assert.That(odbType.IsNonNative(), Is.False);
            Assert.That(odbType.IsNull(), Is.False);
        }

        [Test]
        public void It_should_contain_valid_float_type()
        {
            var odbType = OdbType.Float;

            Assert.That(odbType.BaseClass, Is.Null);
            Assert.That(odbType.Id, Is.EqualTo(OdbType.FloatId));
            Assert.That(odbType.Name, Is.StringStarting("System.Single"));
            Assert.That(odbType.Size, Is.EqualTo(4));
            Assert.That(odbType.SubType, Is.Null);
            Assert.That(odbType.GetNativeClass(), Is.EqualTo(typeof(float)));

            Assert.That(odbType.IsArray(), Is.False);
            Assert.That(odbType.IsAtomicNative(), Is.True);
            Assert.That(odbType.IsBoolean(), Is.False);
            Assert.That(odbType.IsDate(), Is.False);
            Assert.That(odbType.IsEnum(), Is.False);
            Assert.That(odbType.IsNative(), Is.True);
            Assert.That(odbType.IsNonNative(), Is.False);
            Assert.That(odbType.IsNull(), Is.False);
        }

        [Test]
        public void It_should_contain_valid_double_type()
        {
            var odbType = OdbType.Double;

            Assert.That(odbType.BaseClass, Is.Null);
            Assert.That(odbType.Id, Is.EqualTo(OdbType.DoubleId));
            Assert.That(odbType.Name, Is.StringStarting("System.Double"));
            Assert.That(odbType.Size, Is.EqualTo(8));
            Assert.That(odbType.SubType, Is.Null);
            Assert.That(odbType.GetNativeClass(), Is.EqualTo(typeof(double)));

            Assert.That(odbType.IsArray(), Is.False);
            Assert.That(odbType.IsAtomicNative(), Is.True);
            Assert.That(odbType.IsBoolean(), Is.False);
            Assert.That(odbType.IsDate(), Is.False);
            Assert.That(odbType.IsEnum(), Is.False);
            Assert.That(odbType.IsNative(), Is.True);
            Assert.That(odbType.IsNonNative(), Is.False);
            Assert.That(odbType.IsNull(), Is.False);
        }

        [Test]
        public void It_should_contain_valid_date_type()
        {
            var odbType = OdbType.Date;

            Assert.That(odbType.BaseClass, Is.Null);
            Assert.That(odbType.Id, Is.EqualTo(OdbType.DateId));
            Assert.That(odbType.Name, Is.StringStarting("System.DateTime"));
            Assert.That(odbType.Size, Is.EqualTo(8));
            Assert.That(odbType.SubType, Is.Null);
            Assert.That(odbType.GetNativeClass(), Is.EqualTo(typeof(DateTime)));

            Assert.That(odbType.IsArray(), Is.False);
            Assert.That(odbType.IsAtomicNative(), Is.True);
            Assert.That(odbType.IsBoolean(), Is.False);
            Assert.That(odbType.IsDate(), Is.True);
            Assert.That(odbType.IsEnum(), Is.False);
            Assert.That(odbType.IsNative(), Is.True);
            Assert.That(odbType.IsNonNative(), Is.False);
            Assert.That(odbType.IsNull(), Is.False);
        }

        [Test]
        public void It_should_contain_valid_enum_type()
        {
            var odbType = OdbType.Enum;

            Assert.That(odbType.BaseClass, Is.Null);
            Assert.That(odbType.Id, Is.EqualTo(OdbType.EnumId));
            Assert.That(odbType.Name, Is.StringStarting("System.Enum"));
            Assert.That(odbType.Size, Is.EqualTo(1));
            Assert.That(odbType.SubType, Is.Null);
            Assert.That(odbType.GetNativeClass(), Is.EqualTo(typeof(Enum)));

            Assert.That(odbType.IsArray(), Is.False);
            Assert.That(odbType.IsAtomicNative(), Is.False);
            Assert.That(odbType.IsBoolean(), Is.False);
            Assert.That(odbType.IsDate(), Is.False);
            Assert.That(odbType.IsEnum(), Is.True);
            Assert.That(odbType.IsNative(), Is.True);
            Assert.That(odbType.IsNonNative(), Is.False);
            Assert.That(odbType.IsNull(), Is.False);
        }

        [Test]
        public void It_should_contain_valid_oid_type()
        {
            var odbType = OdbType.Oid;

            Assert.That(odbType.BaseClass, Is.EqualTo(typeof(OID)));
            Assert.That(odbType.Id, Is.EqualTo(OdbType.OidId));
            Assert.That(odbType.Name, Is.StringStarting("NDatabase.Odb.OID"));
            Assert.That(odbType.Size, Is.EqualTo(0));
            Assert.That(odbType.SubType, Is.Null);
            Assert.That(odbType.GetNativeClass(), Is.EqualTo(typeof(OID)));

            Assert.That(odbType.IsArray(), Is.False);
            Assert.That(odbType.IsAtomicNative(), Is.True);
            Assert.That(odbType.IsBoolean(), Is.False);
            Assert.That(odbType.IsDate(), Is.False);
            Assert.That(odbType.IsEnum(), Is.False);
            Assert.That(odbType.IsNative(), Is.True);
            Assert.That(odbType.IsNonNative(), Is.False);
            Assert.That(odbType.IsNull(), Is.False);
        }

        [Test]
        public void It_should_contain_valid_object_oid_type()
        {
            var odbType = OdbType.ObjectOid;

            Assert.That(odbType.BaseClass, Is.EqualTo(typeof(ObjectOID)));
            Assert.That(odbType.Id, Is.EqualTo(OdbType.ObjectOidId));
            Assert.That(odbType.Name, Is.StringStarting("NDatabase.Odb.Core.Oid.ObjectOID"));
            Assert.That(odbType.Size, Is.EqualTo(0));
            Assert.That(odbType.SubType, Is.Null);
            Assert.That(odbType.GetNativeClass(), Is.EqualTo(typeof(ObjectOID)));

            Assert.That(odbType.IsArray(), Is.False);
            Assert.That(odbType.IsAtomicNative(), Is.True);
            Assert.That(odbType.IsBoolean(), Is.False);
            Assert.That(odbType.IsDate(), Is.False);
            Assert.That(odbType.IsEnum(), Is.False);
            Assert.That(odbType.IsNative(), Is.True);
            Assert.That(odbType.IsNonNative(), Is.False);
            Assert.That(odbType.IsNull(), Is.False);
        }

        [Test]
        public void It_should_contain_valid_class_oid_type()
        {
            var odbType = OdbType.ClassOid;

            Assert.That(odbType.BaseClass, Is.EqualTo(typeof(ClassOID)));
            Assert.That(odbType.Id, Is.EqualTo(OdbType.ClassOidId));
            Assert.That(odbType.Name, Is.StringStarting("NDatabase.Odb.Core.Oid.ClassOID"));
            Assert.That(odbType.Size, Is.EqualTo(0));
            Assert.That(odbType.SubType, Is.Null);
            Assert.That(odbType.GetNativeClass(), Is.EqualTo(typeof(ClassOID)));

            Assert.That(odbType.IsArray(), Is.False);
            Assert.That(odbType.IsAtomicNative(), Is.True);
            Assert.That(odbType.IsBoolean(), Is.False);
            Assert.That(odbType.IsDate(), Is.False);
            Assert.That(odbType.IsEnum(), Is.False);
            Assert.That(odbType.IsNative(), Is.True);
            Assert.That(odbType.IsNonNative(), Is.False);
            Assert.That(odbType.IsNull(), Is.False);
        }

        [Test]
        public void It_should_contain_valid_string_type()
        {
            var odbType = OdbType.String;

            Assert.That(odbType.BaseClass, Is.Null);
            Assert.That(odbType.Id, Is.EqualTo(OdbType.StringId));
            Assert.That(odbType.Name, Is.StringStarting("System.String"));
            Assert.That(odbType.Size, Is.EqualTo(1));
            Assert.That(odbType.SubType, Is.Null);
            Assert.That(odbType.GetNativeClass(), Is.EqualTo(typeof(String)));

            Assert.That(odbType.IsArray(), Is.False);
            Assert.That(odbType.IsAtomicNative(), Is.True);
            Assert.That(odbType.IsBoolean(), Is.False);
            Assert.That(odbType.IsDate(), Is.False);
            Assert.That(odbType.IsEnum(), Is.False);
            Assert.That(odbType.IsNative(), Is.True);
            Assert.That(odbType.IsNonNative(), Is.False);
            Assert.That(odbType.IsNull(), Is.False);
        }

        [Test]
        public void It_should_contain_valid_array_type()
        {
            var odbType = OdbType.Array;

            Assert.That(odbType.BaseClass, Is.Null);
            Assert.That(odbType.Id, Is.EqualTo(OdbType.ArrayId));
            Assert.That(odbType.Name, Is.EqualTo("array"));
            Assert.That(odbType.Size, Is.EqualTo(0));
            Assert.That(odbType.SubType, Is.Null);
            Assert.That(odbType.GetNativeClass(), Is.Null);

            Assert.That(odbType.IsArray(), Is.True);
            Assert.That(odbType.IsAtomicNative(), Is.False);
            Assert.That(odbType.IsBoolean(), Is.False);
            Assert.That(odbType.IsDate(), Is.False);
            Assert.That(odbType.IsEnum(), Is.False);
            Assert.That(odbType.IsNative(), Is.True);
            Assert.That(odbType.IsNonNative(), Is.False);
            Assert.That(odbType.IsNull(), Is.False);
        }
        
        [Test]
        public void It_should_contain_valid_non_native_type()
        {
            var odbType = OdbType.NonNative;

            Assert.That(odbType.BaseClass, Is.Null);
            Assert.That(odbType.Id, Is.EqualTo(OdbType.NonNativeId));
            Assert.That(odbType.Name, Is.EqualTo("non native"));
            Assert.That(odbType.Size, Is.EqualTo(0));
            Assert.That(odbType.SubType, Is.Null);
            Assert.That(odbType.GetNativeClass(), Is.Null);

            Assert.That(odbType.IsArray(), Is.False);
            Assert.That(odbType.IsAtomicNative(), Is.False);
            Assert.That(odbType.IsBoolean(), Is.False);
            Assert.That(odbType.IsDate(), Is.False);
            Assert.That(odbType.IsEnum(), Is.False);
            Assert.That(odbType.IsNative(), Is.False);
            Assert.That(odbType.IsNonNative(), Is.True);
            Assert.That(odbType.IsNull(), Is.False);
        }
    }
}