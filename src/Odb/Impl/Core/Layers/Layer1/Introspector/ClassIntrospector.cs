using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using NDatabase.Btree;
using NDatabase.Btree.Impl;
using NDatabase.Odb.Core;
using NDatabase.Odb.Core.Layers.Layer1.Introspector;
using NDatabase.Odb.Core.Layers.Layer2.Instance;
using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Odb.Impl.Core.Btree;
using NDatabase.Odb.Impl.Core.Oid;
using NDatabase.Tool;
using NDatabase.Tool.Wrappers;
using NDatabase.Tool.Wrappers.List;
using NDatabase.Tool.Wrappers.Map;

namespace NDatabase.Odb.Impl.Core.Layers.Layer1.Introspector
{
    /// <summary>
    ///   The ClassIntrospector is used to introspect classes.
    /// </summary>
    /// <remarks>
    ///   The ClassIntrospector is used to introspect classes. It uses Reflection to extract class information. It transforms a native Class into a ClassInfo (a meta representation of the class) that contains all informations about the class.
    /// </remarks>
    public sealed class ClassIntrospector : IClassIntrospector
    {
        private const string LogId = "ClassIntrospector";

        private readonly IDictionary<string, IOdbList<FieldInfo>> _fields =
            new OdbHashMap<string, IOdbList<FieldInfo>>();

        private readonly IDictionary<String, IFullInstantiationHelper> _fullInstantiationHelpers =
            new OdbHashMap<String, IFullInstantiationHelper>();

        private readonly IDictionary<String, IInstantiationHelper> _instantiationHelpers =
            new OdbHashMap<String, IInstantiationHelper>();

        private readonly IDictionary<String, IParameterHelper> _parameterHelpers =
            new OdbHashMap<String, IParameterHelper>();

        private readonly IDictionary<string, Type> _systemClasses = new OdbHashMap<string, Type>();

        private IClassPool _classPool;

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

        public void AddInstanciationHelper(Type clazz, IInstantiationHelper helper)
        {
            AddInstantiationHelper(clazz.FullName, helper);
        }

        public void AddParameterHelper(Type clazz, IParameterHelper helper)
        {
            AddParameterHelper(clazz.FullName, helper);
        }

        public void AddFullInstanciationHelper(Type clazz, IFullInstantiationHelper helper)
        {
            AddFullInstantiationHelper(clazz.FullName, helper);
        }

        public void AddInstantiationHelper(string clazz, IInstantiationHelper helper)
        {
            _instantiationHelpers.Add(clazz, helper);
        }

        public void AddParameterHelper(string clazz, IParameterHelper helper)
        {
            _parameterHelpers.Add(clazz, helper);
        }

        public void AddFullInstantiationHelper(string clazz, IFullInstantiationHelper helper)
        {
            _fullInstantiationHelpers.Add(clazz, helper);
        }

        public void RemoveInstantiationHelper(Type clazz)
        {
            RemoveInstantiationHelper(clazz.FullName);
        }

        public void RemoveInstantiationHelper(string canonicalName)
        {
            _instantiationHelpers.Remove(canonicalName);
        }

        public void RemoveParameterHelper(Type clazz)
        {
            RemoveParameterHelper(clazz.FullName);
        }

        public void RemoveParameterHelper(string canonicalName)
        {
            _parameterHelpers.Remove(canonicalName);
        }

        public void RemoveFullInstantiationHelper(Type clazz)
        {
            RemoveFullInstantiationHelper(clazz.FullName);
        }

        public void RemoveFullInstantiationHelper(string canonicalName)
        {
            _fullInstantiationHelpers.Remove(canonicalName);
        }

        /// <summary>
        ///   Builds a class info from a class and an existing class info <pre>The existing class info is used to make sure that fields with the same name will have
        ///                                                                 the same id</pre>
        /// </summary>
        /// <param name="fullClassName"> The name of the class to get info </param>
        /// <param name="existingClassInfo"> </param>
        /// <returns> A ClassInfo - a meta representation of the class </returns>
        public ClassInfo GetClassInfo(String fullClassName, ClassInfo existingClassInfo)
        {
            var classInfo = new ClassInfo(fullClassName);
            classInfo.SetClassCategory(GetClassCategory(fullClassName));

            var fields = GetAllFields(fullClassName);
            IOdbList<ClassAttributeInfo> attributes = new OdbArrayList<ClassAttributeInfo>(fields.Count);

            var maxAttributeId = existingClassInfo.GetMaxAttributeId();
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
                                         ? new ClassInfo(OdbClassUtil.GetFullName(fieldInfo.FieldType))
                                         : null;

                attributes.Add(new ClassAttributeInfo(attributeId, fieldInfo.Name, fieldInfo.FieldType,
                                                      OdbClassUtil.GetFullName(fieldInfo.FieldType), fieldClassInfo));
            }

