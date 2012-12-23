using System.Collections.Generic;
using System.Linq.Expressions;

namespace NDatabase2.Odb.Core.Query.Linq
{
    internal class OrderByDescendingClauseVisitor : OrderByClauseVisitorBase
    {
        private static readonly IDictionary<Expression, IQueryBuilderRecord> Cache =
            new Dictionary<Expression, IQueryBuilderRecord>(10);

        protected override IDictionary<Expression, IQueryBuilderRecord> GetCachingStrategy()
        {
            return Cache;
        }

        protected override void ApplyDirection(IQuery query)
        {
            query.OrderDescending();
        }
    }
}