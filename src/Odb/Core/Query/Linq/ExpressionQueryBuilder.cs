using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

namespace NDatabase2.Odb.Core.Query.Linq
{
    internal abstract class ExpressionQueryBuilder : ExpressionVisitor
    {
        public QueryBuilderRecorder Recorder { get; private set; }

        public virtual IQueryBuilderRecord Process(LambdaExpression expression)
        {
            return ProcessExpression(SubtreeEvaluator.Evaluate(Normalize(expression)));
        }

        private static Expression Normalize(Expression expression)
        {
            return new ExpressionTreeNormalizer().Normalize(expression);
        }

        protected abstract ConcurrentDictionary<Expression, IQueryBuilderRecord> GetCachingStrategy();

        private IQueryBuilderRecord ProcessExpression(Expression expression)
        {
            return GetCachingStrategy().GetOrAdd(expression, CreateRecord);
        }

        private IQueryBuilderRecord CreateRecord(Expression expression)
        {
            Recorder = new QueryBuilderRecorder();
            Visit(expression);
            return Recorder.Record;
        }

        private static bool IsParameter(Expression expression)
        {
            return expression.NodeType == ExpressionType.Parameter;
        }

        protected static bool StartsWithParameterReference(Expression expression)
        {
            if (IsParameter(expression))
                return true;

            var unary = expression as UnaryExpression;
            if (unary != null)
                return StartsWithParameterReference(unary.Operand);

            var me = expression as MemberExpression;
            if (me != null)
                return StartsWithParameterReference(me.Expression);

            var call = expression as MethodCallExpression;
            if (call != null && call.Object != null)
                return StartsWithParameterReference(call.Object);

            return false;
        }

        protected static bool IsFieldAccessExpression(MemberExpression m)
        {
            return m.Member.MemberType == MemberTypes.Field;
        }

        protected static bool IsPropertyAccessExpression(MemberExpression m)
        {
            return m.Member.MemberType == MemberTypes.Property;
        }

        protected static MethodInfo GetGetMethod(MemberExpression m)
        {
            return ((PropertyInfo) m.Member).GetGetMethod();
        }

        protected void ProcessMemberAccess(MemberExpression m)
        {
            Visit(m.Expression);
            if (IsFieldAccessExpression(m))
            {
                var descendingEnumType = ResolveDescendingEnumType(m);
                Recorder.Add(
                    ctx =>
                        {
                            ctx.Descend(m.Member.Name);
                            ctx.PushDescendigFieldEnumType(descendingEnumType);
                        });

                return;
            }

            if (IsPropertyAccessExpression(m))
            {
                AnalyseMethod(Recorder, GetGetMethod(m));
                return;
            }

            CannotOptimize(m);
        }

        private static Type ResolveDescendingEnumType(Expression expression)
        {
            return !expression.Type.IsEnum ? null : expression.Type;
        }

        protected void AnalyseMethod(QueryBuilderRecorder recorder, MethodInfo method)
        {
            try
            {
                var analyser = ReflectionMethodAnalyser.FromMethod(method);
                analyser.Run(recorder);
            }
            catch (Exception e)
            {
                throw new LinqQueryException(e.Message, e);
            }
        }

        protected static void CannotOptimize(Expression e)
        {
            throw new LinqQueryException(e.ToString());
        }

        private static void CannotOptimize(ElementInit init)
        {
            throw new LinqQueryException(init.ToString());
        }

        private static void CannotOptimize(MemberBinding binding)
        {
            throw new LinqQueryException(binding.ToString());
        }

        protected override void VisitBinding(MemberBinding binding)
        {
            CannotOptimize(binding);
        }

        protected override void VisitConditional(ConditionalExpression conditional)
        {
            CannotOptimize(conditional);
        }

        protected override void VisitElementInitializer(ElementInit initializer)
        {
            CannotOptimize(initializer);
        }

        protected override void VisitInvocation(InvocationExpression invocation)
        {
            CannotOptimize(invocation);
        }

        protected override void VisitListInit(ListInitExpression init)
        {
            CannotOptimize(init);
        }

        protected override void VisitNew(NewExpression nex)
        {
            CannotOptimize(nex);
        }

        protected override void VisitNewArray(NewArrayExpression newArray)
        {
            CannotOptimize(newArray);
        }
    }
}