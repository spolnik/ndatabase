using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NDatabase.Odb.Core.Query.Linq
{
    /// <summary>
    /// IOrderedQueryable&lt;T&gt; derived interface, Linq
    /// </summary>
    public interface ILinqQueryable<TElement> : IOrderedQueryable<TElement>, ILinqQueryable
    {
    }

    /// <summary>
    /// IOrderedQueryable derived interface, Linq
    /// </summary>
    public interface ILinqQueryable : IOrderedQueryable
    {
        /// <summary>
        /// Get underliying linq query
        /// </summary>
        /// <returns>Linq query</returns>
        ILinqQuery GetQuery();
    }

    /// <summary>
    /// NDatabase Linq Query generic interface
    /// </summary>
    public interface ILinqQuery<T> : ILinqQuery, IEnumerable<T>
    {
    }

    /// <summary>
    /// NDatabase Linq Query interface
    /// </summary>
    public interface ILinqQuery : IEnumerable
    {
    }
}