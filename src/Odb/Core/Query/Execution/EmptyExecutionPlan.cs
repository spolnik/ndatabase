using System;
using NDatabase.Odb.Core.Layers.Layer2.Meta;

namespace NDatabase.Odb.Core.Query.Execution
{
    
    public sealed class EmptyExecutionPlan : IQueryExecutionPlan
    {
        #region IQueryExecutionPlan Members

        public void End()
        {
        }

        public string GetDetails()
        {
            return "empty plan";
        }

        public long GetDuration()
        {
            return 0;
        }

        public ClassInfoIndex GetIndex()
        {
            return null;
        }

        public void Start()
        {
        }

        public bool UseIndex()
        {
            return false;
        }

        #endregion
    }
}
