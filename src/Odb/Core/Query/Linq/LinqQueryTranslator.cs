using System.Linq.Expressions;

namespace NDatabase2.Odb.Core.Query.Linq
{
    internal class LinqQueryTranslator : ExpressionVisitor
    {
        public static Expression Translate(Expression expression)
        {
            return new LinqQueryTranslator().Visit(expression);
        }

        protected override Expression VisitLambda(LambdaExpression lambda)
        {
            return lambda;
        }
    }
}