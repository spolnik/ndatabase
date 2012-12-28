using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using NDatabase.Odb.Core.Layers.Layer2.Instance;
using NDatabase.Tool.Wrappers;
using NDatabase.Tool.Wrappers.List;
using NDatabase.Tool.Wrappers.Map;
using NDatabase2.Odb;
using NDatabase2.Odb.Core;

namespace NDatabase.Odb.Core.Layers.Layer2.Meta
{
    public interface IMetaModel
    {
        IEnumerable<IClassInfo> GetSchemaClasses();
        int GetNumberOfClasses();
    }

    /// <summary>
    ///   The database meta-model
    /// </summary>
    internal sealed class MetaModel : IMetaModel
    {
        /// <summary>
        ///   A simple list to hold all class infos.
        /// </summary>
        /// <remarks>
        ///   A simple list to hold all class infos. It is redundant with the maps, but in some cases, we need sequential access to classes :-(
        /// </remarks>
        private readonly IList<ClassInfo> _allClassInfos;

        /// <summary>
        ///   A list of changed classes - that must be persisted back when commit is done
        /// </summary>
        private readonly OdbHashMap<ClassInfo, ClassInfo> _changedClasses;

        private readonly IDictionary<OID, ClassInfo> _rapidAccessForClassesByOid;

        /// <summary>
        ///   to identify if meta model has changed
        /// </summary>
        private bool _hasChanged;

        private IDictionary<string, ClassInfo> _rapidAccessForSystemClassesByName;

        /// <summary>
        ///   A hash map to speed up the access of classinfo by full class name
        /// </summary>
        private IDictionary<string, ClassInfo> _rapidAccessForUserClassesByName;

        public MetaModel()
        {
            _rapidAccessForUserClassesByName = new OdbHashMap<string, ClassInfo>(10);
            _rapidAccessForSystemClassesByName = new OdbHashMap<string, ClassInfo>(10);
            _rapidAccessForClassesByOid = new OdbHashMap<OID, ClassInfo>(10);
            _allClassInfos = new List<ClassInfo>();
            _changedClasses = new OdbHashMap<ClassInfo, ClassInfo>();
        }

        public void AddClass(ClassInfo classInfo)
        {
            var fullClassName = classInfo.FullClassName;

            if (classInfo.IsSystemClass())
                _rapidAccessForSystemClassesByName.Add(fullClassName, classInfo);
            else
                _rapidAccessForUserClassesByName.Add(fullClassName, classInfo);

            _rapidAccessForClassesByOid.Add(classInfo.ClassInfoId, classInfo);
            _allClassInfos.Add(classInfo);
        }

        public void AddClasses(ClassInfoList ciList)
        {
            foreach (var classInfo in ciList.GetClassInfos())
                AddClass(classInfo);
        }

        public bool ExistClass(Type type)
        {
            var fullName = OdbClassUtil.GetFullName(type);

            // Check if it is a system class
            var exist = _rapidAccessForSystemClassesByName.ContainsKey(fullName);
            if (exist)
                return true;
            // Check if it is user class
            exist = _rapidAccessForUserClassesByName.ContainsKey(fullName);
            return exist;
        }

        public override string ToString()
        {
            return string.Format("{0}/{1}", _rapidAccessForUserClassesByName.Values, _rapidAccessForSystemClassesByName.Values);
        }

        public IEnumerable<ClassInfo> GetAllClasses()
        {
            return _allClassInfos;
        }

        public IEnumerable<ClassInfo> GetUserClasses()
        {
            return _rapidAccessForUserClassesByName.Values;
        }

        public IEnumerable<IClassInfo> GetSchemaClasses()
        {
            return _rapidAccessForUserClassesByName.Values;
        }

        public IEnumerable<ClassInfo> GetSystemClasses()
        {
            return _rapidAccessForSystemClassesByName.Values;
        }

        public int GetNumberOfClasses()
        {
            return _allClassInfos.Count;
        }

        /// <summary>
        ///   Gets the class info from the OID.
        /// </summary>
        /// <remarks>
        ///   Gets the class info from the OID.
        /// </remarks>
        /// <param name="id"> </param>
        /// <returns> the class info with the OID </returns>
        public ClassInfo GetClassInfoFromId(OID id)
        {
            return _rapidAccessForClassesByOid[id];
        }

        public ClassInfo GetClassInfo(Type type, bool throwExceptionIfDoesNotExist)
        {
            var fullClassName = OdbClassUtil.GetFullName(type);
            return GetClassInfo(fullClassName, throwExceptionIfDoesNotExist);
        }

        public ClassInfo GetClassInfo(string fullClassName, bool throwExceptionIfDoesNotExist)
        {
            // Check if it is a system class
            ClassInfo classInfo;
            _rapidAccessForSystemClassesByName.TryGetValue(fullClassName, out classInfo);

            if (classInfo != null)
                return classInfo;

            // Check if it is user class
            _rapidAccessForUserClassesByName.TryGetValue(fullClassName, out classInfo);
            if (classInfo != null)
                return classInfo;

            if (throwExceptionIfDoesNotExist)
                throw new OdbRuntimeException(
                    NDatabaseError.MetaModelClassNameDoesNotExist.AddParameter(fullClassName));

            return null;
        }

        /// <returns> The Last class info </returns>
        public ClassInfo GetLastClassInfo()
        {
            return _allClassInfos[_allClassInfos.Count - 1];
        }

        /// <param name="index"> The index of the class info to get </param>
        /// <returns> The class info at the specified index </returns>
        public ClassInfo GetClassInfo(int index)
        {
            return _allClassInfos[index];
        }

        public void Clear()
        {
            _rapidAccessForSystemClassesByName.Clear();
            _rapidAccessForUserClassesByName.Clear();
            _rapidAccessForSystemClassesByName = null;
            _rapidAccessForUserClassesByName = null;
            _allClassInfos.Clear();
        }

        public bool HasChanged()
        {
            return _hasChanged;
        }

        public IEnumerable<ClassInfo> GetChangedClassInfo()
        {
            IOdbList<ClassInfo> list = new OdbList<ClassInfo>();
            list.AddAll(_changedClasses.Keys);

            return new ReadOnlyCollection<ClassInfo>(list);
        }

        public void ResetChangedClasses()
        {
            _changedClasses.Clear();
            _hasChanged = false;
        }

        /// <summary>
        ///   Saves the fact that something has changed in the class (number of objects or last object oid)
        /// </summary>
        public void AddChangedClass(ClassInfo classInfo)
        {
            _changedClasses[classInfo] = classInfo;
            _hasChanged = true;
        }

        /// <summary>
        ///   Gets all the persistent classes that are subclasses or equal to the parameter class
        /// </summary>
        /// <returns> The list of class info of persistent classes that are subclasses or equal to the class </returns>
        public IOdbList<ClassInfo> GetPersistentSubclassesOf(Type type)
        {
            IOdbList<ClassInfo> result = new OdbList<ClassInfo>();
            var classNames = _rapidAccessForUserClassesByName.Keys.GetEnumerator();

            var fullClassName = OdbClassUtil.GetFullName(type);
            var theClass = type;
            while (classNames.MoveNext())
            {
                var oneClassName = classNames.Current;
                Debug.Assert(oneClassName != null);

                if (oneClassName.Equals(fullClassName))
                {
                    result.Add(GetClassInfo(oneClassName, true));
                }
                else
                {
                    var oneClass = OdbClassPool.GetClass(oneClassName);
                    if (theClass.IsAssignableFrom(oneClass))
                        result.Add(GetClassInfo(oneClassName, true));
                }
            }

            return result;
        }
    }
}
