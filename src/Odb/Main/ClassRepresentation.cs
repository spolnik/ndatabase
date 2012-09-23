using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Odb.Core.Layers.Layer3;

namespace NDatabase.Odb.Main
{
    public sealed class ClassRepresentation : IClassRepresentation
    {
        private readonly ClassInfo _classInfo;

        private readonly IStorageEngine _storageEngine;

        internal ClassRepresentation(IStorageEngine storageEngine, ClassInfo classInfo)
        {
            _storageEngine = storageEngine;
            _classInfo = classInfo;
        }

        #region IClassRepresentation Members

        public void AddUniqueIndexOn(string name, string[] indexFields, bool verbose)
        {
            _storageEngine.AddIndexOn(_classInfo.FullClassName, name, indexFields, verbose, false);
        }

        public void AddIndexOn(string name, string[] indexFields, bool verbose)
        {
            _storageEngine.AddIndexOn(_classInfo.FullClassName, name, indexFields, verbose, true);
        }

        public bool ExistIndex(string indexName)
        {
            return _classInfo.HasIndex(indexName);
        }

        /// <summary>
        ///   Used to rebuild an index
        /// </summary>
        public void RebuildIndex(string indexName, bool verbose)
        {
            _storageEngine.RebuildIndex(_classInfo.FullClassName, indexName, verbose);
        }

        public void DeleteIndex(string indexName, bool verbose)
        {
            _storageEngine.DeleteIndex(_classInfo.FullClassName, indexName, verbose);
        }

        #endregion
    }
}
