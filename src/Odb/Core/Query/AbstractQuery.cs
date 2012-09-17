using System;
using NDatabase.Odb.Core.Layers.Layer1.Introspector;
using NDatabase.Odb.Core.Layers.Layer3;
using NDatabase.Odb.Core.Query.Execution;

namespace NDatabase.Odb.Core.Query
{
    
    public abstract class AbstractQuery : IQuery
    {
        protected IQueryExecutionPlan ExecutionPlan;
        protected string[] OrderByFields;

        /// <summary>
        ///   The OID attribute is used when the query must be restricted the object with this OID
        /// </summary>
        private OID _oidOfObjectToQuery;

        private OrderByConstants _orderByType;

        [NonPersistent]
        private IStorageEngine _storageEngine;

        protected AbstractQuery()
        {
            _orderByType = OrderByConstants.OrderByNone;
        }

        #region IQuery Members

        public virtual IQuery OrderByDesc(string fields)
        {
            _orderByType = OrderByConstants.OrderByDesc;
            OrderByFields = fields.Split(',');

            return this;
        }

        public virtual IQuery OrderByAsc(string fields)
        {
            _orderByType = OrderByConstants.OrderByAsc;
            OrderByFields = fields.Split(',');
            return this;
        }

        public virtual string[] GetOrderByFieldNames()
        {
            return OrderByFields;
        }

        public virtual OrderByConstants GetOrderByType()
        {
            return _orderByType;
        }

        public virtual bool HasOrderBy()
        {
            return !_orderByType.IsOrderByNone();
        }

        public virtual IStorageEngine GetStorageEngine()
        {
            return _storageEngine;
        }

        public virtual void SetStorageEngine(IStorageEngine storageEngine)
        {
            _storageEngine = storageEngine;
        }

        public virtual IQueryExecutionPlan GetExecutionPlan()
        {
            if (ExecutionPlan == null)
                throw new OdbRuntimeException(NDatabaseError.ExecutionPlanIsNullQueryHasNotBeenExecuted);
            return ExecutionPlan;
        }

        public virtual void SetExecutionPlan(IQueryExecutionPlan plan)
        {
            ExecutionPlan = plan;
        }

        public virtual OID GetOidOfObjectToQuery()
        {
            return _oidOfObjectToQuery;
        }

        /// <summary>
        ///   Returns true is query must apply on a single object OID
        /// </summary>
        public virtual bool IsForSingleOid()
        {
            return _oidOfObjectToQuery != null;
        }

        public abstract void SetFullClassName(Type type);

        #endregion

        public virtual void SetOrderByFields(string[] orderByFields)
        {
            OrderByFields = orderByFields;
        }

        public virtual void SetOrderByType(OrderByConstants orderByType)
        {
            _orderByType = orderByType;
        }

        public virtual void SetOidOfObjectToQuery(OID oidOfObjectToQuery)
        {
            _oidOfObjectToQuery = oidOfObjectToQuery;
        }
    }
}
