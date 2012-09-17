using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using NDatabase.Odb.Core.Layers.Layer1.Introspector;
using NDatabase.Odb.Core.Layers.Layer2.Instance;
using NDatabase.Odb.Core.Oid;
using NDatabase.Tool.Wrappers;
using NDatabase.Tool.Wrappers.List;
using NDatabase.Tool.Wrappers.Map;

namespace NDatabase.Odb.Core.Layers.Layer2.Meta
{
    /// <summary>
    ///   Contains the list for the ODB types
    /// </summary>
    public sealed class OdbType
    {
        public override int GetHashCode()
        {
            unchecked
            {
                return (_id * 397) ^ (_name != null
                                          ? _name.GetHashCode()
                                          : 0);
            }
        }

        public const int NullId = 0;

        public const int BooleanId = 10;

        /// <summary>
        ///   1 byte
        /// </summary>
        public const int ByteId = 20;

        public const int SignedByteId = 21;

        public const int CharacterId = 30;

        /// <summary>
        ///   2 byte
        /// </summary>
        public const int ShortId = 40;

        /// <summary>
        ///   4 byte
        /// </summary>
        public const int IntegerId = 50;

        /// <summary>
        ///   8 bytes
        /// </summary>
        public const int LongId = 60;

        /// <summary>
        ///   4 byte
        /// </summary>
        public const int FloatId = 70;

        /// <summary>
        ///   8 byte
        /// </summary>
        public const int DoubleId = 80;

        /// <summary>
        ///   16 byte
        /// </summary>
        public const int DecimalId = 100;

        public const int DateId = 170;

        public const int DateSqlId = 171;

        public const int DateTimestampId = 172;

        public const int OidId = 180;

        public const int ObjectOidId = 181;

        public const int ClassOidId = 182;

        public const int StringId = 210;

        /// <summary>
        ///   Enums are internally stored as String: the enum name
        /// </summary>
        public const int EnumId = 211;

        public const int NativeFixSizeMaxId = ClassOidId;

        public const int NativeMaxId = StringId;

        public const int CollectionId = 250;
        public const int CollectionGenericId = 251;

        public const int ArrayId = 260;

        public const int MapId = 270;

        public const int NonNativeId = 300;

        [NonPersistent]
        private static IClassPool _classPool;

        public static readonly OdbType Null = new OdbType(true, NullId, "null", 1);
        
        /// <summary>
        ///   1 byte
        /// </summary>
        public static readonly OdbType Byte = new OdbType(true, ByteId, OdbClassUtil.GetFullName(typeof(byte)), 1);

        /// <summary>
        ///   2 byte
        /// </summary>
        public static readonly OdbType Short = new OdbType(true, ShortId, OdbClassUtil.GetFullName(typeof(short)), 2);

        /// <summary>
        ///   4 byte
        /// </summary>
        public static readonly OdbType Integer = new OdbType(true, IntegerId, OdbClassUtil.GetFullName(typeof(int)), 4);

        /// <summary>
        ///   16 byte
        /// </summary>
        public static readonly OdbType Decimal = new OdbType(true, DecimalId, OdbClassUtil.GetFullName(typeof (decimal)),
                                                             16);
        /// <summary>
        ///   8 bytes
        /// </summary>
        public static readonly OdbType Long = new OdbType(true, LongId, OdbClassUtil.GetFullName(typeof(long)), 8);

        /// <summary>
        ///   4 byte
        /// </summary>
        public static readonly OdbType Float = new OdbType(true, FloatId, OdbClassUtil.GetFullName(typeof(float)), 4);

        /// <summary>
        ///   8 byte
        /// </summary>
        public static readonly OdbType Double = new OdbType(true, DoubleId, OdbClassUtil.GetFullName(typeof(double)),
                                                            8);

        /// <summary>
        ///   2 byte
        /// </summary>
        public static readonly OdbType Character = new OdbType(true, CharacterId,
                                                               OdbClassUtil.GetFullName(typeof (char)), 2);
        
        /// <summary>
        ///   1 byte
        /// </summary>
        public static readonly OdbType Boolean = new OdbType(true, BooleanId, OdbClassUtil.GetFullName(typeof(bool)), 1);

        /// <summary>
        ///   8 byte
        /// </summary>
        public static readonly OdbType Date = new OdbType(false, DateId, OdbClassUtil.GetFullName(typeof (DateTime)), 8);

        public static readonly OdbType String = new OdbType(false, StringId, OdbClassUtil.GetFullName(typeof (string)),
                                                            1);

