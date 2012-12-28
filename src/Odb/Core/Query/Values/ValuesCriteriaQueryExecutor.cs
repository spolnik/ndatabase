using System;
using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Odb.Core.Query.Criteria;
using NDatabase.Odb.Core.Query.Execution;
using NDatabase.Tool.Wrappers.List;
using NDatabase2.Odb;
using NDatabase2.Odb.Core.Layers.Layer3;

namespace NDatabase.Odb.Core.Query.Values
{
    internal sealed class ValuesCriteriaQueryExecutor : GenericQueryExecutor
    {
        private SodaQuery _sodaQuery;
        private IOdbList<string> _involvedFields;

        private AttributeValuesMap _values;

        public ValuesCriteriaQueryExecutor(IQuery query, IStorageEngine engine) : base(query, engine)
        {
            _sodaQuery = (SodaQuery) query;
        }

        protected override IQueryExecutionPlan GetExecutionPlan()
        {
            IQueryExecutionPlan plan = new CriteriaQueryExecutionPlan(ClassInfo, Query);
            return plan;
        }

        protected override void PrepareQuery()
        {
            _sodaQuery = Query;
            ((IInternalQuery) _sodaQuery).SetStorageEngine(StorageEngine);
            _involvedFields = _sodaQuery.GetAllInvolvedFields();
        }

        protected override bool MatchObjectWithOid(OID oid, bool returnObject, bool inMemory)
        {
            CurrentOid = oid;

            // Gets a map with the values with the fields involved in the query
            _values = ObjectReader.ReadObjectInfoValuesFromOID(ClassInfo, CurrentOid, true, _involvedFields,
                                                               _involvedFields, 0, _sodaQuery.GetOrderByFieldNames());

            var objectMatches = true;
            if (!_sodaQuery.IsForSingleOid())
            {
                // Then apply the query on the field values
                objectMatches = _sodaQuery.Match(_values);
            }

            var objectInfoHeader = _values.GetObjectInfoHeader();
            // Stores the next position
            NextOID = objectInfoHeader.GetNextObjectOID();
            return objectMatches;
        }

        protected override IComparable ComputeIndexKey(ClassInfo ci, ClassInfoIndex index)
        {
            return IndexTool.ComputeKey(ClassInfo, index, Query);
        }

        protected override object GetCurrentObjectMetaRepresentation()
        {
            return _values;
        }
    }
}