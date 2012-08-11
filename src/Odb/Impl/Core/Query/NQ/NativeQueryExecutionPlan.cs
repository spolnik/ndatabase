using System;
using System.Text;
using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Odb.Core.Query;
using NDatabase.Odb.Core.Query.Execution;
using NDatabase.Tool.Wrappers;

namespace NDatabase.Odb.Impl.Core.Query.NQ
{
    /// <summary>
    ///   A simple Criteria execution plan Check if the query can use index and tries to find the best index to be used
    /// </summary>
    /// <author>osmadja</author>
    [Serializable]
    public sealed class NativeQueryExecutionPlan : IQueryExecutionPlan
    {
        /// <summary>
        ///   to keep track of the end date time of the plan
        /// </summary>
        private long _end;

        /// <summary>
        ///   to keep track of the start date time of the plan
        /// </summary>
        private long _start;

        private bool _useIndex;
        
        public NativeQueryExecutionPlan(IQuery query)
        {
            query.SetExecutionPlan(this);
            Init();
        }

        #region IQueryExecutionPlan Members

        public ClassInfoIndex GetIndex()
        {
            return null;
        }

        public bool UseIndex()
        {
            return _useIndex;
        }

        public string GetDetails()
        {
            var buffer = new StringBuilder();
            buffer.Append("No index used, Execution time=").Append(GetDuration()).Append("ms");
            return buffer.ToString();
        }

        public void End()
        {
            _end = OdbTime.GetCurrentTimeInMs();
        }

        public long GetDuration()
        {
            return (_end - _start);
        }

        public void Start()
        {
            _start = OdbTime.GetCurrentTimeInMs();
        }

        #endregion

        private void Init()
        {
            _useIndex = false;
        }
    }
}
