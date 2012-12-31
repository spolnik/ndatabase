using System.Collections.Generic;
using NDatabase.Odb.Core;
using NDatabase.Tool.Wrappers;

namespace NDatabase.Odb
{
    internal interface IInternalObjectSet<TItem> : IObjectSet<TItem>
    {
        /// <summary>
        ///   Add an object into the collection using a specific ordering key
        /// </summary>
        /// <param name="key"> </param>
        /// <param name="item"> The object can be an OID, can o NNOI (NonNativeObjectInfo) or the object </param>
        void AddWithKey(IOdbComparable key, TItem item);

        /// <summary>
        ///   Returns the collection iterator throughout the order by <see cref="OrderByConstants">NDatabase.Odb.Core.OrderByConstants</see>
        /// </summary>
        /// <param name="orderByType"> </param>
        /// <returns> </returns>
        IEnumerator<TItem> Iterator(OrderByConstants orderByType);

        void AddOid(OID oid);
    }
}