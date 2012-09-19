using System;
using System.Collections.Generic;
using System.Reflection;
using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Tool.Wrappers.List;

namespace NDatabase.Odb.Core.Layers.Layer1.Introspector
{
    public interface IClassIntrospector
    {
        void Reset();

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

        IOdbList<FieldInfo> GetAllFields(string fullClassName);

        ClassInfoList Introspect(string fullClassName, bool recursive);

        object NewInstanceOf(Type clazz);

        FieldInfo GetField(Type clazz, string fieldName);
    }
}
