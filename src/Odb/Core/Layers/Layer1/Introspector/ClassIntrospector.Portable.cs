using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NDatabase.Exceptions;
using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Tool;
using NDatabase.Tool.Wrappers;
using NDatabase.TypeResolution;

namespace NDatabase.Odb.Core.Layers.Layer1.Introspector
{
    /// <summary>
    ///   The Class Introspector is used to introspect classes.
    /// </summary>
    /// <remarks>
    ///   The Class Introspector is used to introspect classes. 
    ///   It uses Reflection to extract class information. 
    ///   It transforms a native Class into a ClassInfo (a meta representation of the class) 
    ///   that contains all informations about the class.
    /// </remarks>
    internal static class ClassIntrospector
    {
        private static readonly Dictionary<Type, IList<FieldInfo>> Fields =
            new Dictionary<Type, IList<FieldInfo>>();

        /// <summary>
        /// </summary>
        /// <param name="type"> The class to introspect </param>
        /// <param name="recursive"> If true, goes does the hierarchy to try to analyze all classes </param>
        /// <returns> </returns>
        public static ClassInfoList Introspect(Type type, bool recursive)
        {
            return InternalIntrospect(type, recursive, null);
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
        ///   The result will be used to check meta model compatibility between the meta model that is currently persisted in the database and the meta 
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

        public static ClassInfoList Introspect(String fullClassName)
        {
            return Introspect(TypeResolutionUtils.ResolveType(fullClassName), true);
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
            var type = TypeResolutionUtils.ResolveType(fullClassName);
            var classInfo = new ClassInfo(type);

            var fields = GetAllFieldsFrom(type);
            var attributes = new OdbList<ClassAttributeInfo>(fields.Count);

            var maxAttributeId = existingClassInfo.MaxAttributeId;
            foreach (var fieldInfo in fields)
            {
                // Gets the attribute id from the existing class info
                var attributeId = existingClassInfo.GetAttributeId(fieldInfo.Name);
                if (attributeId == - 1)
                {
                    maxAttributeId++;
                    // The attribute with field.getName() does not exist in existing class info
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
                if (fieldInfo.FieldType == typeof (IntPtr))
                    fieldsToRemove.Add(fieldInfo);
                else if (fieldInfo.FieldType == typeof (UIntPtr))
                    fieldsToRemove.Add(fieldInfo);
                else if (fieldInfo.FieldType == typeof (void*))
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

        /// <param name="type"> The class to introspect </param>
        /// <param name="recursive"> If true, goes does the hierarchy to try to analyze all classes </param>
        /// <param name="classInfoList"> map with class name that are being introspected, to avoid recursive calls </param>
        private static ClassInfoList InternalIntrospect(Type type, bool recursive, ClassInfoList classInfoList)
        {
            if (classInfoList != null)
            {
                var existingClassInfo = classInfoList.GetClassInfoBy(type);

                if (existingClassInfo != null)
                    return classInfoList;
            }

            var classInfo = new ClassInfo(type);

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
                        classInfoByName = classInfoList.GetClassInfoBy(field.FieldType);
                    }
                    else
                        classInfoByName = new ClassInfo(field.FieldType);
                }

                attributes.Add(new ClassAttributeInfo((i + 1), field.Name, field.FieldType,
                                                      OdbClassUtil.GetFullName(field.FieldType), classInfoByName));
            }

            classInfo.Attributes = attributes;
            classInfo.MaxAttributeId = fields.Count;

            return classInfoList;
        }

        public static Object NewInstanceOf(Type clazz)
        {
            // Checks if exist a default constructor - with no parameters
            var constructor = clazz.GetConstructor(Type.EmptyTypes);

            if (constructor != null)
                return Activator.CreateInstance(clazz);

            // else take the constructer with the smaller number of parameters
            // and call it will null values
            // @TODO Put this info in cache !
            if (OdbConfiguration.IsLoggingEnabled())
            {
                DLogger.Debug(
                    string.Format(
                        "{0} does not have default constructor! using a 'with parameter' constructor will null values",
                        clazz));
            }

            var constructors =
                clazz.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            if (clazz.IsInterface)
            {
                //@TODO This is not a good solution to manage interface
                return null;
            }

            if (constructors.Length == 0)
                throw new OdbRuntimeException(
                    NDatabaseError.InternalError.AddParameter("Class without constructor: " + clazz.AssemblyQualifiedName));

            const int numberOfParameters = 1000;
            var bestConstructorIndex = 0;

            for (var i = 0; i < constructors.Length; i++)
            {
                if (constructors[i].GetParameters().Length < numberOfParameters)
                    bestConstructorIndex = i;
            }
            constructor = constructors[bestConstructorIndex];

            var nulls = new Object[constructor.GetParameters().Length];
            for (var i = 0; i < nulls.Length; i++)
            {
                if (constructor.GetParameters()[i].ParameterType == typeof (Int32))
                {
                    nulls[i] = 0;
                }
                else if (constructor.GetParameters()[i].ParameterType == typeof (Int64))
                {
                    nulls[i] = 0;
                }
                else if (constructor.GetParameters()[i].ParameterType == typeof (Int16))
                {
                    nulls[i] = default(short);
                }
                else if (constructor.GetParameters()[i].ParameterType == typeof (sbyte))
                {
                    nulls[i] = default(sbyte);
                }
                else if (constructor.GetParameters()[i].ParameterType == typeof (Single))
                {
                    nulls[i] = default(Single);
                }
                else if (constructor.GetParameters()[i].ParameterType == typeof (double))
                {
                    nulls[i] = default(double);
                }
                else
                {
                    nulls[i] = null;
                }
            }

            Object objectRenamed;

            try
            {
                objectRenamed = constructor.Invoke(nulls);
            }
            catch (Exception)
            {
                throw new OdbRuntimeException(
                    NDatabaseError.InternalError.AddParameter("No Nullable constructor: " + clazz.AssemblyQualifiedName));
            }

            return objectRenamed;
        }
    }
}
