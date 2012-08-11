using System;
using NDatabase.Odb.Core.Layers.Layer1.Introspector;
using NDatabase.Odb.Core.Layers.Layer2.Instance;
using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Odb.Core.Layers.Layer3;

namespace NDatabase.Odb.Impl.Main
{
    [Serializable]
    public class DefaultClassRepresentation : IClassRepresentation
    {
        private readonly ClassInfo _classInfo;

        private readonly IClassIntrospector _classIntrospector;
        private readonly IStorageEngine _storageEngine;

        public DefaultClassRepresentation(IStorageEngine storageEngine, ClassInfo classInfo)
        {
            _storageEngine = storageEngine;
            _classInfo = classInfo;
            _classIntrospector = OdbConfiguration.GetCoreProvider().GetClassIntrospector();
        }

        #region IClassRepresentation Members

        public virtual void AddUniqueIndexOn(string name, string[] indexFields, bool verbose)
        {
            _storageEngine.AddIndexOn(_classInfo.GetFullClassName(), name, indexFields, verbose, false);
        }

        public virtual void AddIndexOn(string name, string[] indexFields, bool verbose)
        {
            _storageEngine.AddIndexOn(_classInfo.GetFullClassName(), name, indexFields, verbose, true);
        }

        public virtual void AddInstantiationHelper(IInstantiationHelper instantiationHelper)
        {
            _classIntrospector.AddInstantiationHelper(_classInfo.GetFullClassName(), instantiationHelper);
        }

        public virtual void AddFullInstantiationHelper(IFullInstantiationHelper instantiationHelper)
        {
            _classIntrospector.AddFullInstantiationHelper(_classInfo.GetFullClassName(), instantiationHelper);
        }

        public virtual void AddParameterHelper(IParameterHelper parameterHelper)
        {
            _classIntrospector.AddParameterHelper(_classInfo.GetFullClassName(), parameterHelper);
        }

        public virtual void RemoveInstantiationHelper()
        {
            _classIntrospector.RemoveInstantiationHelper(_classInfo.GetFullClassName());
        }

        public virtual void RemoveFullInstantiationHelper()
        {
            _classIntrospector.RemoveInstantiationHelper(_classInfo.GetFullClassName());
        }

        public virtual void RemoveParameterHelper()
        {
            _classIntrospector.RemoveParameterHelper(_classInfo.GetFullClassName());
        }

        public virtual bool ExistIndex(string indexName)
        {
            return _classInfo.HasIndex(indexName);
        }

        /// <summary>
        ///   Used to rebuild an index
        /// </summary>
        public virtual void RebuildIndex(string indexName, bool verbose)
        {
            _storageEngine.RebuildIndex(_classInfo.GetFullClassName(), indexName, verbose);
        }

        public virtual void DeleteIndex(string indexName, bool verbose)
        {
            _storageEngine.DeleteIndex(_classInfo.GetFullClassName(), indexName, verbose);
        }

        #endregion
    }
}
