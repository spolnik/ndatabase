using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NDatabase.Odb.Core.Layers.Layer2.Instance;
using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Tool.Wrappers.List;

namespace NDatabase.Odb.Core.Layers.Layer1.Introspector
{
    public interface IClassIntrospector : ITwoPhaseInit
    {
        void Reset();

        void AddInstanciationHelper(Type clazz, IInstantiationHelper helper);

        void AddParameterHelper(Type clazz, IParameterHelper helper);

        void AddFullInstanciationHelper(Type clazz, IFullInstantiationHelper helper);

        void AddInstantiationHelper(string clazz, IInstantiationHelper helper);

        void AddParameterHelper(string clazz, IParameterHelper helper);

        void AddFullInstantiationHelper(string clazz, IFullInstantiationHelper helper);

        void RemoveInstantiationHelper(Type clazz);

        void RemoveInstantiationHelper(string canonicalName);

        void RemoveParameterHelper(Type clazz);

        void RemoveParameterHelper(string canonicalName);

        void RemoveFullInstantiationHelper(Type clazz);

        void RemoveFullInstantiationHelper(string canonicalName);

        /// <summary>
        ///   introspect a list of classes
        /// </summary>
        /// <param name="classInfos"> </param>
        /// <returns> A map where the key is the class name and the key is the ClassInfo: the class meta representation </returns>
        IDictionary<string, ClassInfo> Instrospect(IOdbList<ClassInfo> classInfos);

        /// <param name="clazz"> The class to instrospect </param>
        /// <param name="recursive"> If true, goes does the hierarchy to try to analyse all classes </param>
        /// <returns> The list of class info detected while introspecting the class </returns>
        ClassInfoList Introspect(Type clazz, bool recursive);

        /// <summary>
        ///   Builds a class info from a class and an existing class info <pre>The existing class info is used to make sure that fields with the same name will have
        ///                                                                 the same id</pre>
        /// </summary>
        /// <param name="fullClassName"> The name of the class to get info </param>
        /// <param name="existingClassInfo"> </param>
        /// <returns> A ClassInfo - a meta representation of the class </returns>
        ClassInfo GetClassInfo(string fullClassName, ClassInfo existingClassInfo);

        /// <param name="fullClassName"> </param>
        /// <param name="includingThis"> </param>
        /// <returns> The list of super classes </returns>
        IList GetSuperClasses(string fullClassName, bool includingThis);

        IOdbList<FieldInfo> GetAllFields(string fullClassName);

        IOdbList<FieldInfo> RemoveUnnecessaryFields(IOdbList<FieldInfo> fields);

        ClassInfoList Introspect(string fullClassName, bool recursive);

        ConstructorInfo GetConstructorOf(string fullClassName);

        object NewFullInstanceOf(Type clazz, NonNativeObjectInfo nnoi);

        object NewInstanceOf(Type clazz);

        byte GetClassCategory(string fullClassName);

        bool IsSystemClass(string fullClassName);

        FieldInfo GetField(Type clazz, string fieldName);
    }
}
