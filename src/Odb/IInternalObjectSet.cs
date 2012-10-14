using System.Collections.Generic;
using NDatabase2.Odb.Core;
using NDatabase2.Tool.Wrappers;

namespace NDatabase2.Odb
{
    internal interface IInternalObjectSet<TItem> : IObjects<TItem>
    {
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