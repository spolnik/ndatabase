using System;
using NDatabase.Odb.Core.Layers.Layer2.Instance;
using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Odb.Core.Layers.Layer3;
using NDatabase.Odb.Core.Query.Execution;

namespace NDatabase.Odb.Core.Query.NQ
{
    public sealed class NativeQueryExecutor : GenericQueryExecutor
    {
        private readonly IInstanceBuilder _instanceBuilder;
        private object _currentObject;

        public NativeQueryExecutor(IQuery query, IStorageEngine engine, IInstanceBuilder instanceBuilder)
            : base(query, engine)
        {
            _instanceBuilder = instanceBuilder;
        }

        public override IQueryExecutionPlan GetExecutionPlan()
        {
            return new NativeQueryExecutionPlan(Query);
        }

        public override void PrepareQuery()
        {
        }

        /// <summary>
        ///   Check if the object at position currentPosition matches the query, returns true This method must compute the next object position and the orderBy key if it exists!
        /// </summary>
        public override bool MatchObjectWithOid(OID oid, bool loadObjectInfo, bool inMemory)
        {
            AbstractObjectInfo aoitemp = ObjectReader.ReadNonNativeObjectInfoFromOid(ClassInfo, oid, true, true);
            var objectMatches = false;
            if (!aoitemp.IsDeletedObject())
            {
                CurrentNnoi = (NonNativeObjectInfo) aoitemp;
                _currentObject = _instanceBuilder.BuildOneInstance(CurrentNnoi);
                objectMatches = Query == null || QueryManager.Match(Query, _currentObject);
                NextOID = CurrentNnoi.GetNextObjectOID();
            }
            return objectMatches;
        }

        public override IComparable ComputeIndexKey(ClassInfo ci, ClassInfoIndex index)
        {
            return null;
        }

        public IComparable BuildOrderByKey()
        {
            return IndexTool.BuildIndexKey("OrderBy", CurrentNnoi, QueryManager.GetOrderByAttributeIds(ClassInfo, Query));
        }

        
        public object GetCurrentInstance()
        {
            return _currentObject;
        }

        public override object GetCurrentObjectMetaRepresentation()
        {
            return CurrentNnoi;
        }
    }
}
