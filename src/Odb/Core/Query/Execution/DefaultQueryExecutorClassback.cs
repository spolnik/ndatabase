using System;

namespace NDatabase.Odb.Core.Query.Execution
{
    public class DefaultQueryExecutorClassback : IQueryExecutorCallback
    {
        #region IQueryExecutorCallback Members

        public virtual void ReadingObject(long index, long oid)
        {
            Console.Out.WriteLine("{0} : oid = {1}", (index + 1), oid);
        }

        #endregion
    }
}