        public static readonly OdbType Enum = new OdbType(false, EnumId, OdbClassUtil.GetFullName(typeof (Enum)), 1);

        public static readonly OdbType Collection = new OdbType(false, CollectionId,
                                                                OdbClassUtil.GetFullName(typeof (ICollection)), 0,
                                                                typeof (ICollection));

        public static readonly OdbType CollectionGeneric = new OdbType(false, CollectionGenericId,
                                                                       OdbClassUtil.GetFullName(
                                                                           typeof (ICollection<object>)), 0,
                                                                       typeof (ICollection<object>));

        public static readonly OdbType Array = new OdbType(false, ArrayId, "array", 0);

        public static readonly OdbType Map = new OdbType(false, MapId, OdbClassUtil.GetFullName(typeof (IDictionary)), 0,
                                                         typeof (IDictionary));

        public static readonly OdbType Oid = new OdbType(false, OidId, OdbClassUtil.GetFullName(typeof (OID)), 0,
                                                         typeof (OID));

        public static readonly OdbType ObjectOid = new OdbType(false, ObjectOidId,
                                                               OdbClassUtil.GetFullName(typeof (OdbObjectOID)), 0,
                                                               typeof (OdbObjectOID));

        public static readonly OdbType ClassOid = new OdbType(false, ClassOidId,
                                                              OdbClassUtil.GetFullName(typeof (OdbClassOID)), 0,
                                                              typeof (OdbClassOID));

        public static readonly OdbType NonNative = new OdbType(false, NonNativeId, "non native", 0);

        private static readonly IDictionary<int, OdbType> TypesById = new OdbHashMap<int, OdbType>();

        private static readonly IDictionary<string, OdbType> TypesByName = new OdbHashMap<string, OdbType>();

        /// <summary>
        ///   This cache is used to cache non default types.
        /// </summary>
        /// <remarks>
        ///   This cache is used to cache non default types. Instead or always testing if a class is an array or a collection or any other, we put the odbtype in this cache
        /// </remarks>
        private static readonly IDictionary<string, OdbType> CacheOfTypesByName = new OdbHashMap<string, OdbType>();

        public static readonly string DefaultCollectionClassName = OdbClassUtil.GetFullName(typeof (ArrayList));

        public static readonly string DefaultMapClassName = OdbClassUtil.GetFullName(typeof (Hashtable));

        public static readonly string DefaultArrayComponentClassName = OdbClassUtil.GetFullName(typeof (object));

        public static readonly int SizeOfInt = Integer.GetSize();

        public static readonly int SizeOfLong = Long.GetSize();

        public static readonly int SizeOfBool = Boolean.GetSize();

        public static readonly int SizeOfByte = Byte.GetSize();

        private readonly int _id;
        private readonly bool _isPrimitive;
        private readonly int _size;
        private string _name;

        /// <summary>
        ///   For array element type
        /// </summary>
        private OdbType _subType;

        private Type _superClass;

        static OdbType()
        {
            IOdbList<OdbType> allTypes = new OdbList<OdbType>(100);
            //// DO NOT FORGET DO ADD THE TYPE IN THIS LIST WHEN CREATING A NEW ONE!!!
            allTypes.Add(Null);
            allTypes.Add(Byte);
            allTypes.Add(Short);
            allTypes.Add(Integer);
            allTypes.Add(Long);
            allTypes.Add(Float);
            allTypes.Add(Double);
            allTypes.Add(Decimal);
            allTypes.Add(Character);
            allTypes.Add(Boolean);
            allTypes.Add(Date);
            allTypes.Add(String);
            allTypes.Add(Enum);
            allTypes.Add(Collection);
            allTypes.Add(CollectionGeneric);
            allTypes.Add(Array);
            allTypes.Add(Map);
            allTypes.Add(Oid);
            allTypes.Add(ObjectOid);
            allTypes.Add(ClassOid);
            allTypes.Add(NonNative);

            foreach (var type in allTypes)
            {
                TypesByName[type.GetName()] = type;
                TypesById[type.GetId()] = type;
            }
        }

        private OdbType(bool isPrimitive, int id, string name, int size)
        {
            _isPrimitive = isPrimitive;
            _id = id;
            _name = name;
            _size = size;
        }

        private OdbType(bool isPrimitive, int id, string name, int size, Type superclass)
        {
            _isPrimitive = isPrimitive;
            _id = id;
            _name = name;
            _size = size;
            _superClass = superclass;
        }

