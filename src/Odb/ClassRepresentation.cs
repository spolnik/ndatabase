using NDatabase.Odb.Core.Layers.Layer2.Instance;

namespace NDatabase.Odb
{
    /// <summary>
    ///   A class abstraction to give access to class level configuration like adding an index, checking if index exists, rebuilding an index,...
    /// </summary>
    /// <remarks>
    ///   A class abstraction to give access to class level configuration like adding an index, checking if index exists, rebuilding an index,...
    /// </remarks>
    /// <author>osmadja</author>
    public interface IClassRepresentation
    {
        /// <param name="name"> The name of the index </param>
        /// <param name="indexFields"> The list of fields of the index </param>
        /// <param name="verbose"> A boolean value to indicate of ODB must describe what it is doing </param>
        /// <exception cref="System.IO.IOException">System.IO.IOException</exception>
        /// <exception cref="System.Exception">System.Exception</exception>
        void AddUniqueIndexOn(string name, string[] indexFields, bool verbose);

        /// <param name="name"> The name of the index </param>
        /// <param name="indexFields"> The list of fields of the index </param>
        /// <param name="verbose"> A boolean value to indicate of ODB must describe what it is doing </param>
        /// <exception cref="System.IO.IOException">System.IO.IOException</exception>
        /// <exception cref="System.Exception">System.Exception</exception>
        void AddIndexOn(string name, string[] indexFields, bool verbose);

        /// <summary>
        ///   Adds an helper to tell ODB how to create an instance when no default constructor is available
        /// </summary>
        void AddParameterHelper(IParameterHelper parameterHelper);

        /// <summary>
        ///   Remove the parameter helper for this class, if exists
        /// </summary>
        void RemoveParameterHelper();

        /// <summary>
        ///   Adds an helper to tell ODB what class to call to create an instance
        /// </summary>
        void AddInstantiationHelper(IInstantiationHelper instantiationHelper);

        /// <summary>
        ///   Remove the instantiation helper of this class, if exists
        /// </summary>
        void RemoveInstantiationHelper();

        /// <summary>
        ///   Adds an helper to tell ODB what class to call to create an instance
        /// </summary>
        void AddFullInstantiationHelper(IFullInstantiationHelper instantiationHelper);

        /// <summary>
        ///   Remove the instantiation helper of this class, if exists
        /// </summary>
        void RemoveFullInstantiationHelper();

        /// <summary>
        ///   To check if an index exist
        /// </summary>
        /// <param name="indexName"> </param>
        /// <returns> </returns>
        bool ExistIndex(string indexName);

        /// <param name="indexName"> </param>
        /// <param name="verbose"> </param>
        void RebuildIndex(string indexName, bool verbose);

        /// <param name="indexName"> </param>
        /// <param name="verbose"> </param>
        void DeleteIndex(string indexName, bool verbose);
    }
}
