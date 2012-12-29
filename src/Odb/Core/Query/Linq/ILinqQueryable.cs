using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NDatabase.Odb.Core.Query.Linq
{
    public interface ILinqQueryable<TElement> : IOrderedQueryable<TElement>, ILinqQueryable
    {
    }

    public interface ILinqQueryable : IOrderedQueryable
    {
        ILinqQuery GetQuery();
    }

    public interface ILinqQuery<T> : ILinqQuery, IEnumerable<T>
    {
    }

    public interface ILinqQuery : IEnumerable
    {
    }
}