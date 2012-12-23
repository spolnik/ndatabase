using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NDatabase2.Odb.Core.Query.Linq
{
    public interface ILinqQueryable<out TElement> : IOrderedQueryable<TElement>, ILinqQueryable
    {
    }

    public interface ILinqQueryable : IOrderedQueryable
    {
        ILinqQuery GetQuery();
    }

    public interface ILinqQuery<out T> : ILinqQuery, IEnumerable<T>
    {
    }

    public interface ILinqQuery : IEnumerable
    {
    }
}