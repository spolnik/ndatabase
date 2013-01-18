using System.Collections.Generic;
using System.Linq.Expressions;

namespace NDatabase.Odb.Core.Query.Linq
{
    internal class OrderByAscendingClauseVisitor : OrderByClauseVisitorBase
    {
        private static readonly Dictionary<Expression, IQueryBuilderRecord> Cache =
            new Dictionary<Expression, IQueryBuilderRecord>();

        protected override Dictionary<Expression, IQueryBuilderRecord> GetCachingStrategy()
        {
            return Cache;
        }

        protected override void ApplyDirection(IQuery query)
        {
            query.OrderAscending();
        }
    }
}