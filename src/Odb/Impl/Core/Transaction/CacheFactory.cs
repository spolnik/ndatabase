using NDatabase.Odb.Core.Transaction;

namespace NDatabase.Odb.Impl.Core.Transaction
{
    public static class CacheFactory
    {
        public static ICache GetLocalCache(string name)
        {
            return new Cache(name);
        }

        public static ITmpCache GetLocalTmpCache()
        {
            return new TmpCache();
        }

        /// <summary>
        ///   This factory method returns an implementation of <see cref="ICrossSessionCache">NDatabase.Odb.Core.Transaction.ICrossSessionCache</see> to take over the objects across the sessions.
        /// </summary>
        /// <param name="identification"> TODO </param>
        /// <returns> <see cref="ICrossSessionCache">NDatabase.Odb.Core.Transaction.ICrossSessionCache</see> </returns>
        public static ICrossSessionCache GetCrossSessionCache(string identification)
        {
            return CrossSessionCache.GetInstance(identification);
        }
    }
}
