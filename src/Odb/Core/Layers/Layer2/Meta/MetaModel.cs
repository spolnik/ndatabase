using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using NDatabase.Odb.Core.Layers.Layer2.Instance;
using NDatabase.Tool.Wrappers.List;
using NDatabase.Tool.Wrappers.Map;

namespace NDatabase.Odb.Core.Layers.Layer2.Meta
{
    /// <summary>
    ///   The database meta-model
    /// </summary>
    /// <author>olivier s</author>
    [Serializable]
    public abstract class MetaModel
    {
        /// <summary>
        ///   A simple list to hold all class infos.
        /// </summary>
        /// <remarks>
        ///   A simple list to hold all class infos. It is redundant with the maps, but in some cases, we need sequential access to classes :-(
        /// </remarks>
        private readonly IOdbList<ClassInfo> _allClassInfos;

        [NonSerialized]
        private readonly IClassPool _classPool;

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

        protected MetaModel()
        {
            _classPool = OdbConfiguration.GetCoreProvider().GetClassPool();
            _rapidAccessForUserClassesByName = new OdbHashMap<string, ClassInfo>(10);
            _rapidAccessForSystemClassesByName = new OdbHashMap<string, ClassInfo>(10);
            _rapidAccessForClassesByOid = new OdbHashMap<OID, ClassInfo>(10);
            _allClassInfos = new OdbArrayList<ClassInfo>();
        }

        public void AddClass(ClassInfo classInfo)
        {
            if (classInfo.IsSystemClass())
                _rapidAccessForSystemClassesByName.Add(classInfo.GetFullClassName(), classInfo);
            else
                _rapidAccessForUserClassesByName.Add(classInfo.GetFullClassName(), classInfo);
            _rapidAccessForClassesByOid.Add(classInfo.GetId(), classInfo);
            _allClassInfos.Add(classInfo);
        }

        public virtual void AddClasses(ClassInfoList ciList)
        {
            foreach (var classInfo in ciList.GetClassInfos())
                AddClass(classInfo);
        }

        public virtual bool ExistClass(string fullClassName)
        {
            // Check if it is a system class
            var exist = _rapidAccessForSystemClassesByName.ContainsKey(fullClassName);
            if (exist)
                return true;
            // Check if it is user class
            exist = _rapidAccessForUserClassesByName.ContainsKey(fullClassName);
            return exist;
        }

        public override string ToString()
        {
            return _rapidAccessForUserClassesByName.Values + "/" + _rapidAccessForSystemClassesByName.Values;
        }

        public virtual IOdbList<ClassInfo> GetAllClasses()
        {
            return _allClassInfos;
        }

        public virtual ICollection<ClassInfo> GetUserClasses()
        {
            return _rapidAccessForUserClassesByName.Values;
        }

        public virtual ICollection<ClassInfo> GetSystemClasses()
        {
            return _rapidAccessForSystemClassesByName.Values;
        }

        public virtual int GetNumberOfClasses()
        {
            return _allClassInfos.Count;
        }

        public virtual int GetNumberOfUserClasses()
        {
            return _rapidAccessForUserClassesByName.Count;
        }

        public virtual int GetNumberOfSystemClasses()
        {
            return _rapidAccessForSystemClassesByName.Count;
        }

        /// <summary>
        ///   Gets the class info from the OID.
        /// </summary>
        /// <remarks>
        ///   Gets the class info from the OID.
        /// </remarks>
        /// <param name="id"> </param>
        /// <returns> the class info with the OID </returns>
        public virtual ClassInfo GetClassInfoFromId(OID id)
        {
            return _rapidAccessForClassesByOid[id];
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
                throw new OdbRuntimeException(NDatabaseError.MetaModelClassNameDoesNotExist.AddParameter(fullClassName));

            return null;
        }

        /// <returns> The Last class info </returns>
        public virtual ClassInfo GetLastClassInfo()
        {
            return _allClassInfos[_allClassInfos.Count - 1];
        }

