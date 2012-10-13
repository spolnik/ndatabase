using System;
using NDatabase2.Odb.Core.Layers.Layer2.Meta;
using NDatabase2.Odb.Core.Layers.Layer3;
using NDatabase2.Odb.Core.Query.Execution;
using NDatabase2.Tool.Wrappers.List;

namespace NDatabase2.Odb.Core.Query.Criteria
{
    internal sealed class CriteriaQueryExecutor<T> : GenericQueryExecutor where T : class 
    {
        private CriteriaQuery<T> _criteriaQuery;
        private IOdbList<string> _involvedFields;

        public CriteriaQueryExecutor(IQuery query, IStorageEngine engine) : base(query, engine)
        {
            _criteriaQuery = (CriteriaQuery<T>) query;
        }

        public override IQueryExecutionPlan GetExecutionPlan()
        {
            return new CriteriaQueryExecutionPlan<T>(ClassInfo, (CriteriaQuery<T>)Query);
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
            var tmpCache = Session.GetTmpCache();
            try
            {
                ObjectInfoHeader objectInfoHeader;

                if (!_criteriaQuery.HasCriteria())
                {
                    // true, false = use cache, false = do not return object
                    // TODO Warning setting true to useCache will put all objects in the cache
                    // This is not a good idea for big queries!, But use cache=true
                    // resolves when object have not been committed yet!
                    // for big queries, user should use a LazyCache!
                    if (inMemory)
                    {
                        CurrentNnoi = ObjectReader.ReadNonNativeObjectInfoFromOid(ClassInfo, CurrentOid, true,
                                                                                  returnObject);
                        if (CurrentNnoi.IsDeletedObject())
                            return false;
                        CurrentOid = CurrentNnoi.GetOid();
                        NextOID = CurrentNnoi.GetNextObjectOID();
                    }
                    else
                    {
                        objectInfoHeader = ObjectReader.ReadObjectInfoHeaderFromOid(CurrentOid, false);
                        NextOID = objectInfoHeader.GetNextObjectOID();
                    }
                    return true;
                }

                // Gets a map with the values with the fields involved in the query
                var attributeValues = ObjectReader.ReadObjectInfoValuesFromOID(ClassInfo, CurrentOid, true,
                                                                               _involvedFields, _involvedFields, 0,
                                                                               _criteriaQuery.GetOrderByFieldNames());

                // Then apply the query on the field values
                var objectMatches = _criteriaQuery.Match(attributeValues);

                if (objectMatches)
                {
                    // Then load the entire object
                    // true, false = use cache
                    CurrentNnoi = ObjectReader.ReadNonNativeObjectInfoFromOid(ClassInfo, CurrentOid, true, returnObject);
                    CurrentOid = CurrentNnoi.GetOid();
                }

                objectInfoHeader = attributeValues.GetObjectInfoHeader();
                // Stores the next position
                NextOID = objectInfoHeader.GetNextObjectOID();
                return objectMatches;
            }
            finally
            {
                tmpCache.ClearObjectInfos();
            }
        }

        public override IComparable ComputeIndexKey(ClassInfo ci, ClassInfoIndex index)
        {
            var query = (CriteriaQuery<T>) Query;
            var values = query.GetCriteria().GetValues();

            // if values.hasOid() is true, this means that we are working of the full object,
            // the index key is then the oid and not the object itself
            if (values.HasOid())
                return new SimpleCompareKey(values.GetOid());
            return IndexTool.ComputeKey(ClassInfo, index, (CriteriaQuery<T>) Query);
        }

        public override object GetCurrentObjectMetaRepresentation()
        {
            return CurrentNnoi;
        }
    }
}
