using System;
using System.Collections.Generic;
using System.Text;
using NDatabase.Tool.Wrappers.Map;

namespace NDatabase.Odb.Core.Layers.Layer2.Meta
{
    /// <summary>
    ///   A simple list to contain some class infos.
    /// </summary>
    /// <remarks>
    ///   A simple list to contain some class infos. <pre>It used by ClassIntropector.introspect to return all the class info detected by introspecting a class.
    ///                                                For example, if we have a class Class1 that has a field of type Class2. And Class2 has a field of type Class3.
    ///                                                Introspecting Class1 return a ClassInfoList with the classes Class1, Class2, Class3. Class1 being the main class info</pre>
    /// </remarks>
    /// <author>osmadja</author>
    [Serializable]
    public class ClassInfoList
    {
        /// <summary>
        ///   key=ClassInfoName,value=ClassInfo
        /// </summary>
        private readonly IDictionary<string, ClassInfo> _classInfos;

        private ClassInfo _mainClassInfo;

        public ClassInfoList()
        {
        }

        public ClassInfoList(ClassInfo mainClassInfo)
        {
            _classInfos = new OdbHashMap<string, ClassInfo>();
            _classInfos[mainClassInfo.GetFullClassName()] = mainClassInfo;
            _mainClassInfo = mainClassInfo;
        }

        public virtual ClassInfo GetMainClassInfo()
        {
            return _mainClassInfo;
        }

        public virtual void AddClassInfo(ClassInfo classInfo)
        {
            _classInfos[classInfo.GetFullClassName()] = classInfo;
        }

        public virtual ICollection<ClassInfo> GetClassInfos()
        {
            return _classInfos.Values;
        }

        public virtual bool HasClassInfos()
        {
            return _classInfos.Count != 0;
        }

        /// <param name="name"> </param>
        /// <returns> null if it does not exist </returns>
        public virtual ClassInfo GetClassInfoWithName(string name)
        {
            ClassInfo classInfo;
            _classInfos.TryGetValue(name, out classInfo);
            return classInfo;
        }

        public override string ToString()
        {
            var buffer = new StringBuilder();
            buffer.Append(_classInfos.Count).Append(" - ").Append(_classInfos.Keys);
            return buffer.ToString();
        }

        public virtual void SetMainClassInfo(ClassInfo classInfo)
        {
            _mainClassInfo = classInfo;
        }
    }
}