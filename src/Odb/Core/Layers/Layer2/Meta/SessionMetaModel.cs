using System;
using System.Collections.Generic;
using NDatabase.Tool.Wrappers.List;
using NDatabase.Tool.Wrappers.Map;

namespace NDatabase.Odb.Core.Layers.Layer2.Meta
{
    /// <summary>
    ///   The main implementation of the MetaModel abstract class.
    /// </summary>
    /// <remarks>
    ///   The main implementation of the MetaModel abstract class.
    /// </remarks>
    /// <author>osmadja</author>
    [Serializable]
    public sealed class SessionMetaModel : MetaModel
    {
        /// <summary>
        ///   A list of changed classes - that must be persisted back when commit is done
        /// </summary>
        private OdbHashMap<ClassInfo, ClassInfo> _changedClasses;

        public SessionMetaModel()
        {
            _changedClasses = new OdbHashMap<ClassInfo, ClassInfo>();
        }

        /// <summary>
        ///   Saves the fact that something has changed in the class (number of objects or last object oid)
        /// </summary>
        /// <param name="classInfo"> </param>
        public override void AddChangedClass(ClassInfo classInfo)
        {
            _changedClasses[classInfo] = classInfo;
            SetHasChanged(true);
        }

        public override ICollection<ClassInfo> GetChangedClassInfo()
        {
            IOdbList<ClassInfo> list = new OdbArrayList<ClassInfo>();
            list.AddAll(_changedClasses.Keys);
            // TODO return an unmodifianle collection
            // return Collections.unmodifiableCollection(l);
            return list;
        }

        public override void ResetChangedClasses()
        {
            _changedClasses.Clear();
            SetHasChanged(false);
        }

        public override MetaModel Duplicate()
        {
            var model = new SessionMetaModel();
            var classes = GetAllClasses();
            foreach (var classInfo in classes)
                model.AddClass(classInfo.Duplicate(false));

            model._changedClasses = new OdbHashMap<ClassInfo, ClassInfo>();
            model._changedClasses.PutAll(_changedClasses);

            return model;
        }
    }
}
