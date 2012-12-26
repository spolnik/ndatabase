using NDatabase2.Odb.Core.Layers.Layer2.Meta;
using NDatabase2.Odb.Core.Layers.Layer3;

namespace NDatabase2.Odb.Main
{
    internal sealed class IndexManager : IIndexManager
    {
        private readonly ClassInfo _classInfo;

        private readonly IStorageEngine _storageEngine;

        internal IndexManager(IStorageEngine storageEngine, ClassInfo classInfo)
        {
            _storageEngine = storageEngine;
            _classInfo = classInfo;
        }

        #region IClassRepresentation Members

        public void AddUniqueIndexOn(string indexName, params string[] indexFields)
        {
            _storageEngine.AddIndexOn(_classInfo.FullClassName, indexName, indexFields, false);
        }

        public void AddIndexOn(string indexName, params string[] indexFields)
        {
            _storageEngine.AddIndexOn(_classInfo.FullClassName, indexName, indexFields, true);
        }

        public bool ExistIndex(string indexName)
        {
            return _classInfo.HasIndex(indexName);
        }

        /// <summary>
        ///   Used to rebuild an index
        /// </summary>
        public void RebuildIndex(string indexName)
        {
            _storageEngine.RebuildIndex(_classInfo.FullClassName, indexName);
        }

        public void DeleteIndex(string indexName)
        {
            _storageEngine.DeleteIndex(_classInfo.FullClassName, indexName);
        }

        #endregion
    }
}
