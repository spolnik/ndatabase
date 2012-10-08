using System;
using NDatabase.Odb.Core.Query.Execution;

namespace NDatabase.Odb.Core.Query.NQ
{
    public abstract class NativeQuery<T> : AbstractQuery where T : class
    {
        public virtual Type[] GetObjectTypes()
        {
            var classes = new Type[1];
            classes[0] = UnderlyingType;
            return classes;
        }

        public virtual string[] GetIndexFields()
        {
            return new string[0];
        }

        internal new void SetExecutionPlan(IQueryExecutionPlan plan)
        {
            ExecutionPlan = plan;
        }

        private readonly Type _type = typeof(T);

        public override Type UnderlyingType
        {
            get { return _type; }
        }

        public override bool Match(object @object)
        {
            return Match((T)@object);
        }

        public abstract bool Match(T @object);
    }
}
