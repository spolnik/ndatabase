namespace NDatabase.Odb
{
    /// <summary>
    ///   A class abstraction to give access to class level configuration like adding an index, checking if index exists, rebuilding an index,...
    /// </summary>
    public interface IIndexManager
    {
        /// <param name="indexName"> The name of the index </param>
        /// <param name="indexFields"> The list of fields of the index </param>
        void AddUniqueIndexOn(string indexName, params string[] indexFields);

        /// <param name="indexName"> The name of the index </param>
        /// <param name="indexFields"> The list of fields of the index </param>
        void AddIndexOn(string indexName, params string[] indexFields);

        /// <summary>
        ///   To check if an index exist
        /// </summary>
        bool ExistIndex(string indexName);

        void RebuildIndex(string indexName);

        void DeleteIndex(string indexName);
    }
}
