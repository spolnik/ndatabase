using NDatabase.Odb;
using NDatabase.Odb.Core.Oid;
using NDatabase.Tool.Wrappers;

namespace NDatabase.Tool
{
    internal static class UniqueIdGenerator
    {
        private static long GetRandomLongId()
        {
            lock (typeof (UniqueIdGenerator))
            {
                return (long) (OdbRandom.GetRandomDouble() * long.MaxValue);
            }
        }

        /// <summary>
        ///   Returns a database id : 4 longs
        /// </summary>
        /// <param name="creationDate"> </param>
        /// <returns> a 4 long array </returns>
        internal static IDatabaseId GetDatabaseId(long creationDate)
        {
            var id = new[] {creationDate, GetRandomLongId(), GetRandomLongId(), GetRandomLongId()};
            
            return new DatabaseId(id);
        }
    }
}
