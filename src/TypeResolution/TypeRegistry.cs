using System;
using System.Collections.Generic;

namespace NDatabase.TypeResolution
{
    /// <summary> 
    /// Provides access to a central registry of aliased <see cref="System.Type"/>s.
    /// </summary>
    /// <remarks>
    /// <p>
    /// Simplifies configuration by allowing aliases to be used instead of
    /// fully qualified type names.
    /// </p>
    /// <p>
    /// Comes 'pre-loaded' with a number of convenience alias' for the more
    /// common types; an example would be the '<c>int</c>' (or '<c>Integer</c>'
    /// for Visual Basic.NET developers) alias for the <see cref="System.Int32"/>
    /// type.
    /// </p>
    /// </remarks>
    internal sealed class TypeRegistry
    {
        /// <summary>
        /// The alias around the 'int' type.
        /// </summary>
        public const string Int32Alias = "int";

        /// <summary>
        /// The alias around the 'Integer' type (Visual Basic.NET style).
        /// </summary>
        public const string Int32AliasVB = "Integer";

        /// <summary>
        /// The alias around the 'int[]' array type.
        /// </summary>
        public const string Int32ArrayAlias = "int[]";

        /// <summary>
        /// The alias around the 'Integer()' array type (Visual Basic.NET style).
        /// </summary>
        public const string Int32ArrayAliasVB = "Integer()";

        /// <summary>
        /// The alias around the 'decimal' type.
        /// </summary>
        public const string DecimalAlias = "decimal";

        /// <summary>
        /// The alias around the 'Decimal' type (Visual Basic.NET style).
        /// </summary>
        public const string DecimalAliasVB = "Decimal";

        /// <summary>
        /// The alias around the 'decimal[]' array type.
        /// </summary>
        public const string DecimalArrayAlias = "decimal[]";

        /// <summary>
        /// The alias around the 'Decimal()' array type (Visual Basic.NET style).
        /// </summary>
        public const string DecimalArrayAliasVB = "Decimal()";

        /// <summary>
        /// The alias around the 'char' type.
        /// </summary>
        public const string CharAlias = "char";

        /// <summary>
        /// The alias around the 'Char' type (Visual Basic.NET style).
        /// </summary>
        public const string CharAliasVB = "Char";

        /// <summary>
        /// The alias around the 'char[]' array type.
        /// </summary>
        public const string CharArrayAlias = "char[]";

        /// <summary>
        /// The alias around the 'Char()' array type (Visual Basic.NET style).
        /// </summary>
        public const string CharArrayAliasVB = "Char()";

        /// <summary>
        /// The alias around the 'long' type.
        /// </summary>
        public const string Int64Alias = "long";

        /// <summary>
        /// The alias around the 'Long' type (Visual Basic.NET style).
        /// </summary>
        public const string Int64AliasVB = "Long";

        /// <summary>
        /// The alias around the 'long[]' array type.
        /// </summary>
        public const string Int64ArrayAlias = "long[]";

        /// <summary>
        /// The alias around the 'Long()' array type (Visual Basic.NET style).
        /// </summary>
        public const string Int64ArrayAliasVB = "Long()";

        /// <summary>
        /// The alias around the 'short' type.
        /// </summary>
        public const string Int16Alias = "short";

        /// <summary>
        /// The alias around the 'Short' type (Visual Basic.NET style).
        /// </summary>
        public const string Int16AliasVB = "Short";

        /// <summary>
        /// The alias around the 'short[]' array type.
        /// </summary>
        public const string Int16ArrayAlias = "short[]";

        /// <summary>
        /// The alias around the 'Short()' array type (Visual Basic.NET style).
        /// </summary>
        public const string Int16ArrayAliasVB = "Short()";

        /// <summary>
        /// The alias around the 'unsigned int' type.
        /// </summary>
        public const string UInt32Alias = "uint";

        /// <summary>
        /// The alias around the 'unsigned long' type.
        /// </summary>
        public const string UInt64Alias = "ulong";

        /// <summary>
        /// The alias around the 'ulong[]' array type.
        /// </summary>
        public const string UInt64ArrayAlias = "ulong[]";

        /// <summary>
        /// The alias around the 'uint[]' array type.
        /// </summary>
        public const string UInt32ArrayAlias = "uint[]";

        /// <summary>
        /// The alias around the 'unsigned short' type.
        /// </summary>
        public const string UInt16Alias = "ushort";

