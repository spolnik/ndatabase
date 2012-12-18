using System;
using NDatabase2.Odb.Core.Layers.Layer3;
using NDatabase2.Odb.Core.Query;

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
        ///   Search for objects that matches the query.
        /// </summary>
        /// <returns> The list of values </returns>
        IValues GetValues<T>(IValuesQuery query) where T : class;

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
        IIndexManager IndexManagerFor<T>() where T : class;

        ITriggerManager TriggerManagerFor<T>() where T : class;

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

        IQuery Query<T>() where T : class;
    }
}
