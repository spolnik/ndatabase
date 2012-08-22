using NDatabase.Odb.Impl.Core.Oid;
using NDatabase.Tool.Wrappers;

namespace NDatabase.Odb.Impl.Tool
{
    /// <summary>
    ///   Unique ID generator
    /// </summary>
    /// <author>osmadja</author>
    public static class UUID
    {
        public static long GetUniqueId(string simpleSeed)
        {
            lock (typeof (UUID))
            {
                return OdbTime.GetCurrentTimeInTicks() - (long) (OdbRandom.GetRandomDouble() * simpleSeed.GetHashCode());
            }
        }

        public static long GetRandomLongId()
        {
            lock (typeof (UUID))
            {
                return (long) (OdbRandom.GetRandomDouble() * long.MaxValue);
            }
        }

        /// <summary>
        ///   Returns a block marker , 5 longs
        /// </summary>
        /// <param name="position"> </param>
        /// <returns> A 4 long array </returns>
        public static long[] GetBlockMarker(long position)
        {
            lock (typeof (UUID))
            {
                const long l1 = unchecked((int) (0xFFEFCFBF));
                var id = new[] {l1, l1, l1, position, l1};
                return id;
            }
        }

        /// <summary>
        ///   Returns a database id : 4 longs
        /// </summary>
        /// <param name="creationDate"> </param>
        /// <returns> a 4 long array </returns>
        public static IDatabaseId GetDatabaseId(long creationDate)
        {
            lock (typeof (UUID))
            {
                var id = new[] {creationDate, GetRandomLongId(), GetRandomLongId(), GetRandomLongId()};
                // FIXME do  not instanciate directly
                IDatabaseId databaseId = new DatabaseIdImpl(id);
                return databaseId;
            }
        }
    }
}
