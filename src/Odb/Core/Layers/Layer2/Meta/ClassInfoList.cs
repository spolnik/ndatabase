using System.Collections.Generic;
using System.Text;
using NDatabase.Tool.Wrappers.Map;

namespace NDatabase.Odb.Core.Layers.Layer2.Meta
{
    /// <summary>
    ///   A simple list to contain some class infos.
    /// </summary>
    /// <remarks>
    ///   A simple list to contain some class infos. 
    ///  <pre>It used by ClassIntropector.introspect to return all the class info detected by introspecting a class.
    ///       For example, if we have a class Class1 that has a field of type Class2. And Class2 has a field of type Class3.
    ///       Introspecting Class1 return a ClassInfoList with the classes Class1, Class2, Class3. Class1 being the main class info</pre>
    /// </remarks>
    internal sealed class ClassInfoList
    {
        /// <summary>
        ///   key=ClassInfoName,value=ClassInfo
        /// </summary>
        private readonly IDictionary<string, ClassInfo> _classInfos;

        private readonly ClassInfo _mainClassInfo;

        public ClassInfoList(ClassInfo mainClassInfo)
        {
            _classInfos = new OdbHashMap<string, ClassInfo>();
            _classInfos[mainClassInfo.FullClassName] = mainClassInfo;
            _mainClassInfo = mainClassInfo;
        }

        public ClassInfo GetMainClassInfo()
        {
            return _mainClassInfo;
        }

        public void AddClassInfo(ClassInfo classInfo)
        {
            _classInfos[classInfo.FullClassName] = classInfo;
        }

        public ICollection<ClassInfo> GetClassInfos()
        {
            return _classInfos.Values;
        }

        /// <returns> null if it does not exist </returns>
        public ClassInfo GetClassInfoBy(string name)
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
    }
}
