using System.Text;
using NDatabase.Odb.Core.Layers.Layer1.Introspector;
using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Odb.Core.Query.Execution;
using NDatabase.Tool.Wrappers;
using NDatabase.Tool.Wrappers.List;

namespace NDatabase.Odb.Core.Query.Criteria
{
    /// <summary>
    ///   A simple Criteria execution plan Check if the query can use index and tries to find the best index to be used
    /// </summary>
    internal sealed class CriteriaQueryExecutionPlan<T> : IQueryExecutionPlan where T : class
    {
        [NonPersistent]
        private readonly ClassInfo _classInfo;

        [NonPersistent]
        private readonly CriteriaQuery<T> _query;

        [NonPersistent]
        private ClassInfoIndex _classInfoIndex;

        /// <summary>
        ///   To keep the execution detail
        /// </summary>
        private string _details;

        /// <summary>
        ///   to keep track of the end date time of the plan
        /// </summary>
        private long _end;

        /// <summary>
        ///   to keep track of the start date time of the plan
        /// </summary>
        private long _start;

        private bool _useIndex;

        public CriteriaQueryExecutionPlan()
        {
        }

        public CriteriaQueryExecutionPlan(ClassInfo classInfo, CriteriaQuery<T> query)
        {
            _classInfo = classInfo;
            _query = query;
            ((IInternalQuery)_query).SetExecutionPlan(this);
            Init();
        }

        #region IQueryExecutionPlan Members

        public ClassInfoIndex GetIndex()
        {
            return _classInfoIndex;
        }

        public bool UseIndex()
        {
            return _useIndex;
        }

        public string GetDetails()
        {
            if (_details != null)
                return _details;

            var buffer = new StringBuilder();
            if (_classInfoIndex == null)
            {
                buffer.Append("No index used, Execution time=").Append(GetDuration()).Append("ms");
                return buffer.ToString();
            }

            return
                buffer.Append("Following indexes have been used : ").Append(_classInfoIndex.Name).Append(
                    ", Execution time=").Append(GetDuration()).Append("ms").ToString();
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
            _start = 0;
            _end = 0;

            // for instance, only manage index for one field query using 'equal'
            if (_classInfo.HasIndex() && _query.HasCriteria() && CanUseIndex(_query.GetCriteria()))
            {
                var fields = _query.GetAllInvolvedFields();
                if (fields.IsEmpty())
                    _useIndex = false;
                else
                {
                    var fieldIds = GetAllInvolvedFieldIds(fields);
                    _classInfoIndex = _classInfo.GetIndexForAttributeIds(fieldIds);
                    if (_classInfoIndex != null)
                        _useIndex = true;
                }
            }

            // Keep the detail
            _details = GetDetails();
        }

        /// <summary>
        ///   Transform a list of field names into a list of field ids
        /// </summary>
        /// <param name="fields"> </param>
        /// <returns> The array of field ids </returns>
        private int[] GetAllInvolvedFieldIds(IOdbList<string> fields)
        {
            var nbFields = fields.Count;
            var fieldIds = new int[nbFields];
            for (var i = 0; i < nbFields; i++)
                fieldIds[i] = _classInfo.GetAttributeId(fields[i]);

            return fieldIds;
        }

        private static bool CanUseIndex(ICriterion criteria)
        {
            return criteria.CanUseIndex();
        }
    }
}
