namespace NDatabase2.Odb
{
    /// <summary>
    ///   A class abstraction to give access to class level configuration like adding an index, checking if index exists, rebuilding an index,...
    /// </summary>
    public interface IClassRepresentation
    {
        /// <param name="name"> The name of the index </param>
        /// <param name="indexFields"> The list of fields of the index </param>
        /// <exception cref="System.IO.IOException">System.IO.IOException</exception>
        /// <exception cref="System.Exception">System.Exception</exception>
        void AddUniqueIndexOn(string name, string[] indexFields);

        /// <param name="name"> The name of the index </param>
        /// <param name="indexFields"> The list of fields of the index </param>
        /// <exception cref="System.IO.IOException">System.IO.IOException</exception>
        /// <exception cref="System.Exception">System.Exception</exception>
        void AddIndexOn(string name, string[] indexFields);

        /// <summary>
        ///   To check if an index exist
        /// </summary>
        /// <param name="indexName"> </param>
        /// <returns> </returns>
        bool ExistIndex(string indexName);

        void RebuildIndex(string indexName);

        void DeleteIndex(string indexName);
    }
}