        public static int Number { get; private set; }

        private void InitClassPool()
        {
            lock (this)
            {
                _classPool = OdbConfiguration.GetCoreProvider().GetClassPool();
            }
        }

        public OdbType Copy()
        {
            return new OdbType(_isPrimitive, _id, _name, _size) {_subType = GetSubType()};
        }

        public static OdbType GetFromId(int id)
        {
            var odbType = TypesById[id];
            if (odbType == null)
                throw new OdbRuntimeException(NDatabaseError.OdbTypeIdDoesNotExist.AddParameter(id));
            return odbType;
        }

        public static string GetNameFromId(int id)
        {
            return GetFromId(id).GetName();
        }

        public static OdbType GetFromName(string fullName)
        {
            OdbType odbType;

            TypesByName.TryGetValue(fullName, out odbType);
            
            return odbType ?? new OdbType(NonNative._isPrimitive, NonNativeId, fullName, 0);
        }

        public static OdbType GetFromClass(Type clazz)
        {
            if (clazz.IsEnum)
            {
                var type = new OdbType(Enum._isPrimitive, EnumId, Enum.GetName(), 0);
                type.SetName(OdbClassUtil.GetFullName(clazz));
                return type;
            }

            var className = OdbClassUtil.GetFullName(clazz);

            // First check if it is a 'default type'
            OdbType odbType;
            var success = TypesByName.TryGetValue(className, out odbType);
            if (success)
                return odbType;

            // Then check if it is a 'non default type'
            success = CacheOfTypesByName.TryGetValue(className, out odbType);
            if (success)
                return odbType;

            if (clazz.IsArray)
            {
                var type = new OdbType(Array._isPrimitive, ArrayId, Array.GetName(), 0)
                    {_subType = GetFromClass(clazz.GetElementType())};

                CacheOfTypesByName.Add(className, type);
                return type;
            }

            if (IsMap(clazz))
            {
                CacheOfTypesByName.Add(className, Map);
                return Map;
            }
            // check if it is a list
            if (IsCollection(clazz))
            {
                CacheOfTypesByName.Add(className, Collection);
                return Collection;
            }
            Number++;
            var nonNative = new OdbType(NonNative._isPrimitive, NonNativeId, className, 0);
            CacheOfTypesByName.Add(className, nonNative);
            return nonNative;
        }

        public static bool IsMap(Type clazz)
        {
            var isNonGenericMap = Map._superClass.IsAssignableFrom(clazz);

            if (isNonGenericMap)
                return true;

            var types = clazz.GetInterfaces();

            foreach (var type in types)
            {
                var ind = type.FullName.IndexOf("System.Collections.Generic.IDictionary", StringComparison.Ordinal);
                if (ind != -1)
                    return true;
            }
            
            return false;
        }

        public static bool IsCollection(Type clazz)
        {
            var isNonGenericCollection = Collection._superClass.IsAssignableFrom(clazz);
            if (isNonGenericCollection)
                return true;
            var types = clazz.GetInterfaces();
            
            foreach (var type in types)
            {
                var ind = type.FullName.IndexOf("System.Collections.Generic.ICollection", StringComparison.Ordinal);
                if (ind != -1)
                    return true;
            }
            
            return false;
        }

        public static bool IsNative(Type clazz)
        {
            OdbType odbType;

            TypesByName.TryGetValue(OdbClassUtil.GetFullName(clazz), out odbType);
            if (odbType != null)
                return true;
            if (clazz.IsArray)
            {
                //ODBType type = new ODBType(ODBType.ARRAY.isPrimitive,ODBType.ARRAY_ID,ODBType.ARRAY.getName(),0);
                //type.subType = getFromClass(clazz.getComponentType());
                return true;
            }
            if (Map._superClass.IsAssignableFrom(clazz))
                return true;
            // check if it is a list
            if (Collection._superClass.IsAssignableFrom(clazz))
                return true;
            return false;
        }

        public static bool Exist(string name)
        {
            return TypesByName.ContainsKey(name);
        }

        public int GetId()
        {
            return _id;
        }

        public string GetName()
        {
            return _name;
        }

        public void SetName(string name)
        {
            _name = name;
        }

        public int GetSize()
        {
            return _size;
        }

        public bool IsCollection()
        {
            return _id == CollectionId;
        }

        public static bool IsCollection(int odbTypeId)
        {
            return odbTypeId == CollectionId;
        }

        public bool IsArray()
        {
            return _id == ArrayId;
        }