            classInfo.SetAttributes(attributes);
            classInfo.SetMaxAttributeId(maxAttributeId);

            return classInfo;
        }

        /// <summary>
        /// </summary>
        /// <param name="fullClassName"> </param>
        /// <param name="includingThis"> </param>
        /// <returns> The list of super classes </returns>
        public IList GetSuperClasses(string fullClassName, bool includingThis)
        {
            IList result = new ArrayList();

            var clazz = _classPool.GetClass(fullClassName);

            if (clazz == null)
                return result;

            if (clazz.IsInterface)
            {
                //throw new ODBRuntimeException(clazz.getName() + " is an interface");
            }
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
            result = new OdbArrayList<FieldInfo>(50);
            var classes = GetSuperClasses(fullClassName, true);

            foreach (Type clazz1 in classes)
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

        public IOdbList<FieldInfo> RemoveUnnecessaryFields(IOdbList<FieldInfo> fields)
        {
            IOdbList<FieldInfo> fieldsToRemove = new OdbArrayList<FieldInfo>(fields.Count);

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
                var currentClassInfo = GetClassInfo(persistedClassInfo.GetFullClassName(), persistedClassInfo);

                classInfoSet.Add(currentClassInfo.GetFullClassName(), currentClassInfo);
            }

            return classInfoSet;
        }

        public ClassInfoList Introspect(String fullClassName, bool recursive)
        {
            return Introspect(_classPool.GetClass(fullClassName), true);
        }

        public ConstructorInfo GetConstructorOf(String fullClassName)
        {
            var clazz = _classPool.GetClass(fullClassName);

            try
            {
                // Checks if exist a default constructor - with no parameters
                var constructor = clazz.GetConstructor(new Type[0]);

                return constructor;
            }
            catch (MethodAccessException)
            {
                // else take the constructer with the smaller number of parameters
                // and call it will null values
                // @TODO Put this inf oin cache !
                if (OdbConfiguration.IsDebugEnabled(LogId))
                {
                    DLogger.Debug(
                        string.Format(
                            "{0} does not have default constructor! using a 'with parameter' constructor will null values",
                            clazz));
                }

                var constructors = clazz.GetConstructors();
                const int numberOfParameters = 1000;
                var bestConstructorIndex = 0;

                for (var i = 0; i < constructors.Length; i++)
                {
                    //UPGRADE_TODO: The equivalent in .NET for method 'java.lang.reflect.Constructor.getParameterTypes' may return a different value. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
                    if (constructors[i].GetParameters().Length < numberOfParameters)
                        bestConstructorIndex = i;
                }

                var constructor = constructors[bestConstructorIndex];
                return constructor;
            }
        }

        public void Reset()
        {
            _fields.Clear();
            _fullInstantiationHelpers.Clear();
            _instantiationHelpers.Clear();
            _parameterHelpers.Clear();
        }

        /// <summary>
        ///   Two phase init method
        /// </summary>
        public void Init2()
        {
            _classPool = OdbConfiguration.GetCoreProvider().GetClassPool();
        }

        public object NewFullInstanceOf(Type clazz, NonNativeObjectInfo nnoi)
        {
            var className = clazz.FullName;
            
            var helper = _fullInstantiationHelpers[className];

            if (helper != null)
            {
                var o = helper.Instantiate(nnoi);
                if (o != null)
                    return o;
            }

            return null;
        }