        /// <summary>
        /// The alias around the 'ushort[]' array type.
        /// </summary>
        public const string UInt16ArrayAlias = "ushort[]";

        /// <summary>
        /// The alias around the 'double' type.
        /// </summary>
        public const string DoubleAlias = "double";

        /// <summary>
        /// The alias around the 'Double' type (Visual Basic.NET style).
        /// </summary>
        public const string DoubleAliasVB = "Double";

        /// <summary>
        /// The alias around the 'double[]' array type.
        /// </summary>
        public const string DoubleArrayAlias = "double[]";

        /// <summary>
        /// The alias around the 'Double()' array type (Visual Basic.NET style).
        /// </summary>
        public const string DoubleArrayAliasVB = "Double()";

        /// <summary>
        /// The alias around the 'float' type.
        /// </summary>
        public const string FloatAlias = "float";

        /// <summary>
        /// The alias around the 'Single' type (Visual Basic.NET style).
        /// </summary>
        public const string SingleAlias = "Single";

        /// <summary>
        /// The alias around the 'float[]' array type.
        /// </summary>
        public const string FloatArrayAlias = "float[]";

        /// <summary>
        /// The alias around the 'Single()' array type (Visual Basic.NET style).
        /// </summary>
        public const string SingleArrayAliasVB = "Single()";

        /// <summary>
        /// The alias around the 'DateTime' type.
        /// </summary>
        public const string DateTimeAlias = "DateTime";

        /// <summary>
        /// The alias around the 'DateTime' type (C# style).
        /// </summary>
        public const string DateAlias = "date";

        /// <summary>
        /// The alias around the 'DateTime' type (Visual Basic.NET style).
        /// </summary>
        public const string DateAliasVB = "Date";

        /// <summary>
        /// The alias around the 'DateTime[]' array type.
        /// </summary>
        public const string DateTimeArrayAlias = "DateTime[]";

        /// <summary>
        /// The alias around the 'DateTime[]' array type.
        /// </summary>
        public const string DateTimeArrayAliasCSharp = "date[]";

        /// <summary>
        /// The alias around the 'DateTime()' array type (Visual Basic.NET style).
        /// </summary>
        public const string DateTimeArrayAliasVB = "DateTime()";

        /// <summary>
        /// The alias around the 'bool' type.
        /// </summary>
        public const string BoolAlias = "bool";

        /// <summary>
        /// The alias around the 'Boolean' type (Visual Basic.NET style).
        /// </summary>
        public const string BoolAliasVB = "Boolean";

        /// <summary>
        /// The alias around the 'bool[]' array type.
        /// </summary>
        public const string BoolArrayAlias = "bool[]";

        /// <summary>
        /// The alias around the 'Boolean()' array type (Visual Basic.NET style).
        /// </summary>
        public const string BoolArrayAliasVB = "Boolean()";

        /// <summary>
        /// The alias around the 'string' type.
        /// </summary>
        public const string StringAlias = "string";

        /// <summary>
        /// The alias around the 'string' type (Visual Basic.NET style).
        /// </summary>
        public const string StringAliasVB = "String";

        /// <summary>
        /// The alias around the 'string[]' array type.
        /// </summary>
        public const string StringArrayAlias = "string[]";

        /// <summary>
        /// The alias around the 'string[]' array type (Visual Basic.NET style).
        /// </summary>
        public const string StringArrayAliasVB = "String()";

        /// <summary>
        /// The alias around the 'object' type.
        /// </summary>
        public const string ObjectAlias = "object";

        /// <summary>
        /// The alias around the 'object' type (Visual Basic.NET style).
        /// </summary>
        public const string ObjectAliasVB = "Object";

        /// <summary>
        /// The alias around the 'object[]' array type.
        /// </summary>
        public const string ObjectArrayAlias = "object[]";

        /// <summary>
        /// The alias around the 'object[]' array type (Visual Basic.NET style).
        /// </summary>
        public const string ObjectArrayAliasVB = "Object()";

        /// <summary>
        /// The alias around the 'int?' type.
        /// </summary>
        public const string NullableInt32Alias = "int?";

        /// <summary>
        /// The alias around the 'int?[]' array type.
        /// </summary>
        public const string NullableInt32ArrayAlias = "int?[]";

        /// <summary>
        /// The alias around the 'decimal?' type.
        /// </summary>
        public const string NullableDecimalAlias = "decimal?";