        public static bool IsArray(int odbTypeId)
        {
            return odbTypeId == ArrayId;
        }

        public bool IsMap()
        {
            return _id == MapId;
        }

        public static bool IsMap(int odbTypeId)
        {
            return odbTypeId == MapId;
        }

        public bool IsArrayOrCollection()
        {
            return IsArray() || IsCollection();
        }

        public static bool IsNative(int odbtypeId)
        {
            return odbtypeId != NonNativeId;
        }

        public bool IsNative()
        {
            return _id != NonNativeId;
        }

        public OdbType GetSubType()
        {
            return _subType;
        }

        public Type GetSuperClass()
        {
            return _superClass;
        }

        public void SetSuperClass(Type superClass)
        {
            _superClass = superClass;
        }

        public override string ToString()
        {
            return System.String.Format("{0} - {1}", _id, _name);
        }

        public void SetSubType(OdbType subType)
        {
            _subType = subType;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != typeof (OdbType))
                return false;
            var type = (OdbType) obj;
            return GetId() == type.GetId();
        }

        public Type GetNativeClass()
        {
            switch (_id)
            {
                case BooleanId:
                    return typeof (bool);
                case ByteId:
                    return typeof (byte);
                case CharacterId:
                    return typeof (char);
                case DoubleId:
                    return typeof (double);
                case FloatId:
                    return typeof (float);
                case IntegerId:
                    return typeof (int);
                case LongId:
                    return typeof (long);
                case ShortId:
                    return typeof (short);
                case DecimalId:
                    return typeof (decimal);
                case ObjectOidId:
                    return typeof (OdbObjectOID);
                case ClassOidId:
                    return typeof (OdbClassOID);
                case OidId:
                    return typeof (OID);
            }
            if (_classPool == null)
                InitClassPool();
            Debug.Assert(_classPool != null);

            return _classPool.GetClass(GetName());
        }

        public bool IsNonNative()
        {
            return _id == NonNativeId;
        }

        public static bool IsNonNative(int odbtypeId)
        {
            return odbtypeId == NonNativeId;
        }

        public bool IsNull()
        {
            return _id == NullId;
        }

        public static bool IsNull(int odbTypeId)
        {
            return odbTypeId == NullId;
        }

        public bool HasFixSize()
        {
            return HasFixSize(_id);
        }

        public static bool HasFixSize(int odbId)
        {
            return odbId > 0 && odbId <= NativeFixSizeMaxId;
        }

        //return odbId != BIG_INTEGER_ID && odbId != BIG_DECIMAL_ID && odbId != STRING_ID && odbId != COLLECTION_ID && odbId!=ARRAY_ID && odbId!= MAP_ID && odbId!=NON_NATIVE_ID;
        public bool IsStringOrBigDecimal()
        {
            return IsStringOrBigDecimal(_id);
        }

        public static bool IsStringOrBigDecimal(int odbTypeId)
        {
            return odbTypeId == StringId || odbTypeId == DecimalId;
        }

        public static bool IsAtomicNative(int odbTypeId)
        {
            return (odbTypeId > 0 && odbTypeId <= NativeMaxId);
        }

        public bool IsAtomicNative()
        {
            return IsAtomicNative(_id);
        }

        public static bool IsEnum(int odbTypeId)
        {
            return odbTypeId == EnumId;
        }

        public bool IsEnum()
        {
            return IsEnum(_id);
        }

        public static bool IsPrimitive(int odbTypeId)
        {
            return GetFromId(odbTypeId)._isPrimitive;
        }

        public static bool TypesAreCompatible(OdbType type1, OdbType type2)
        {
            if (type1.IsArray() && type2.IsArray())
                return TypesAreCompatible(type1.GetSubType(), type2.GetSubType());
            if (type1.GetName().Equals(type2.GetName()))
                return true;
            if (type1.IsNative() && type2.IsNative())
            {
                if (type1.IsEquivalent(type2))
                    return true;
                return false;
            }
            if (type1.IsNonNative() && type2.IsNonNative())
            {
                return (type1.GetNativeClass() == type2.GetNativeClass()) ||
                       (type1.GetNativeClass().IsAssignableFrom(type2.GetNativeClass()));
            }
            return false;
        }

        public bool IsBoolean()
        {
            return _id == BooleanId;
        }

        private bool IsEquivalent(OdbType type2)
        {
            return (_id == IntegerId && type2._id == IntegerId);
        }

        public bool IsDate()
        {
            return _id == DateId;
        }
    }
}