        /// <summary>
        ///   This method is only used by the odb explorer.
        /// </summary>
        /// <remarks>
        ///   This method is only used by the odb explorer. So there is no too much problem with performance issue.
        /// </remarks>
        /// <param name="ci"> </param>
        /// <returns> The index of the class info </returns>
        public virtual int SlowGetUserClassInfoIndex(ClassInfo ci)
        {
            IEnumerator iterator = _rapidAccessForUserClassesByName.Values.GetEnumerator();
            var i = 0;
            while (iterator.MoveNext())
            {
                var classInfo = (ClassInfo) iterator.Current;
                Debug.Assert(classInfo != null);

                if (classInfo.GetId() == ci.GetId())
                    return i;
                i++;
            }
            throw new OdbRuntimeException(
                NDatabaseError.ClassInfoDoesNotExistInMetaModel.AddParameter(ci.GetFullClassName()));
        }

        /// <param name="index"> The index of the class info to get </param>
        /// <returns> The class info at the specified index </returns>
        public virtual ClassInfo GetClassInfo(int index)
        {
            return _allClassInfos[index];
        }

        /// <summary>
        ///   The method is slow nut it is only used in the odb explorer.
        /// </summary>
        /// <remarks>
        ///   The method is slow nut it is only used in the odb explorer.
        /// </remarks>
        /// <param name="index"> </param>
        /// <returns> </returns>
        public virtual ClassInfo SlowGetUserClassInfo(int index)
        {
            IEnumerator iterator = _rapidAccessForUserClassesByName.Values.GetEnumerator();
            var i = 0;
            while (iterator.MoveNext())
            {
                var classInfo = (ClassInfo) iterator.Current;
                if (i == index)
                    return classInfo;
                i++;
            }
            throw new OdbRuntimeException(
                NDatabaseError.ClassInfoDoesNotExistInMetaModel.AddParameter(" with index " + index));
        }

        public virtual void Clear()
        {
            _rapidAccessForSystemClassesByName.Clear();
            _rapidAccessForUserClassesByName.Clear();
            _rapidAccessForSystemClassesByName = null;
            _rapidAccessForUserClassesByName = null;
            _allClassInfos.Clear();
        }

        public virtual bool HasChanged()
        {
            return _hasChanged;
        }

        public virtual void SetHasChanged(bool hasChanged)
        {
            _hasChanged = hasChanged;
        }

        public abstract ICollection<ClassInfo> GetChangedClassInfo();

        public abstract void ResetChangedClasses();

        /// <summary>
        ///   Saves the fact that something has changed in the class (number of objects or last object oid)
        /// </summary>
        /// <param name="ci"> </param>
        public abstract void AddChangedClass(ClassInfo ci);

        public virtual IDictionary<string, object> GetHistory()
        {
            IDictionary<string, object> map = new OdbHashMap<string, object>();
            var iterator = _allClassInfos.GetEnumerator();
            while (iterator.MoveNext())
            {
                var classInfo = iterator.Current;
                Debug.Assert(classInfo != null);

                map.Add(classInfo.GetFullClassName(), classInfo.GetHistory());
            }
            return map;
        }

        /// <summary>
        ///   Builds a meta model from a list of class infos
        /// </summary>
        /// <param name="classInfos"> </param>
        /// <returns> The new Metamodel </returns>
        public static MetaModel FromClassInfos(IOdbList<ClassInfo> classInfos)
        {
            MetaModel metaModel = new SessionMetaModel();
            var nbClasses = classInfos.Count;

            for (var i = 0; i < nbClasses; i++)
                metaModel.AddClass(classInfos[i]);

            return metaModel;
        }

        /// <summary>
        ///   Gets all the persistent classes that are subclasses or equal to the parameter class
        /// </summary>
        /// <param name="fullClassName"> </param>
        /// <returns> The list of class info of persistent classes that are subclasses or equal to the class </returns>
        public virtual IOdbList<ClassInfo> GetPersistentSubclassesOf(string fullClassName)
        {
            IOdbList<ClassInfo> result = new OdbArrayList<ClassInfo>();
            var classNames = _rapidAccessForUserClassesByName.Keys.GetEnumerator();

            var theClass = _classPool.GetClass(fullClassName);
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
                    var oneClass = _classPool.GetClass(oneClassName);
                    if (theClass.IsAssignableFrom(oneClass))
                        result.Add(GetClassInfo(oneClassName, true));
                }
            }

            return result;
        }

        public abstract MetaModel Duplicate();
    }
}
