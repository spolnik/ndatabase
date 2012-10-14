using System;
using NDatabase2.Odb.Core.Layers.Layer3;
using NDatabase2.Odb.Core.Query;
using NDatabase2.Odb.Core.Query.Criteria;
using NDatabase2.Odb.Core.Trigger;

namespace NDatabase2.Odb
{
    /// <summary>
    ///   The main ODB public interface: It is what the user sees.
    /// </summary>
    public interface IOdb : IDisposable
    {
        /// <summary>
        ///   Commit all the change of the database @
        /// </summary>
        void Commit();

        /// <summary>
        ///   Undo all uncommitted changes
        /// </summary>
        void Rollback();

        /// <summary>
        ///   Closes the database.
        /// </summary>
        /// <remarks>
        ///   Closes the database. Automatically commit uncommitted changes
        /// </remarks>
        void Close();

        /// <summary>
        ///   Store a plain Object in the ODB Database
        /// </summary>
        /// <param name="plainObject"> A plain Object </param>
        OID Store<T>(T plainObject) where T : class;

        /// <summary>
        ///   Get all objects of a specific type
        /// </summary>
        /// <returns> The list of objects </returns>
        IObjects<T> GetObjects<T>() where T : class;

        /// <summary>
        ///   Get all objects of a specific type
        /// </summary>
        /// <param name="inMemory"> if true, preload all objects,if false,load on demand </param>
        /// <returns> The list of objects </returns>
        IObjects<T> GetObjects<T>(bool inMemory) where T : class;

        /// <param name="inMemory"> if true, preload all objects,if false,load on demand </param>
        /// <param name="startIndex"> The index of the first object </param>
        /// <param name="endIndex"> The index of the last object that must be returned </param>
        /// <returns> A List of objects </returns>
        IObjects<T> GetObjects<T>(bool inMemory, int startIndex, int endIndex) where T : class;

        /// <summary>
        ///   Delete an object from database
        /// </summary>
        OID Delete<T>(T plainObject) where T : class;

        /// <summary>
        ///   Delete an object from the database with the id
        /// </summary>
        /// <param name="oid"> The object id to be deleted </param>
        void DeleteObjectWithId(OID oid);

        /// <summary>
        ///   Search for objects that matches the query.
        /// </summary>
        /// <returns> The list of values </returns>
        IValues GetValues<T>(IValuesQuery query) where T : class;

        /// <summary>
        ///   Search for objects that matches the query.
        /// </summary>
        /// <returns> The list of objects </returns>
        IObjects<T> GetObjects<T>(IQuery query) where T : class;

        /// <summary>
        ///   Search for objects that matches the native query.
        /// </summary>
        /// <returns> The list of objects </returns>
        IObjects<T> GetObjects<T>(IQuery query, bool inMemory) where T : class;

        /// <summary>
        ///   Return a list of objects that matches the query
        /// </summary>
        /// <param name="query"> </param>
        /// <param name="inMemory"> if true, preload all objects,if false,load on demand </param>
        /// <param name="startIndex"> The index of the first object </param>
        /// <param name="endIndex"> The index of the last object that must be returned </param>
        /// <returns> A List of objects, if start index and end index are -1, they are ignored. 
        /// If not, the length of the sublist is endIndex - startIndex </returns>
        IObjects<T> GetObjects<T>(IQuery query, bool inMemory, int startIndex, int endIndex) where T : class;

        /// <summary>
        ///   Returns the number of objects that satisfy the query
        /// </summary>
        /// <param name="query"> </param>
        /// <returns> The number of objects that satisfy the query </returns>
        long Count<T>(CriteriaQuery<T> query) where T : class;

        /// <summary>
        ///   Get the id of an ODB-aware object
        /// </summary>
        /// <param name="plainObject"> </param>
        /// <returns> The ODB internal object id </returns>
        OID GetObjectId<T>(T plainObject) where T : class;

        /// <summary>
        ///   Get the object with a specific id
        /// </summary>
        /// <param name="id"> </param>
        /// <returns> The object with the specific id @ </returns>
        object GetObjectFromId(OID id);

        /// <summary>
        ///   Defragment ODB Database
        /// </summary>
        void DefragmentTo(string newFileName);

        /// <summary>
        ///   Get an abstract representation of a class
        /// </summary>
        /// <returns> a public meta-representation of a class </returns>
        IClassRepresentation GetClassRepresentation<T>() where T : class;

        /// <summary>
        ///   Used to add an update trigger callback for the specific class
        /// </summary>
        void AddUpdateTrigger<T>(UpdateTrigger trigger) where T : class;

        /// <summary>
        ///   Used to add an insert trigger callback for the specific class
        /// </summary>
        void AddInsertTrigger<T>(InsertTrigger trigger) where T : class;

        /// <summary>
        ///   USed to add a delete trigger callback for the specific class
        /// </summary>
        void AddDeleteTrigger<T>(DeleteTrigger trigger) where T : class;

        /// <summary>
        ///   Used to add a select trigger callback for the specific class
        /// </summary>
        void AddSelectTrigger<T>(SelectTrigger trigger) where T : class;

        /// <summary>
        ///   Returns the object used to refactor the database
        /// </summary>
        IRefactorManager GetRefactorManager();

        /// <summary>
        ///   Get the extension of ODB to get access to advanced functions
        /// </summary>
        IOdbExt Ext();

        /// <summary>
        ///   Used to disconnect the object from the current session.
        /// </summary>
        /// <remarks>
        ///   Used to disconnect the object from the current session. The object is removed from the cache
        /// </remarks>
        void Disconnect<T>(T plainObject) where T : class;

        bool IsClosed();

        CriteriaQuery<T> CriteriaQuery<T>(IConstraint criterio) where T : class;

        CriteriaQuery<T> CriteriaQuery<T>() where T : class;

        /// <summary>
        ///   Return the name of the database
        /// </summary>
        /// <returns> the file name in local mode and the base id (alias) in client server mode. </returns>
        string GetName();
    }
}
