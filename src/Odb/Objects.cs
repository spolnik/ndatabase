using System.Collections.Generic;
using NDatabase2.Odb.Core;
using NDatabase2.Tool.Wrappers;

namespace NDatabase2.Odb
{
    /// <summary>
    ///   The main interface of all query results of NDatabase ODB.
    /// </summary>
    /// <remarks>
    ///   The main interface of all query results of NDatabase ODB. Objects interface extends the Collection interface so it provides a standard collection behavior.
    /// </remarks>
    /// <author>osmadja</author>
    public interface IObjects<TItem> : ICollection<TItem>
    {
        /// <summary>
        ///   Inform if the internal Iterator has more objects
        /// </summary>
        /// <returns> </returns>
        bool HasNext();

        /// <summary>
        ///   Returns the next object of the internal iterator of the collection
        /// </summary>
        /// <returns> </returns>
        TItem Next();

        /// <summary>
        ///   Return the first object of the collection, if exist
        /// </summary>
        /// <returns> </returns>
        TItem GetFirst();

        /// <summary>
        ///   Reset the internal iterator of the collection
        /// </summary>
        void Reset();

        /// <summary>
        ///   Add an object into the collection using a specific ordering key
        /// </summary>
        /// <param name="key"> </param>
        /// <param name="item"> The object can be an OID, can o NNOI (NonNativeObjectInfo) or the object </param>
        /// <returns> </returns>
        bool AddWithKey(IOdbComparable key, TItem item);

        /// <summary>
        ///   Add an object into the collection using a specific ordering key
        /// </summary>
        /// <param name="key"> </param>
        /// <param name="item"> </param>
        /// <returns> </returns>
        bool AddWithKey(int key, TItem item);

        /// <summary>
        ///   Returns the collection iterator throughout the order by <see cref="OrderByConstants">NDatabase.Odb.Core.OrderByConstants</see>
        /// </summary>
        /// <param name="orderByType"> </param>
        /// <returns> </returns>
        IEnumerator<TItem> Iterator(OrderByConstants orderByType);

        void AddOid(OID oid);
    }
}
