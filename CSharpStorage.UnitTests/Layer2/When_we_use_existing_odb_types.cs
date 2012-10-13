using System;
using System.Collections;
using System.Collections.Generic;
using NDatabase.Odb;
using NDatabase2.Odb;
using NDatabase2.Odb.Core.Layers.Layer2.Meta;
using NDatabase2.Odb.Core.Oid;
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

        [Test]
        public void It_should_contain_valid_boolean_type()
        {
            var odbType = OdbType.Boolean;

            Assert.That(odbType.BaseClass, Is.Null);
            Assert.That(odbType.Id, Is.EqualTo(OdbType.BooleanId));
            Assert.That(odbType.Name, Is.StringStarting("System.Boolean,mscorlib"));
            Assert.That(odbType.Size, Is.EqualTo(1));
            Assert.That(odbType.SubType, Is.Null);
            Assert.That(odbType.GetNativeClass(), Is.EqualTo(typeof(bool)));
            Assert.That(odbType.HasFixSize(), Is.True);

            Assert.That(odbType.IsArray(), Is.False);
            Assert.That(odbType.IsArrayOrCollection(), Is.False);
            Assert.That(odbType.IsAtomicNative(), Is.True);
            Assert.That(odbType.IsBoolean(), Is.True);
            Assert.That(odbType.IsCollection(), Is.False);
            Assert.That(odbType.IsDate(), Is.False);
            Assert.That(odbType.IsEnum(), Is.False);
            Assert.That(odbType.IsMap(), Is.False);
            Assert.That(odbType.IsNative(), Is.True);
            Assert.That(odbType.IsNonNative(), Is.False);
            Assert.That(odbType.IsNull(), Is.False);
            Assert.That(odbType.IsStringOrBigDecimal(), Is.False);
        }

        [Test]
        public void It_should_contain_valid_byte_type()
        {
            var odbType = OdbType.Byte;

            Assert.That(odbType.BaseClass, Is.Null);
            Assert.That(odbType.Id, Is.EqualTo(OdbType.ByteId));
            Assert.That(odbType.Name, Is.StringStarting("System.Byte,mscorlib"));
            Assert.That(odbType.Size, Is.EqualTo(1));
            Assert.That(odbType.SubType, Is.Null);
            Assert.That(odbType.GetNativeClass(), Is.EqualTo(typeof(byte)));
            Assert.That(odbType.HasFixSize(), Is.True);

            Assert.That(odbType.IsArray(), Is.False);
            Assert.That(odbType.IsArrayOrCollection(), Is.False);
            Assert.That(odbType.IsAtomicNative(), Is.True);
            Assert.That(odbType.IsBoolean(), Is.False);
            Assert.That(odbType.IsCollection(), Is.False);
            Assert.That(odbType.IsDate(), Is.False);
            Assert.That(odbType.IsEnum(), Is.False);
            Assert.That(odbType.IsMap(), Is.False);
            Assert.That(odbType.IsNative(), Is.True);
            Assert.That(odbType.IsNonNative(), Is.False);
            Assert.That(odbType.IsNull(), Is.False);
            Assert.That(odbType.IsStringOrBigDecimal(), Is.False);
        }

        [Test]
        public void It_should_contain_valid_short_type()
        {
            var odbType = OdbType.Short;

            Assert.That(odbType.BaseClass, Is.Null);
            Assert.That(odbType.Id, Is.EqualTo(OdbType.ShortId));
            Assert.That(odbType.Name, Is.StringStarting("System.Int16,mscorlib"));
            Assert.That(odbType.Size, Is.EqualTo(2));
            Assert.That(odbType.SubType, Is.Null);
            Assert.That(odbType.GetNativeClass(), Is.EqualTo(typeof(short)));
            Assert.That(odbType.HasFixSize(), Is.True);

            Assert.That(odbType.IsArray(), Is.False);
            Assert.That(odbType.IsArrayOrCollection(), Is.False);
            Assert.That(odbType.IsAtomicNative(), Is.True);
            Assert.That(odbType.IsBoolean(), Is.False);
            Assert.That(odbType.IsCollection(), Is.False);
            Assert.That(odbType.IsDate(), Is.False);
            Assert.That(odbType.IsEnum(), Is.False);
            Assert.That(odbType.IsMap(), Is.False);
            Assert.That(odbType.IsNative(), Is.True);
            Assert.That(odbType.IsNonNative(), Is.False);
            Assert.That(odbType.IsNull(), Is.False);
            Assert.That(odbType.IsStringOrBigDecimal(), Is.False);
        }

        [Test]
        public void It_should_contain_valid_integer_type()
        {
            var odbType = OdbType.Integer;

            Assert.That(odbType.BaseClass, Is.Null);
            Assert.That(odbType.Id, Is.EqualTo(OdbType.IntegerId));
            Assert.That(odbType.Name, Is.StringStarting("System.Int32,mscorlib"));
            Assert.That(odbType.Size, Is.EqualTo(4));
            Assert.That(odbType.SubType, Is.Null);
            Assert.That(odbType.GetNativeClass(), Is.EqualTo(typeof(int)));
            Assert.That(odbType.HasFixSize(), Is.True);

            Assert.That(odbType.IsArray(), Is.False);
            Assert.That(odbType.IsArrayOrCollection(), Is.False);
            Assert.That(odbType.IsAtomicNative(), Is.True);
            Assert.That(odbType.IsBoolean(), Is.False);
            Assert.That(odbType.IsCollection(), Is.False);
            Assert.That(odbType.IsDate(), Is.False);
            Assert.That(odbType.IsEnum(), Is.False);
            Assert.That(odbType.IsMap(), Is.False);
            Assert.That(odbType.IsNative(), Is.True);
            Assert.That(odbType.IsNonNative(), Is.False);
            Assert.That(odbType.IsNull(), Is.False);
            Assert.That(odbType.IsStringOrBigDecimal(), Is.False);
        }

        [Test]
        public void It_should_contain_valid_long_type()
        {
            var odbType = OdbType.Long;

            Assert.That(odbType.BaseClass, Is.Null);
            Assert.That(odbType.Id, Is.EqualTo(OdbType.LongId));
            Assert.That(odbType.Name, Is.StringStarting("System.Int64,mscorlib"));
            Assert.That(odbType.Size, Is.EqualTo(8));
            Assert.That(odbType.SubType, Is.Null);
            Assert.That(odbType.GetNativeClass(), Is.EqualTo(typeof(long)));
            Assert.That(odbType.HasFixSize(), Is.True);

            Assert.That(odbType.IsArray(), Is.False);
            Assert.That(odbType.IsArrayOrCollection(), Is.False);
            Assert.That(odbType.IsAtomicNative(), Is.True);
            Assert.That(odbType.IsBoolean(), Is.False);
            Assert.That(odbType.IsCollection(), Is.False);
            Assert.That(odbType.IsDate(), Is.False);
            Assert.That(odbType.IsEnum(), Is.False);
            Assert.That(odbType.IsMap(), Is.False);
            Assert.That(odbType.IsNative(), Is.True);
            Assert.That(odbType.IsNonNative(), Is.False);
            Assert.That(odbType.IsNull(), Is.False);
            Assert.That(odbType.IsStringOrBigDecimal(), Is.False);
        }

        [Test]
        public void It_should_contain_valid_character_type()
        {
            var odbType = OdbType.Character;

            Assert.That(odbType.BaseClass, Is.Null);
            Assert.That(odbType.Id, Is.EqualTo(OdbType.CharacterId));
            Assert.That(odbType.Name, Is.StringStarting("System.Char,mscorlib"));
            Assert.That(odbType.Size, Is.EqualTo(2));
            Assert.That(odbType.SubType, Is.Null);
            Assert.That(odbType.GetNativeClass(), Is.EqualTo(typeof(char)));
            Assert.That(odbType.HasFixSize(), Is.True);

            Assert.That(odbType.IsArray(), Is.False);
            Assert.That(odbType.IsArrayOrCollection(), Is.False);
            Assert.That(odbType.IsAtomicNative(), Is.True);
            Assert.That(odbType.IsBoolean(), Is.False);
            Assert.That(odbType.IsCollection(), Is.False);
            Assert.That(odbType.IsDate(), Is.False);
            Assert.That(odbType.IsEnum(), Is.False);
            Assert.That(odbType.IsMap(), Is.False);
            Assert.That(odbType.IsNative(), Is.True);
            Assert.That(odbType.IsNonNative(), Is.False);
            Assert.That(odbType.IsNull(), Is.False);
            Assert.That(odbType.IsStringOrBigDecimal(), Is.False);
        }

        [Test]
        public void It_should_contain_valid_decimal_type()
        {
            var odbType = OdbType.Decimal;

            Assert.That(odbType.BaseClass, Is.Null);
            Assert.That(odbType.Id, Is.EqualTo(OdbType.DecimalId));
            Assert.That(odbType.Name, Is.StringStarting("System.Decimal,mscorlib"));
            Assert.That(odbType.Size, Is.EqualTo(16));
            Assert.That(odbType.SubType, Is.Null);
            Assert.That(odbType.GetNativeClass(), Is.EqualTo(typeof(decimal)));
            Assert.That(odbType.HasFixSize(), Is.True);

            Assert.That(odbType.IsArray(), Is.False);
            Assert.That(odbType.IsArrayOrCollection(), Is.False);
            Assert.That(odbType.IsAtomicNative(), Is.True);
            Assert.That(odbType.IsBoolean(), Is.False);
            Assert.That(odbType.IsCollection(), Is.False);
            Assert.That(odbType.IsDate(), Is.False);
            Assert.That(odbType.IsEnum(), Is.False);
            Assert.That(odbType.IsMap(), Is.False);
            Assert.That(odbType.IsNative(), Is.True);
            Assert.That(odbType.IsNonNative(), Is.False);
            Assert.That(odbType.IsNull(), Is.False);
            Assert.That(odbType.IsStringOrBigDecimal(), Is.True);
        }

        [Test]
        public void It_should_contain_valid_float_type()
        {
            var odbType = OdbType.Float;

            Assert.That(odbType.BaseClass, Is.Null);
            Assert.That(odbType.Id, Is.EqualTo(OdbType.FloatId));
            Assert.That(odbType.Name, Is.StringStarting("System.Single,mscorlib"));
            Assert.That(odbType.Size, Is.EqualTo(4));
            Assert.That(odbType.SubType, Is.Null);
            Assert.That(odbType.GetNativeClass(), Is.EqualTo(typeof(float)));
            Assert.That(odbType.HasFixSize(), Is.True);

            Assert.That(odbType.IsArray(), Is.False);
            Assert.That(odbType.IsArrayOrCollection(), Is.False);
            Assert.That(odbType.IsAtomicNative(), Is.True);
            Assert.That(odbType.IsBoolean(), Is.False);
            Assert.That(odbType.IsCollection(), Is.False);
            Assert.That(odbType.IsDate(), Is.False);
            Assert.That(odbType.IsEnum(), Is.False);
            Assert.That(odbType.IsMap(), Is.False);
            Assert.That(odbType.IsNative(), Is.True);
            Assert.That(odbType.IsNonNative(), Is.False);
            Assert.That(odbType.IsNull(), Is.False);
            Assert.That(odbType.IsStringOrBigDecimal(), Is.False);
        }

        [Test]
        public void It_should_contain_valid_double_type()
        {
            var odbType = OdbType.Double;

            Assert.That(odbType.BaseClass, Is.Null);
            Assert.That(odbType.Id, Is.EqualTo(OdbType.DoubleId));
            Assert.That(odbType.Name, Is.StringStarting("System.Double,mscorlib"));
            Assert.That(odbType.Size, Is.EqualTo(8));
            Assert.That(odbType.SubType, Is.Null);
            Assert.That(odbType.GetNativeClass(), Is.EqualTo(typeof(double)));
            Assert.That(odbType.HasFixSize(), Is.True);

            Assert.That(odbType.IsArray(), Is.False);
            Assert.That(odbType.IsArrayOrCollection(), Is.False);
            Assert.That(odbType.IsAtomicNative(), Is.True);
            Assert.That(odbType.IsBoolean(), Is.False);
            Assert.That(odbType.IsCollection(), Is.False);
            Assert.That(odbType.IsDate(), Is.False);
            Assert.That(odbType.IsEnum(), Is.False);
            Assert.That(odbType.IsMap(), Is.False);
            Assert.That(odbType.IsNative(), Is.True);
            Assert.That(odbType.IsNonNative(), Is.False);
            Assert.That(odbType.IsNull(), Is.False);
            Assert.That(odbType.IsStringOrBigDecimal(), Is.False);
        }

        [Test]
        public void It_should_contain_valid_date_type()
        {
            var odbType = OdbType.Date;

            Assert.That(odbType.BaseClass, Is.Null);
            Assert.That(odbType.Id, Is.EqualTo(OdbType.DateId));
            Assert.That(odbType.Name, Is.StringStarting("System.DateTime,mscorlib"));
            Assert.That(odbType.Size, Is.EqualTo(8));
            Assert.That(odbType.SubType, Is.Null);
            Assert.That(odbType.GetNativeClass(), Is.EqualTo(typeof(DateTime)));
            Assert.That(odbType.HasFixSize(), Is.True);

            Assert.That(odbType.IsArray(), Is.False);
            Assert.That(odbType.IsArrayOrCollection(), Is.False);
            Assert.That(odbType.IsAtomicNative(), Is.True);
            Assert.That(odbType.IsBoolean(), Is.False);
            Assert.That(odbType.IsCollection(), Is.False);
            Assert.That(odbType.IsDate(), Is.True);
            Assert.That(odbType.IsEnum(), Is.False);
            Assert.That(odbType.IsMap(), Is.False);
            Assert.That(odbType.IsNative(), Is.True);
            Assert.That(odbType.IsNonNative(), Is.False);
            Assert.That(odbType.IsNull(), Is.False);
            Assert.That(odbType.IsStringOrBigDecimal(), Is.False);
        }

        //TODO: should be native and with fixed size
        [Test]
        public void It_should_contain_valid_enum_type()
        {
            var odbType = OdbType.Enum;

            Assert.That(odbType.BaseClass, Is.Null);
            Assert.That(odbType.Id, Is.EqualTo(OdbType.EnumId));
            Assert.That(odbType.Name, Is.StringStarting("System.Enum,mscorlib"));
            Assert.That(odbType.Size, Is.EqualTo(1));
            Assert.That(odbType.SubType, Is.Null);
            Assert.That(odbType.GetNativeClass(), Is.EqualTo(typeof(Enum)));
            Assert.That(odbType.HasFixSize(), Is.False);

            Assert.That(odbType.IsArray(), Is.False);
            Assert.That(odbType.IsArrayOrCollection(), Is.False);
            Assert.That(odbType.IsAtomicNative(), Is.False);
            Assert.That(odbType.IsBoolean(), Is.False);
            Assert.That(odbType.IsCollection(), Is.False);
            Assert.That(odbType.IsDate(), Is.False);
            Assert.That(odbType.IsEnum(), Is.True);
            Assert.That(odbType.IsMap(), Is.False);
            Assert.That(odbType.IsNative(), Is.True);
            Assert.That(odbType.IsNonNative(), Is.False);
            Assert.That(odbType.IsNull(), Is.False);
            Assert.That(odbType.IsStringOrBigDecimal(), Is.False);
        }

        [Test]
        public void It_should_contain_valid_oid_type()
        {
            var odbType = OdbType.Oid;

            Assert.That(odbType.BaseClass, Is.EqualTo(typeof(OID)));
            Assert.That(odbType.Id, Is.EqualTo(OdbType.OidId));
            Assert.That(odbType.Name, Is.EqualTo("NDatabase2.Odb.OID,NDatabase"));
            Assert.That(odbType.Size, Is.EqualTo(0));
            Assert.That(odbType.SubType, Is.Null);
            Assert.That(odbType.GetNativeClass(), Is.EqualTo(typeof(OID)));
            Assert.That(odbType.HasFixSize(), Is.True); //TODO: is that correct?

            Assert.That(odbType.IsArray(), Is.False);
            Assert.That(odbType.IsArrayOrCollection(), Is.False);
            Assert.That(odbType.IsAtomicNative(), Is.True);
            Assert.That(odbType.IsBoolean(), Is.False);
            Assert.That(odbType.IsCollection(), Is.False);
            Assert.That(odbType.IsDate(), Is.False);
            Assert.That(odbType.IsEnum(), Is.False);
            Assert.That(odbType.IsMap(), Is.False);
            Assert.That(odbType.IsNative(), Is.True);
            Assert.That(odbType.IsNonNative(), Is.False);
            Assert.That(odbType.IsNull(), Is.False);
            Assert.That(odbType.IsStringOrBigDecimal(), Is.False);
        }

        [Test]
        public void It_should_contain_valid_object_oid_type()
        {
            var odbType = OdbType.ObjectOid;

            Assert.That(odbType.BaseClass, Is.EqualTo(typeof(ObjectOID)));
            Assert.That(odbType.Id, Is.EqualTo(OdbType.ObjectOidId));
            Assert.That(odbType.Name, Is.EqualTo("NDatabase2.Odb.Core.Oid.ObjectOID,NDatabase"));
            Assert.That(odbType.Size, Is.EqualTo(0));
            Assert.That(odbType.SubType, Is.Null);
            Assert.That(odbType.GetNativeClass(), Is.EqualTo(typeof(ObjectOID)));
            Assert.That(odbType.HasFixSize(), Is.True); //TODO: is that correct?

            Assert.That(odbType.IsArray(), Is.False);
            Assert.That(odbType.IsArrayOrCollection(), Is.False);
            Assert.That(odbType.IsAtomicNative(), Is.True);
            Assert.That(odbType.IsBoolean(), Is.False);
            Assert.That(odbType.IsCollection(), Is.False);
            Assert.That(odbType.IsDate(), Is.False);
            Assert.That(odbType.IsEnum(), Is.False);
            Assert.That(odbType.IsMap(), Is.False);
            Assert.That(odbType.IsNative(), Is.True);
            Assert.That(odbType.IsNonNative(), Is.False);
            Assert.That(odbType.IsNull(), Is.False);
            Assert.That(odbType.IsStringOrBigDecimal(), Is.False);
        }

        [Test]
        public void It_should_contain_valid_class_oid_type()
        {
            var odbType = OdbType.ClassOid;

            Assert.That(odbType.BaseClass, Is.EqualTo(typeof(ClassOID)));
            Assert.That(odbType.Id, Is.EqualTo(OdbType.ClassOidId));
            Assert.That(odbType.Name, Is.EqualTo("NDatabase2.Odb.Core.Oid.ClassOID,NDatabase"));
            Assert.That(odbType.Size, Is.EqualTo(0));
            Assert.That(odbType.SubType, Is.Null);
            Assert.That(odbType.GetNativeClass(), Is.EqualTo(typeof(ClassOID)));
            Assert.That(odbType.HasFixSize(), Is.True); //TODO: is that correct?

            Assert.That(odbType.IsArray(), Is.False);
            Assert.That(odbType.IsArrayOrCollection(), Is.False);
            Assert.That(odbType.IsAtomicNative(), Is.True);
            Assert.That(odbType.IsBoolean(), Is.False);
            Assert.That(odbType.IsCollection(), Is.False);
            Assert.That(odbType.IsDate(), Is.False);
            Assert.That(odbType.IsEnum(), Is.False);
            Assert.That(odbType.IsMap(), Is.False);
            Assert.That(odbType.IsNative(), Is.True);
            Assert.That(odbType.IsNonNative(), Is.False);
            Assert.That(odbType.IsNull(), Is.False);
            Assert.That(odbType.IsStringOrBigDecimal(), Is.False);
        }

        [Test]
        public void It_should_contain_valid_string_type()
        {
            var odbType = OdbType.String;

            Assert.That(odbType.BaseClass, Is.Null);
            Assert.That(odbType.Id, Is.EqualTo(OdbType.StringId));
            Assert.That(odbType.Name, Is.StringStarting("System.String,mscorlib"));
            Assert.That(odbType.Size, Is.EqualTo(1));
            Assert.That(odbType.SubType, Is.Null);
            Assert.That(odbType.GetNativeClass(), Is.EqualTo(typeof(String)));
            Assert.That(odbType.HasFixSize(), Is.False);

            Assert.That(odbType.IsArray(), Is.False);
            Assert.That(odbType.IsArrayOrCollection(), Is.False);
            Assert.That(odbType.IsAtomicNative(), Is.True);
            Assert.That(odbType.IsBoolean(), Is.False);
            Assert.That(odbType.IsCollection(), Is.False);
            Assert.That(odbType.IsDate(), Is.False);
            Assert.That(odbType.IsEnum(), Is.False);
            Assert.That(odbType.IsMap(), Is.False);
            Assert.That(odbType.IsNative(), Is.True);
            Assert.That(odbType.IsNonNative(), Is.False);
            Assert.That(odbType.IsNull(), Is.False);
            Assert.That(odbType.IsStringOrBigDecimal(), Is.True);
        }

        [Test]
        public void It_should_contain_valid_collection_type()
        {
            var odbType = OdbType.Collection;

            Assert.That(odbType.BaseClass, Is.EqualTo(typeof(ICollection)));
            Assert.That(odbType.Id, Is.EqualTo(OdbType.CollectionId));
            Assert.That(odbType.Name, Is.StringStarting("System.Collections.ICollection,mscorlib"));
            Assert.That(odbType.Size, Is.EqualTo(0));
            Assert.That(odbType.SubType, Is.Null);
            Assert.That(odbType.GetNativeClass(), Is.EqualTo(typeof(ICollection)));
            Assert.That(odbType.HasFixSize(), Is.False);

            Assert.That(odbType.IsArray(), Is.False);
            Assert.That(odbType.IsArrayOrCollection(), Is.True);
            Assert.That(odbType.IsAtomicNative(), Is.False);
            Assert.That(odbType.IsBoolean(), Is.False);
            Assert.That(odbType.IsCollection(), Is.True);
            Assert.That(odbType.IsDate(), Is.False);
            Assert.That(odbType.IsEnum(), Is.False);
            Assert.That(odbType.IsMap(), Is.False);
            Assert.That(odbType.IsNative(), Is.True);
            Assert.That(odbType.IsNonNative(), Is.False);
            Assert.That(odbType.IsNull(), Is.False);
            Assert.That(odbType.IsStringOrBigDecimal(), Is.False);
        }

        [Test]
        public void It_should_contain_valid_collection_generic_type()
        {
            var odbType = OdbType.CollectionGeneric;

            Assert.That(odbType.BaseClass, Is.EqualTo(typeof(ICollection<object>)));
            Assert.That(odbType.Id, Is.EqualTo(OdbType.CollectionGenericId));
            Assert.That(odbType.Name, Is.StringStarting("System.Collections.Generic.ICollection"));
            Assert.That(odbType.Size, Is.EqualTo(0));
            Assert.That(odbType.SubType, Is.Null);
            Assert.That(odbType.GetNativeClass(), Is.EqualTo(typeof(ICollection<object>)));
            Assert.That(odbType.HasFixSize(), Is.False);

            Assert.That(odbType.IsArray(), Is.False);
            Assert.That(odbType.IsArrayOrCollection(), Is.True);
            Assert.That(odbType.IsAtomicNative(), Is.False);
            Assert.That(odbType.IsBoolean(), Is.False);
            Assert.That(odbType.IsCollection(), Is.True);
            Assert.That(odbType.IsDate(), Is.False);
            Assert.That(odbType.IsEnum(), Is.False);
            Assert.That(odbType.IsMap(), Is.False);
            Assert.That(odbType.IsNative(), Is.True);
            Assert.That(odbType.IsNonNative(), Is.False);
            Assert.That(odbType.IsNull(), Is.False);
            Assert.That(odbType.IsStringOrBigDecimal(), Is.False);
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
            Assert.That(odbType.HasFixSize(), Is.False);

            Assert.That(odbType.IsArray(), Is.True);
            Assert.That(odbType.IsArrayOrCollection(), Is.True);
            Assert.That(odbType.IsAtomicNative(), Is.False);
            Assert.That(odbType.IsBoolean(), Is.False);
            Assert.That(odbType.IsCollection(), Is.False);
            Assert.That(odbType.IsDate(), Is.False);
            Assert.That(odbType.IsEnum(), Is.False);
            Assert.That(odbType.IsMap(), Is.False);
            Assert.That(odbType.IsNative(), Is.True);
            Assert.That(odbType.IsNonNative(), Is.False);
            Assert.That(odbType.IsNull(), Is.False);
            Assert.That(odbType.IsStringOrBigDecimal(), Is.False);
        }

        [Test]
        public void It_should_contain_valid_map_type()
        {
            var odbType = OdbType.Map;

            Assert.That(odbType.BaseClass, Is.EqualTo(typeof(IDictionary)));
            Assert.That(odbType.Id, Is.EqualTo(OdbType.MapId));
            Assert.That(odbType.Name, Is.StringStarting("System.Collections.IDictionary,mscorlib"));
            Assert.That(odbType.Size, Is.EqualTo(0));
            Assert.That(odbType.SubType, Is.Null);
            Assert.That(odbType.GetNativeClass(), Is.EqualTo(typeof(IDictionary)));
            Assert.That(odbType.HasFixSize(), Is.False);

            Assert.That(odbType.IsArray(), Is.False);
            Assert.That(odbType.IsArrayOrCollection(), Is.False);
            Assert.That(odbType.IsAtomicNative(), Is.False);
            Assert.That(odbType.IsBoolean(), Is.False);
            Assert.That(odbType.IsCollection(), Is.False);
            Assert.That(odbType.IsDate(), Is.False);
            Assert.That(odbType.IsEnum(), Is.False);
            Assert.That(odbType.IsMap(), Is.True);
            Assert.That(odbType.IsNative(), Is.True);
            Assert.That(odbType.IsNonNative(), Is.False);
            Assert.That(odbType.IsNull(), Is.False);
            Assert.That(odbType.IsStringOrBigDecimal(), Is.False);
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
            Assert.That(odbType.HasFixSize(), Is.False);

            Assert.That(odbType.IsArray(), Is.False);
            Assert.That(odbType.IsArrayOrCollection(), Is.False);
            Assert.That(odbType.IsAtomicNative(), Is.False);
            Assert.That(odbType.IsBoolean(), Is.False);
            Assert.That(odbType.IsCollection(), Is.False);
            Assert.That(odbType.IsDate(), Is.False);
            Assert.That(odbType.IsEnum(), Is.False);
            Assert.That(odbType.IsMap(), Is.False);
            Assert.That(odbType.IsNative(), Is.False);
            Assert.That(odbType.IsNonNative(), Is.True);
            Assert.That(odbType.IsNull(), Is.False);
            Assert.That(odbType.IsStringOrBigDecimal(), Is.False);
        }
    }
}