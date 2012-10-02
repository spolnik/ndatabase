using System;
using NDatabase.Odb.Core.Query.Execution;

namespace NDatabase.Odb.Core.Query.NQ
{
    
    public abstract class NativeQuery : AbstractQuery
    {
        public abstract bool Match(object @object);

        public abstract Type GetObjectType();

        public virtual Type[] GetObjectTypes()
        {
            var classes = new Type[1];
            classes[0] = GetObjectType();
            return classes;
        }

        public virtual string[] GetIndexFields()
        {
            return new string[0];
        }

        public override void SetExecutionPlan(IQueryExecutionPlan plan)
        {
            ExecutionPlan = plan;
        }
    }
}
