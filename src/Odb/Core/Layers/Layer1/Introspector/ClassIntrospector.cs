using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NDatabase.Btree;
using NDatabase.Btree.Impl;
using NDatabase.Odb.Core.BTree;
using NDatabase.Odb.Core.Layers.Layer2.Instance;
using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Odb.Core.Oid;
using NDatabase.Tool.Wrappers;
using NDatabase.Tool.Wrappers.List;

namespace NDatabase.Odb.Core.Layers.Layer1.Introspector
{
    /// <summary>
    ///   The ClassIntrospector is used to introspect classes.
    /// </summary>
    /// <remarks>
    ///   The ClassIntrospector is used to introspect classes. 
    ///   It uses Reflection to extract class information. 
    ///   It transforms a native Class into a ClassInfo (a meta representation of the class) 
    ///   that contains all informations about the class.
    /// </remarks>
    internal static class ClassIntrospector
    {
        private static readonly ConcurrentDictionary<Type, IList<FieldInfo>> Fields =
            new ConcurrentDictionary<Type, IList<FieldInfo>>();

        private static readonly IDictionary<string, Type> SystemClasses = new Dictionary<string, Type>();

        static ClassIntrospector()
        {
            SystemClasses.Add(typeof (ClassInfoIndex).FullName, typeof (ClassInfoIndex));
            SystemClasses.Add(typeof (OID).FullName, typeof (OID));
            SystemClasses.Add(typeof (ObjectOID).FullName, typeof (ObjectOID));
            SystemClasses.Add(typeof (ClassOID).FullName, typeof (ClassOID));
            SystemClasses.Add(typeof (OdbBtreeNodeSingle).FullName, typeof (OdbBtreeNodeSingle));
            SystemClasses.Add(typeof (OdbBtreeNodeMultiple).FullName, typeof (OdbBtreeNodeMultiple));
            SystemClasses.Add(typeof (OdbBtreeSingle).FullName, typeof (OdbBtreeSingle));
            SystemClasses.Add(typeof (IBTree).FullName, typeof (IBTree));
            SystemClasses.Add(typeof (IBTreeNodeOneValuePerKey).FullName, typeof (IBTreeNodeOneValuePerKey));
            SystemClasses.Add(typeof (IKeyAndValue).FullName, typeof (IKeyAndValue));
            SystemClasses.Add(typeof (KeyAndValue).FullName, typeof (KeyAndValue));
        }

        /// <summary>
        /// </summary>
        /// <param name="clazz"> The class to instrospect </param>
        /// <param name="recursive"> If true, goes does the hierarchy to try to analyse all classes </param>
        /// <returns> </returns>
        public static ClassInfoList Introspect(Type clazz, bool recursive)
        {
            return InternalIntrospect(clazz, recursive, null);
        }

        public static IList<FieldInfo> GetAllFieldsFrom(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type", "Type cannot be null.");

            return Fields.GetOrAdd(type, GetFieldInfo);
        }

        private static IList<FieldInfo> GetFieldInfo(Type type)
        {
            const int capacity = 50;

            var attributesNames = new List<string>(capacity);
            var result = new List<FieldInfo>(capacity);

            var classes = GetAllClasses(type);

            foreach (var class1 in classes)
            {
                var baseClassfields =
                    class1.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public |
                                     BindingFlags.DeclaredOnly);

                foreach (var fieldInfo in baseClassfields)
                {
                    if (attributesNames.Contains(fieldInfo.Name))
                        continue;

                    result.Add(fieldInfo);
                    attributesNames.Add(fieldInfo.Name);
                }
            }

            result = FilterFields(result).OrderBy(field => field.Name).ToList();
            return result;
        }

        /// <summary>
        ///   introspect a list of classes This method return the current meta model based on the classes that currently exist in the execution classpath.
        /// </summary>
        /// <remarks>
        ///   introspect a list of classes This method return the current meta model based on the classes that currently exist in the execution classpath. 
        ///   The result will be used to check meta model compatiblity between the meta model that is currently persisted in the database and the meta 
        ///   model currently executing in JVM. This is used b the automatic meta model refactoring
        /// </remarks>
        /// <returns> A map where the key is the class name and the key is the ClassInfo: the class meta representation </returns>
        public static IDictionary<string, ClassInfo> Instrospect(IEnumerable<ClassInfo> classInfos)
        {
            IDictionary<string, ClassInfo> classInfoSet = new Dictionary<string, ClassInfo>();

            foreach (var persistedClassInfo in classInfos)
            {
                var currentClassInfo = GetClassInfo(persistedClassInfo.FullClassName, persistedClassInfo);

                classInfoSet.Add(currentClassInfo.FullClassName, currentClassInfo);
            }

            return classInfoSet;
        }

        public static ClassInfoList Introspect(String fullClassName, bool recursive)
        {
            return Introspect(OdbClassPool.GetClass(fullClassName), true);
        }

