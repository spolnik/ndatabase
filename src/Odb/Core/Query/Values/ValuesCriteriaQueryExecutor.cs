using System;
using NDatabase2.Odb.Core.Layers.Layer2.Meta;
using NDatabase2.Odb.Core.Layers.Layer3;
using NDatabase2.Odb.Core.Query.Criteria;
using NDatabase2.Odb.Core.Query.Execution;
using NDatabase2.Tool.Wrappers.List;

namespace NDatabase2.Odb.Core.Query.Values
{
    internal sealed class ValuesCriteriaQueryExecutor<T> : GenericQueryExecutor where T : class
    {
        private CriteriaQuery<T> _criteriaQuery;
        private IOdbList<string> _involvedFields;

        private AttributeValuesMap _values;

        public ValuesCriteriaQueryExecutor(IQuery query, IStorageEngine engine) : base(query, engine)
        {
            _criteriaQuery = (CriteriaQuery<T>) query;
        }

        public override IQueryExecutionPlan GetExecutionPlan()
        {
            IQueryExecutionPlan plan = new CriteriaQueryExecutionPlan<T>(ClassInfo, (CriteriaQuery<T>) Query);
            return plan;
        }

        public override void PrepareQuery()
        {
            _criteriaQuery = (CriteriaQuery<T>) Query;
            ((IInternalQuery)_criteriaQuery).SetStorageEngine(StorageEngine);
            _involvedFields = _criteriaQuery.GetAllInvolvedFields();
        }

        public override bool MatchObjectWithOid(OID oid, bool returnObject, bool inMemory)
        {
            CurrentOid = oid;

            // Gets a map with the values with the fields involved in the query
            _values = ObjectReader.ReadObjectInfoValuesFromOID(ClassInfo, CurrentOid, true, _involvedFields,
                                                               _involvedFields, 0, _criteriaQuery.GetOrderByFieldNames());

            var objectMatches = true;
            if (!_criteriaQuery.IsForSingleOid())
            {
                // Then apply the query on the field values
                objectMatches = _criteriaQuery.Match(_values);
            }

            var objectInfoHeader = _values.GetObjectInfoHeader();
            // Stores the next position
            NextOID = objectInfoHeader.GetNextObjectOID();
            return objectMatches;
        }

        public override IComparable ComputeIndexKey(ClassInfo ci, ClassInfoIndex index)
        {
            return IndexTool.ComputeKey(ClassInfo, index, (CriteriaQuery<T>) Query);
        }

        public override object GetCurrentObjectMetaRepresentation()
        {
            return _values;
        }
    }
}
