using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace NDatabase2.Odb.Core.Query.Linq
{
    internal class OrderByDescendingClauseVisitor : OrderByClauseVisitorBase
    {
        private static readonly ConcurrentDictionary<Expression, IQueryBuilderRecord> Cache =
            new ConcurrentDictionary<Expression, IQueryBuilderRecord>();

        protected override ConcurrentDictionary<Expression, IQueryBuilderRecord> GetCachingStrategy()
        {
            return Cache;
        }

        protected override void ApplyDirection(IQuery query)
        {
            query.OrderDescending();
        }
    }
}