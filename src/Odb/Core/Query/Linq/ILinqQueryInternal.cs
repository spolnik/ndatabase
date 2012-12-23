using System;
using System.Collections.Generic;

namespace NDatabase2.Odb.Core.Query.Linq
{
    internal interface ILinqQueryInternal<out T> : ILinqQuery<T>
    {
        IEnumerable<T> UnoptimizedThenBy<TKey>(Func<T, TKey> function);

        IEnumerable<T> UnoptimizedThenByDescending<TKey>(Func<T, TKey> function);

        IEnumerable<T> UnoptimizedWhere(Func<T, bool> func);
    }
}