        /// <summary>
        /// The alias around the 'decimal?[]' array type.
        /// </summary>
        public const string NullableDecimalArrayAlias = "decimal?[]";

        /// <summary>
        /// The alias around the 'char?' type.
        /// </summary>
        public const string NullableCharAlias = "char?";

        /// <summary>
        /// The alias around the 'char?[]' array type.
        /// </summary>
        public const string NullableCharArrayAlias = "char?[]";

        /// <summary>
        /// The alias around the 'long?' type.
        /// </summary>
        public const string NullableInt64Alias = "long?";

        /// <summary>
        /// The alias around the 'long?[]' array type.
        /// </summary>
        public const string NullableInt64ArrayAlias = "long?[]";

        /// <summary>
        /// The alias around the 'short?' type.
        /// </summary>
        public const string NullableInt16Alias = "short?";

        /// <summary>
        /// The alias around the 'short?[]' array type.
        /// </summary>
        public const string NullableInt16ArrayAlias = "short?[]";

        /// <summary>
        /// The alias around the 'unsigned int?' type.
        /// </summary>
        public const string NullableUInt32Alias = "uint?";

        /// <summary>
        /// The alias around the 'unsigned long?' type.
        /// </summary>
        public const string NullableUInt64Alias = "ulong?";

        /// <summary>
        /// The alias around the 'ulong?[]' array type.
        /// </summary>
        public const string NullableUInt64ArrayAlias = "ulong?[]";

        /// <summary>
        /// The alias around the 'uint?[]' array type.
        /// </summary>
        public const string NullableUInt32ArrayAlias = "uint?[]";

        /// <summary>
        /// The alias around the 'unsigned short?' type.
        /// </summary>
        public const string NullableUInt16Alias = "ushort?";

        /// <summary>
        /// The alias around the 'ushort?[]' array type.
        /// </summary>
        public const string NullableUInt16ArrayAlias = "ushort?[]";

        /// <summary>
        /// The alias around the 'double?' type.
        /// </summary>
        public const string NullableDoubleAlias = "double?";

        /// <summary>
        /// The alias around the 'double?[]' array type.
        /// </summary>
        public const string NullableDoubleArrayAlias = "double?[]";

        /// <summary>
        /// The alias around the 'float?' type.
        /// </summary>
        public const string NullableFloatAlias = "float?";

        /// <summary>
        /// The alias around the 'float?[]' array type.
        /// </summary>
        public const string NullableFloatArrayAlias = "float?[]";

        /// <summary>
        /// The alias around the 'bool?' type.
        /// </summary>
        public const string NullableBoolAlias = "bool?";

        /// <summary>
        /// The alias around the 'bool?[]' array type.
        /// </summary>
        public const string NullableBoolArrayAlias = "bool?[]";

        private static readonly object SyncRoot = new object();
        private static readonly IDictionary<string, Type> Types = new Dictionary<string, Type>();

        #region Constructor (s) / Destructor

