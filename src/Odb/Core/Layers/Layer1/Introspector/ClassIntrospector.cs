using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using NDatabase.Btree;
using NDatabase.Btree.Impl;
using NDatabase.Odb.Core.BTree;
using NDatabase.Odb.Core.Layers.Layer2.Instance;
using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Odb.Core.Oid;
using NDatabase.Tool.Wrappers;
using NDatabase.Tool.Wrappers.List;
using NDatabase.Tool.Wrappers.Map;

namespace NDatabase.Odb.Core.Layers.Layer1.Introspector
{
    /// <summary>
    ///   The ClassIntrospector is used to introspect classes.
    /// </summary>
    /// <remarks>
    ///   The ClassIntrospector is used to introspect classes. It uses Reflection to extract class information. It transforms a native Class into a ClassInfo (a meta representation of the class) that contains all informations about the class.
    /// </remarks>
    public sealed class ClassIntrospector : IClassIntrospector
    {
        public static readonly IClassIntrospector Instance = new ClassIntrospector();

        private readonly IDictionary<string, IOdbList<FieldInfo>> _fields =
            new OdbHashMap<string, IOdbList<FieldInfo>>();

        private readonly IDictionary<string, Type> _systemClasses = new OdbHashMap<string, Type>();

        #region IClassIntrospector Members

        /// <summary>
        /// </summary>
        /// <param name="clazz"> The class to instrospect </param>
        /// <param name="recursive"> If true, goes does the hierarchy to try to analyse all classes </param>
        /// <returns> </returns>
        public ClassInfoList Introspect(Type clazz, bool recursive)
        {
            return InternalIntrospect(clazz, recursive, null);
        }

        /// <summary>
        ///   Builds a class info from a class and an existing class info <pre>The existing class info is used to make sure that fields with the same name will have
        ///                                                                 the same id</pre>
        /// </summary>
        /// <param name="fullClassName"> The name of the class to get info </param>
        /// <param name="existingClassInfo"> </param>
        /// <returns> A ClassInfo - a meta representation of the class </returns>
        private ClassInfo GetClassInfo(String fullClassName, ClassInfo existingClassInfo)
        {
            var classInfo = new ClassInfo(fullClassName) {ClassCategory = GetClassCategory(fullClassName)};

            var fields = GetAllFields(fullClassName);
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

        /// <summary>
        ///   Get The list of super classes
        /// </summary>
        /// <returns> The list of super classes </returns>
        private static IEnumerable<Type> GetSuperClasses(string fullClassName, bool includingThis)
        {
            IList<Type> result = new List<Type>();

            var clazz = OdbClassPool.GetClass(fullClassName);

            if (clazz == null)
                return result;

            if (includingThis)
                result.Add(clazz);

            var baseType = clazz.BaseType;

            while (baseType != null && baseType != typeof (Object))
            {
                result.Add(baseType);
                baseType = baseType.BaseType;
            }

            return result;
        }

        public FieldInfo GetField(Type type, string fieldName)
        {
            return type.GetField(fieldName);
        }

        public IOdbList<FieldInfo> GetAllFields(string fullClassName)
        {
            IOdbList<FieldInfo> result;
            _fields.TryGetValue(fullClassName, out result);

            if (result != null)
                return result;

            IDictionary attributesNames = new Hashtable();
            result = new OdbList<FieldInfo>(50);
            var classes = GetSuperClasses(fullClassName, true);

            foreach (var clazz1 in classes)
            {
                var superClassfields =
                    clazz1.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public |
                                     BindingFlags.DeclaredOnly | BindingFlags.Static);
                foreach (var fieldInfo in superClassfields)
                {
                    // Only adds the attribute if it does not exist one with same name
                    if (attributesNames[fieldInfo.Name] == null)
                    {
                        result.Add(fieldInfo);
                        attributesNames[fieldInfo.Name] = fieldInfo.Name;
                    }
                }
            }

            result = RemoveUnnecessaryFields(result);
            _fields[fullClassName] = result;
            attributesNames.Clear();

            return result;
        }

        private static IOdbList<FieldInfo> RemoveUnnecessaryFields(IOdbList<FieldInfo> fields)
        {
            IOdbList<FieldInfo> fieldsToRemove = new OdbList<FieldInfo>(fields.Count);

            // Remove static fields
            foreach (var fieldInfo in fields)
            {
                // by osmadja
                if (fieldInfo.IsNotSerialized || fieldInfo.IsStatic)
                    fieldsToRemove.Add(fieldInfo);

                //by cristi
                if (fieldInfo.FieldType == typeof (IntPtr))
                    fieldsToRemove.Add(fieldInfo);

                var oattr = fieldInfo.GetCustomAttributes(true);
                var isNonPersistent = oattr.OfType<NonPersistentAttribute>().Any();

                if (isNonPersistent || fieldInfo.IsStatic)
                    fieldsToRemove.Add(fieldInfo);

                // Remove inner class fields
                if (fieldInfo.Name.StartsWith("this$"))
                    fieldsToRemove.Add(fieldInfo);
            }

            fields.RemoveAll(fieldsToRemove);
            return fields;
        }

