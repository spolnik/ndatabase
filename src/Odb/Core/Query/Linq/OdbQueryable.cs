using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NDatabase2.Odb.Core.Query.Linq.Helper;

namespace NDatabase2.Odb.Core.Query.Linq
{
    public class OdbQueryable<TElement> : IOrderedQueryable<TElement>, IQueryProvider
    {
        private readonly Expression _expression;
        private readonly IQueryProvider _provider;

        protected OdbQueryable(IQueryProvider provider)
        {
            if (provider == null)
                throw new ArgumentNullException("provider");

            _provider = provider;
            _expression = Expression.Constant(this);
        }

        protected OdbQueryable(IQueryProvider provider, Expression expression)
        {
            if (provider == null)
                throw new ArgumentNullException("provider");

            if (expression == null)
                throw new ArgumentNullException("expression");

            if (!typeof (IQueryable<TElement>).IsAssignableFrom(expression.Type))
                throw new ArgumentOutOfRangeException("expression");

            _provider = provider;
            _expression = expression;
        }

        #region IOrderedQueryable<TItem> Members

        public IEnumerator<TElement> GetEnumerator()
        {
            return _provider.Execute<IEnumerable<TElement>>(_expression).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _provider.Execute<IEnumerable>(_expression).GetEnumerator();
        }

        public Expression Expression
        {
            get { return _expression; }
        }

        public Type ElementType
        {
            get { return typeof (TElement); }
        }

        public IQueryProvider Provider
        {
            get { return _provider; }
        }

        #endregion

        #region IQueryProvider Members

        public IQueryable CreateQuery(Expression expression)
        {
            var elementType = TypeSystem.GetElementType(expression.Type);

            try
            {
                return
                    (IQueryable)
                    Activator.CreateInstance(typeof(OdbQueryable<>).MakeGenericType(elementType),
                                             new object[] { this, expression });
            }
            catch (TargetInvocationException tie)
            {
                throw tie.InnerException;
            }
        }

        public IQueryable<T> CreateQuery<T>(Expression expression)
        {
            return new OdbQueryable<T>(this, expression);
        }

        public object Execute(Expression expression)
        {
            return Expression.Lambda(TranslateQuery(expression)).Compile().DynamicInvoke();
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return Expression.Lambda<Func<TResult>>(TranslateQuery(expression)).Compile().Invoke();
        }

        #endregion

        private static Expression TranslateQuery(Expression expression)
        {
            return LinqQueryTranslator.Translate(expression);
        }
    }
}