        /// <summary>
        /// Registers standard and user-configured type aliases.
        /// </summary>
        static TypeRegistry()
        {
            lock (SyncRoot)
            {
                Types["Int32"] = typeof(Int32);
                Types[Int32Alias] = typeof(Int32);
                Types[Int32AliasVB] = typeof(Int32);
                Types[Int32ArrayAlias] = typeof(Int32[]);
                Types[Int32ArrayAliasVB] = typeof(Int32[]);

                Types["UInt32"] = typeof(UInt32);
                Types[UInt32Alias] = typeof(UInt32);
                Types[UInt32ArrayAlias] = typeof(UInt32[]);

                Types["Int16"] = typeof(Int16);
                Types[Int16Alias] = typeof(Int16);
                Types[Int16AliasVB] = typeof(Int16);
                Types[Int16ArrayAlias] = typeof(Int16[]);
                Types[Int16ArrayAliasVB] = typeof(Int16[]);

                Types["UInt16"] = typeof(UInt16);
                Types[UInt16Alias] = typeof(UInt16);
                Types[UInt16ArrayAlias] = typeof(UInt16[]);

                Types["Int64"] = typeof(Int64);
                Types[Int64Alias] = typeof(Int64);
                Types[Int64AliasVB] = typeof(Int64);
                Types[Int64ArrayAlias] = typeof(Int64[]);
                Types[Int64ArrayAliasVB] = typeof(Int64[]);

                Types["UInt64"] = typeof(UInt64);
                Types[UInt64Alias] = typeof(UInt64);
                Types[UInt64ArrayAlias] = typeof(UInt64[]);

                Types[DoubleAlias] = typeof(double);
                Types[DoubleAliasVB] = typeof(double);
                Types[DoubleArrayAlias] = typeof(double[]);
                Types[DoubleArrayAliasVB] = typeof(double[]);

                Types[FloatAlias] = typeof(float);
                Types[SingleAlias] = typeof(float);
                Types[FloatArrayAlias] = typeof(float[]);
                Types[SingleArrayAliasVB] = typeof(float[]);

                Types[DateTimeAlias] = typeof(DateTime);
                Types[DateAlias] = typeof(DateTime);
                Types[DateAliasVB] = typeof(DateTime);
                Types[DateTimeArrayAlias] = typeof(DateTime[]);
                Types[DateTimeArrayAliasCSharp] = typeof(DateTime[]);
                Types[DateTimeArrayAliasVB] = typeof(DateTime[]);

                Types[BoolAlias] = typeof(bool);
                Types[BoolAliasVB] = typeof(bool);
                Types[BoolArrayAlias] = typeof(bool[]);
                Types[BoolArrayAliasVB] = typeof(bool[]);

                Types[DecimalAlias] = typeof(decimal);
                Types[DecimalAliasVB] = typeof(decimal);
                Types[DecimalArrayAlias] = typeof(decimal[]);
                Types[DecimalArrayAliasVB] = typeof(decimal[]);

                Types[CharAlias] = typeof(char);
                Types[CharAliasVB] = typeof(char);
                Types[CharArrayAlias] = typeof(char[]);
                Types[CharArrayAliasVB] = typeof(char[]);

                Types[StringAlias] = typeof(string);
                Types[StringAliasVB] = typeof(string);
                Types[StringArrayAlias] = typeof(string[]);
                Types[StringArrayAliasVB] = typeof(string[]);

                Types[ObjectAlias] = typeof(object);
                Types[ObjectAliasVB] = typeof(object);
                Types[ObjectArrayAlias] = typeof(object[]);
                Types[ObjectArrayAliasVB] = typeof(object[]);

                Types[NullableInt32Alias] = typeof(int?);
                Types[NullableInt32ArrayAlias] = typeof(int?[]);

                Types[NullableDecimalAlias] = typeof(decimal?);
                Types[NullableDecimalArrayAlias] = typeof(decimal?[]);

                Types[NullableCharAlias] = typeof(char?);
                Types[NullableCharArrayAlias] = typeof(char?[]);

                Types[NullableInt64Alias] = typeof(long?);
                Types[NullableInt64ArrayAlias] = typeof(long?[]);

                Types[NullableInt16Alias] = typeof(short?);
                Types[NullableInt16ArrayAlias] = typeof(short?[]);

                Types[NullableUInt32Alias] = typeof(uint?);
                Types[NullableUInt32ArrayAlias] = typeof(uint?[]);

                Types[NullableUInt64Alias] = typeof(ulong?);
                Types[NullableUInt64ArrayAlias] = typeof(ulong?[]);

                Types[NullableUInt16Alias] = typeof(ushort?);
                Types[NullableUInt16ArrayAlias] = typeof(ushort?[]);

                Types[NullableDoubleAlias] = typeof(double?);
                Types[NullableDoubleArrayAlias] = typeof(double?[]);

                Types[NullableFloatAlias] = typeof(float?);
                Types[NullableFloatArrayAlias] = typeof(float?[]);

                Types[NullableBoolAlias] = typeof(bool?);
                Types[NullableBoolArrayAlias] = typeof(bool?[]);
            }
        }

        #endregion

        /// <summary> 
        /// Resolves the supplied <paramref name="alias"/> to a <see cref="System.Type"/>. 
        /// </summary> 
        /// <param name="alias">
        /// The alias to resolve.
        /// </param>
        /// <returns>
        /// The <see cref="System.Type"/> the supplied <paramref name="alias"/> was
        /// associated with, or <see lang="null"/> if no <see cref="System.Type"/> 
        /// was previously registered for the supplied <paramref name="alias"/>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// If the supplied <paramref name="alias"/> is <see langword="null"/> or
        /// contains only whitespace character(s).
        /// </exception>
        public static Type ResolveType(string alias)
        {
            if (string.IsNullOrWhiteSpace(alias))
                throw new ArgumentNullException("alias");

            Type type;
            Types.TryGetValue(alias, out type);
            return type;
        }
    }
}