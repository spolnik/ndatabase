using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace NDatabase.Odb.Core.Query.Linq
{
    internal sealed class OrderByAscendingClauseVisitor : OrderByClauseVisitorBase
    {
        private static readonly ConcurrentDictionary<Expression, IQueryBuilderRecord> Cache =
            new ConcurrentDictionary<Expression, IQueryBuilderRecord>();

        protected override ConcurrentDictionary<Expression, IQueryBuilderRecord> GetCachingStrategy()
        {
            return Cache;
        }

        protected override void ApplyDirection(IQuery query)
        {
            query.OrderAscending();
        }
    }
}