        /// <summary>
        ///   Builds a class info from a class and an existing class info 
        ///   The existing class info is used to make sure that fields with the same name will have the same id
        /// </summary>
        /// <param name="fullClassName"> The name of the class to get info </param>
        /// <param name="existingClassInfo"> </param>
        /// <returns> A ClassInfo - a meta representation of the class </returns>
        private static ClassInfo GetClassInfo(String fullClassName, ClassInfo existingClassInfo)
        {
            var classInfo = new ClassInfo(fullClassName) {ClassCategory = GetClassCategory(fullClassName)};

            var type = OdbClassPool.GetClass(fullClassName);
            var fields = GetAllFieldsFrom(type);
            IOdbList<ClassAttributeInfo> attributes = new OdbList<ClassAttributeInfo>(fields.Count);

            var maxAttributeId = existingClassInfo.MaxAttributeId;
            foreach (var fieldInfo in fields)
            {
                // Gets the attribute id from the existing class info
                var attributeId = existingClassInfo.GetAttributeId(fieldInfo.Name);
                if (attributeId == - 1)
                {
                    maxAttributeId++;
                    // The attibute with field.getName() does not exist in existing class info
                    //  create a new id
                    attributeId = maxAttributeId;
                }
                var fieldClassInfo = !OdbType.GetFromClass(fieldInfo.FieldType).IsNative()
                                         ? new ClassInfo(fieldInfo.FieldType)
                                         : null;

                attributes.Add(new ClassAttributeInfo(attributeId, fieldInfo.Name, fieldInfo.FieldType,
                                                      OdbClassUtil.GetFullName(fieldInfo.FieldType), fieldClassInfo));
            }

            classInfo.Attributes = attributes;
            classInfo.MaxAttributeId = maxAttributeId;

            return classInfo;
        }

        private static IEnumerable<Type> GetAllClasses(Type type)
        {
            var result = new List<Type> {type};

            var baseType = type.BaseType;

            while (baseType != null && baseType != typeof (Object))
            {
                result.Add(baseType);
                baseType = baseType.BaseType;
            }

            return result;
        }

        private static IEnumerable<FieldInfo> FilterFields(ICollection<FieldInfo> fields)
        {
            var fieldsToRemove = new OdbList<FieldInfo>(fields.Count);

            foreach (var fieldInfo in fields)
            {
                if (fieldInfo.IsNotSerialized)
                    fieldsToRemove.Add(fieldInfo);
                else if (fieldInfo.FieldType == typeof (IntPtr))
                    fieldsToRemove.Add(fieldInfo);
                else if (fieldInfo.FieldType == typeof (UIntPtr))
                    fieldsToRemove.Add(fieldInfo);
                else if (fieldInfo.FieldType == typeof (void*))
                    fieldsToRemove.Add(fieldInfo);
                else if (fieldInfo.FieldType == typeof (Pointer))
                    fieldsToRemove.Add(fieldInfo);
                else if (fieldInfo.FieldType.FullName.StartsWith("System.Reflection.CerHashtable"))
                    fieldsToRemove.Add(fieldInfo);
                else if (fieldInfo.Name.StartsWith("this$"))
                    fieldsToRemove.Add(fieldInfo);
                else
                {
                    var oattr = fieldInfo.GetCustomAttributes(true);
                    var isNonPersistent = oattr.OfType<NonPersistentAttribute>().Any();

                    if (isNonPersistent)
                        fieldsToRemove.Add(fieldInfo);
                }
            }

            foreach (var item in fieldsToRemove)
                fields.Remove(item);

            return fields;
        }

        private static byte GetClassCategory(Type type)
        {
            return GetClassCategory(OdbClassUtil.GetFullName(type));
        }

        private static byte GetClassCategory(string fullClassName)
        {
            return SystemClasses.ContainsKey(fullClassName)
                       ? ClassInfo.CategorySystemClass
                       : ClassInfo.CategoryUserClass;
        }

        /// <param name="type"> The class to instrospect </param>
        /// <param name="recursive"> If true, goes does the hierarchy to try to analyse all classes </param>
        /// <param name="classInfoList"> map with classname that are being introspected, to avoid recursive calls </param>
        private static ClassInfoList InternalIntrospect(Type type, bool recursive, ClassInfoList classInfoList)
        {
            if (classInfoList != null)
            {
                var fullClassName = OdbClassUtil.GetFullName(type);
                var existingClassInfo = classInfoList.GetClassInfoBy(fullClassName);

                if (existingClassInfo != null)
                    return classInfoList;
            }

            var classInfo = new ClassInfo(type) {ClassCategory = GetClassCategory(type)};

            if (classInfoList == null)
                classInfoList = new ClassInfoList(classInfo);
            else
                classInfoList.AddClassInfo(classInfo);

            var fields = GetAllFieldsFrom(type);
            var attributes = new OdbList<ClassAttributeInfo>(fields.Count);

            for (var i = 0; i < fields.Count; i++)
            {
                var field = fields[i];

                ClassInfo classInfoByName;

                if (OdbType.GetFromClass(field.FieldType).IsNative())
                {
                    classInfoByName = null;
                }
                else
                {
                    if (recursive)
                    {
                        classInfoList = InternalIntrospect(field.FieldType, true, classInfoList);
                        classInfoByName = classInfoList.GetClassInfoBy(OdbClassUtil.GetFullName(field.FieldType));
                    }
                    else
                        classInfoByName = new ClassInfo(OdbClassUtil.GetFullName(field.FieldType));
                }

                attributes.Add(new ClassAttributeInfo((i + 1), field.Name, field.FieldType,
                                                      OdbClassUtil.GetFullName(field.FieldType), classInfoByName));
            }

            classInfo.Attributes = attributes;
            classInfo.MaxAttributeId = fields.Count;

            return classInfoList;
        }
    }
}