        /// <summary>
        ///   introspect a list of classes This method return the current meta model based on the classes that currently exist in the execution classpath.
        /// </summary>
        /// <remarks>
        ///   introspect a list of classes This method return the current meta model based on the classes that currently exist in the execution classpath. The result will be used to check meta model compatiblity between the meta model that is currently persisted in the database and the meta model currently executing in JVM. This is used b the automatic meta model refactoring
        /// </remarks>
        /// <returns> </returns>
        /// <returns> A map where the key is the class name and the key is the ClassInfo: the class meta representation </returns>
        public IDictionary<string, ClassInfo> Instrospect(IOdbList<ClassInfo> classInfos)
        {
            IDictionary<string, ClassInfo> classInfoSet = new Dictionary<string, ClassInfo>();
            
            // re introspect classes
            foreach (var persistedClassInfo in classInfos)
            {
                var currentClassInfo = GetClassInfo(persistedClassInfo.FullClassName, persistedClassInfo);

                classInfoSet.Add(currentClassInfo.FullClassName, currentClassInfo);
            }

            return classInfoSet;
        }

        public ClassInfoList Introspect(String fullClassName, bool recursive)
        {
            return Introspect(OdbClassPool.GetClass(fullClassName), true);
        }

        public void Reset()
        {
            _fields.Clear();
        }

        public Object NewInstanceOf(Type clazz)
        {
            return FormatterServices.GetUninitializedObject(clazz);
        }

        private byte GetClassCategory(Type type)
        {
            return GetClassCategory(OdbClassUtil.GetFullName(type));
        }

        private byte GetClassCategory(string fullClassName)
        {
            if ((_systemClasses.Count == 0))
                FillSystemClasses();

            if (_systemClasses.ContainsKey(fullClassName))
                return ClassInfo.CategorySystemClass;
            return ClassInfo.CategoryUserClass;
        }

        #endregion

        /// <param name="type"> The class to instrospect </param>
        /// <param name="recursive"> If true, goes does the hierarchy to try to analyse all classes </param>
        /// <param name="classInfoList"> map with classname that are being introspected, to avoid recursive calls </param>
        private ClassInfoList InternalIntrospect(Type type, bool recursive, ClassInfoList classInfoList)
        {
            var fullClassName = OdbClassUtil.GetFullName(type);

            if (classInfoList != null)
            {
                var existingClassInfo = classInfoList.GetClassInfoWithName(fullClassName);
                if (existingClassInfo != null)
                    return classInfoList;
            }

            var classInfo = new ClassInfo(type) {ClassCategory = GetClassCategory(type)};

            if (classInfoList == null)
                classInfoList = new ClassInfoList(classInfo);
            else
                classInfoList.AddClassInfo(classInfo);

            var fields = GetAllFields(fullClassName);
            IOdbList<ClassAttributeInfo> attributes = new OdbList<ClassAttributeInfo>(fields.Count);

            for (var i = 0; i < fields.Count; i++)
            {
                var field = fields[i];
                
                ClassInfo classInfoWithName;

                if (!OdbType.GetFromClass(field.FieldType).IsNative())
                {
                    if (recursive)
                    {
                        classInfoList = InternalIntrospect(field.FieldType, true, classInfoList);
                        classInfoWithName = classInfoList.GetClassInfoWithName(OdbClassUtil.GetFullName(field.FieldType));
                    }
                    else
                    {
                        classInfoWithName = new ClassInfo(OdbClassUtil.GetFullName(field.FieldType));
                    }
                }
                else
                    classInfoWithName = null;
                attributes.Add(new ClassAttributeInfo((i + 1), field.Name, field.FieldType,
                                                      OdbClassUtil.GetFullName(field.FieldType), classInfoWithName));
            }
            classInfo.Attributes = attributes;
            classInfo.MaxAttributeId = fields.Count;
            return classInfoList;
        }

        private void FillSystemClasses()
        {
            _systemClasses.Add(typeof(ClassInfoIndex).FullName, typeof (ClassInfoIndex));
            _systemClasses.Add(typeof(OID).FullName, typeof (OID));
            _systemClasses.Add(typeof(ObjectOID).FullName, typeof (ObjectOID));
            _systemClasses.Add(typeof(ClassOID).FullName, typeof (ClassOID));
            _systemClasses.Add(typeof(OdbBtreeNodeSingle).FullName, typeof (OdbBtreeNodeSingle));
            _systemClasses.Add(typeof(OdbBtreeNodeMultiple).FullName, typeof (OdbBtreeNodeMultiple));
            _systemClasses.Add(typeof(OdbBtreeSingle).FullName, typeof (OdbBtreeSingle));
            _systemClasses.Add(typeof(IBTree).FullName, typeof (IBTree));
            _systemClasses.Add(typeof(IBTreeNodeOneValuePerKey).FullName, typeof (IBTreeNodeOneValuePerKey));
            _systemClasses.Add(typeof(IKeyAndValue).FullName, typeof (IKeyAndValue));
            _systemClasses.Add(typeof(KeyAndValue).FullName, typeof (KeyAndValue));
        }
    }
}