        public Object NewInstanceOf(Type clazz)
        {
            var fullClassName = OdbClassUtil.GetFullName(clazz);

            var helper = _instantiationHelpers[fullClassName];
            if (helper != null)
            {
                var newInstance = helper.Instantiate();
                if (newInstance != null)
                    return newInstance;
            }

            var constructor = _classPool.GetConstrutor(fullClassName);

            if (constructor == null)
            {
                // Checks if exist a default constructor - with no parameters
                constructor = clazz.GetConstructor(Type.EmptyTypes);
                //UPGRADE_ISSUE: Method 'java.lang.reflect.AccessibleObject.setAccessible' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javalangreflectAccessibleObject'"
                //c by cristi
                //constructor.setAccessible(true);
                if (constructor != null)
                    _classPool.AddConstrutor(fullClassName, constructor);
            }
            if (constructor != null)
            {
                var obj = constructor.Invoke(new Object[0]);

                return obj;
            }

            if (clazz.IsValueType)
                return Activator.CreateInstance(clazz);

            // else take the constructer with the smaller number of parameters
            // and call it will null values
            // @TODO Put this info in cache !
            if (OdbConfiguration.IsDebugEnabled(LogId))
            {
                DLogger.Debug(
                    string.Format(
                        "{0} does not have default constructor! using a 'with parameter' constructor will null values",
                        clazz));
            }

            //TODO: do we really need that?
            var constructors =
                clazz.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public |
                                      BindingFlags.DeclaredOnly);

            if (clazz.IsInterface)
            {
                //@TODO This is not a good solution to manage interface
                return null;
            }

            if (constructors.Length == 0)
                throw new OdbRuntimeException(
                    NDatabaseError.ClassWithoutConstructor.AddParameter(clazz.AssemblyQualifiedName));
            
            if (OdbConfiguration.EnableEmptyConstructorCreation())
                return FormatterServices.GetUninitializedObject(clazz);

            throw new OdbRuntimeException(
                NDatabaseError.NoNullableConstructor.AddParameter(string.Format("[{0}]",
                                                                                DisplayUtility.ObjectArrayToString(
                                                                                    constructors.Last().GetParameters()))).
                    AddParameter(clazz.AssemblyQualifiedName));
        }

        // FIXME put the list of the classes elsewhere!

        public bool IsSystemClass(string fullClassName)
        {
            return _systemClasses.ContainsKey(fullClassName);
        }

        public byte GetClassCategory(string fullClassName)
        {
            if ((_systemClasses.Count == 0))
                FillSystemClasses();

            if (_systemClasses.ContainsKey(fullClassName))
                return ClassInfo.CategorySystemClass;
            return ClassInfo.CategoryUserClass;
        }

        #endregion

        /// <summary>
        /// </summary>
        /// <param name="clazz"> The class to instrospect </param>
        /// <param name="recursive"> If true, goes does the hierarchy to try to analyse all classes </param>
        /// <param name="classInfoList"> map with classname that are being introspected, to avoid recursive calls </param>
        /// <returns> </returns>
        private ClassInfoList InternalIntrospect(Type clazz, bool recursive, ClassInfoList classInfoList)
        {
            var fullClassName = OdbClassUtil.GetFullName(clazz);

            if (classInfoList != null)
            {
                var existingClassInfo = classInfoList.GetClassInfoWithName(fullClassName);
                if (existingClassInfo != null)
                    return classInfoList;
            }

            var classInfo = new ClassInfo(fullClassName);
            classInfo.SetClassCategory(GetClassCategory(fullClassName));

            if (classInfoList == null)
                classInfoList = new ClassInfoList(classInfo);
            else
                classInfoList.AddClassInfo(classInfo);

            // Field[] fields = clazz.getDeclaredFields();
            //UPGRADE_TODO: The equivalent in .NET for method 'java.lang.Class.getName' may return a different value. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
            //m by cristi
            var fields = GetAllFields(fullClassName);
            IOdbList<ClassAttributeInfo> attributes = new OdbArrayList<ClassAttributeInfo>(fields.Count);

            for (var i = 0; i < fields.Count; i++)
            {
                var field = fields[i];
                //Console.WriteLine("Field " + field.Name + " , type = " + field.FieldType);
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
            classInfo.SetAttributes(attributes);
            classInfo.SetMaxAttributeId(fields.Count);
            return classInfoList;
        }

        private void FillSystemClasses()
        {
            _systemClasses.Add(typeof(ClassInfoIndex).FullName, typeof (ClassInfoIndex));
            _systemClasses.Add(typeof(OID).FullName, typeof (OID));
            _systemClasses.Add(typeof(OdbObjectOID).FullName, typeof (OdbObjectOID));
            _systemClasses.Add(typeof(OdbClassOID).FullName, typeof (OdbClassOID));
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
