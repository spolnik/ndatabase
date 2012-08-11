using System;
using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Odb.Core.Layers.Layer3;
using NDatabase.Odb.Core.Query;
using NDatabase.Odb.Core.Query.Execution;
using NDatabase.Odb.Impl.Core.Query.Criteria;
using NDatabase.Tool.Wrappers.List;

namespace NDatabase.Odb.Impl.Core.Query.Values
{
    public class ValuesCriteriaQueryExecutor : GenericQueryExecutor
    {
        private CriteriaQuery _criteriaQuery;
        private IOdbList<string> _involvedFields;

        private AttributeValuesMap _values;

        public ValuesCriteriaQueryExecutor(IQuery query, IStorageEngine engine) : base(query, engine)
        {
            _criteriaQuery = (CriteriaQuery) query;
        }

        public override IQueryExecutionPlan GetExecutionPlan()
        {
            IQueryExecutionPlan plan = new CriteriaQueryExecutionPlan(ClassInfo, (CriteriaQuery) Query);
            return plan;
        }

        public override void PrepareQuery()
        {
            _criteriaQuery = (CriteriaQuery) Query;
            _criteriaQuery.SetStorageEngine(StorageEngine);
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
                objectMatches = CriteriaQueryManager.Match(_criteriaQuery, _values);
            }

            var objectInfoHeader = _values.GetObjectInfoHeader();
            // Stores the next position
            NextOID = objectInfoHeader.GetNextObjectOID();
            return objectMatches;
        }

        public override IComparable ComputeIndexKey(ClassInfo ci, ClassInfoIndex index)
        {
            return IndexTool.ComputeKey(ClassInfo, index, (CriteriaQuery) Query);
        }

        public override object GetCurrentObjectMetaRepresentation()
        {
            return _values;
        }
    }
}
