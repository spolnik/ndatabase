using System;
using NDatabase.Odb.Core.Layers.Layer3;
using NDatabase.Odb.Core.Query;
using NDatabase.Odb.Core.Query.Criteria;
using NDatabase.Odb.Core.Trigger;
using NDatabase.Odb.Impl.Core.Query.Criteria;

namespace NDatabase.Odb
{
    /// <summary>
    ///   The main ODB public interface: It is what the user sees.
    /// </summary>
    /// <remarks>
    ///   The main ODB public interface: It is what the user sees.
    /// </remarks>
    /// <author>osmadja</author>
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
        ///   Store a plain java Object in the ODB Database
        /// </summary>
        /// <param name="object"> A plain Java Object </param>
        OID Store(object @object);

        /// <summary>
        ///   Get all objects of a specific type
        /// </summary>
        /// <returns> The list of objects </returns>
        IObjects<T> GetObjects<T>();

        /// <summary>
        ///   Get all objects of a specific type
        /// </summary>
        /// <param name="inMemory"> if true, preload all objects,if false,load on demand </param>
        /// <returns> The list of objects </returns>
        IObjects<T> GetObjects<T>(bool inMemory);

        /// <param name="inMemory"> if true, preload all objects,if false,load on demand </param>
        /// <param name="startIndex"> The index of the first object </param>
        /// <param name="endIndex"> The index of the last object that must be returned </param>
        /// <returns> A List of objects </returns>
        IObjects<T> GetObjects<T>(bool inMemory, int startIndex, int endIndex);

        /// <summary>
        ///   Delete an object from database
        /// </summary>
        /// <param name="object"> </param>
        OID Delete(object @object);

        /// <summary>
        ///   Delete an object from the database with the id
        /// </summary>
        /// <param name="oid"> The object id to be deleted </param>
        void DeleteObjectWithId(OID oid);

        /// <summary>
        ///   Search for objects that matches the query.
        /// </summary>
        /// <remarks>
        ///   Search for objects that matches the query.
        /// </remarks>
        /// <param name="query"> </param>
        /// <returns> The list of values </returns>
        IValues GetValues(IValuesQuery query);

        /// <summary>
        ///   Search for objects that matches the query.
        /// </summary>
        /// <remarks>
        ///   Search for objects that matches the query.
        /// </remarks>
        /// <param name="query"> </param>
        /// <returns> The list of objects </returns>
        IObjects<T> GetObjects<T>(IQuery query);

        /// <summary>
        ///   Search for objects that matches the native query.
        /// </summary>
        /// <remarks>
        ///   Search for objects that matches the native query.
        /// </remarks>
        /// <param name="query"> </param>
        /// <param name="inMemory"> </param>
        /// <returns> The list of objects </returns>
        IObjects<T> GetObjects<T>(IQuery query, bool inMemory);

        /// <summary>
        ///   Return a list of objects that matches the query
        /// </summary>
        /// <param name="query"> </param>
        /// <param name="inMemory"> if true, preload all objects,if false,load on demand </param>
        /// <param name="startIndex"> The index of the first object </param>
        /// <param name="endIndex"> The index of the last object that must be returned </param>
        /// <returns> A List of objects, if start index and end index are -1, they are ignored. If not, the length of the sublist is endIndex - startIndex </returns>
        IObjects<T> GetObjects<T>(IQuery query, bool inMemory, int startIndex, int endIndex);

        /// <summary>
        ///   Returns the number of objects that satisfy the query
        /// </summary>
        /// <param name="query"> </param>
        /// <returns> The number of objects that satisfy the query </returns>
        long Count(CriteriaQuery query);

        /// <summary>
        ///   Get the id of an ODB-aware object
        /// </summary>
        /// <param name="object"> </param>
        /// <returns> The ODB internal object id </returns>
        OID GetObjectId(object @object);

        /// <summary>
        ///   Get the object with a specific id
        /// </summary>
        /// <param name="id"> </param>
        /// <returns> The object with the specific id @ </returns>
        object GetObjectFromId(OID id);

        /// <summary>
        ///   Defragment ODB Database
        /// </summary>
        /// <param name="newFileName"> </param>
        void DefragmentTo(string newFileName);

        /// <summary>
        ///   Get an abstract representation of a class
        /// </summary>
        /// <param name="clazz"> </param>
        /// <returns> a public meta-representation of a class </returns>
        IClassRepresentation GetClassRepresentation(Type clazz);

        /// <summary>
        ///   Get an abstract representation of a class
        /// </summary>
        /// <param name="fullClassName"> </param>
        /// <returns> a public meta-representation of a class </returns>
        IClassRepresentation GetClassRepresentation(string fullClassName);

        /// <summary>
        ///   Used to add an update trigger callback for the specific class
        /// </summary>
        /// <param name="clazz"> </param>
        /// <param name="trigger"> </param>
        void AddUpdateTrigger(Type clazz, UpdateTrigger trigger);

        /// <summary>
        ///   Used to add an insert trigger callback for the specific class
        /// </summary>
        /// <param name="clazz"> </param>
        /// <param name="trigger"> </param>
        void AddInsertTrigger(Type clazz, InsertTrigger trigger);

        /// <summary>
        ///   USed to add a delete trigger callback for the specific class
        /// </summary>
        /// <param name="clazz"> </param>
        /// <param name="trigger"> </param>
        void AddDeleteTrigger(Type clazz, DeleteTrigger trigger);

        /// <summary>
        ///   Used to add a select trigger callback for the specific class
        /// </summary>
        /// <param name="clazz"> </param>
        /// <param name="trigger"> </param>
        void AddSelectTrigger(Type clazz, SelectTrigger trigger);

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
        void Disconnect(object @object);

        /// <returns> </returns>
        bool IsClosed();

        CriteriaQuery CriteriaQuery(Type clazz, ICriterion criterio);

        CriteriaQuery CriteriaQuery(Type clazz);

        /// <summary>
        ///   Return the name of the database
        /// </summary>
        /// <returns> the file name in local mode and the base id (alias) in client server mode. </returns>
        string GetName();
    }
}