using System;
using NDatabase.Odb.Core.Layers.Layer2.Meta;

namespace NDatabase.Odb.Core.Query.Execution
{
    [Serializable]
    public class EmptyExecutionPlan : IQueryExecutionPlan
    {
        #region IQueryExecutionPlan Members

        public virtual void End()
        {
        }

        public virtual string GetDetails()
        {
            return "empty plan";
        }

        public virtual long GetDuration()
        {
            return 0;
        }

        public virtual ClassInfoIndex GetIndex()
        {
            return null;
        }

        public virtual void Start()
        {
        }

        public virtual bool UseIndex()
        {
            return false;
        }

        #endregion
    }
}
