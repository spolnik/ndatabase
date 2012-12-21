using System;
using NDatabase2.Odb.Core.Layers.Layer1.Introspector;
using NDatabase2.Odb.Core.Layers.Layer3;
using NDatabase2.Odb.Core.Query.Criteria;
using NDatabase2.Odb.Core.Query.Execution;
using NDatabase2.Odb.Core.Query.Values;

namespace NDatabase2.Odb.Core.Query
{
    internal abstract class AbstractQuery : IInternalQuery
    {
        protected IInternalConstraint Constraint;
        private readonly Type _underlyingType;

        internal IQueryExecutionPlan ExecutionPlan;
        protected string[] OrderByFields;

        /// <summary>
        ///   The OID attribute is used when the query must be restricted the object with this OID
        /// </summary>
        private OID _oidOfObjectToQuery;

        private OrderByConstants _orderByType;

        [NonPersistent] private IStorageEngine _storageEngine;

        protected AbstractQuery(Type underlyingType)
        {
            if (underlyingType == null)
                throw new ArgumentNullException("underlyingType");

            if (underlyingType.IsValueType)
                throw new ArgumentException("Underlying type for query cannot to be value type.", "underlyingType");

            _orderByType = OrderByConstants.OrderByNone;
            _underlyingType = underlyingType;
        }

        #region IInternalQuery Members

        IQueryExecutionPlan IInternalQuery.GetExecutionPlan()
        {
            if (ExecutionPlan == null)
                throw new OdbRuntimeException(NDatabaseError.ExecutionPlanIsNullQueryHasNotBeenExecuted);
            return ExecutionPlan;
        }

        void IInternalQuery.SetExecutionPlan(IQueryExecutionPlan plan)
        {
            ExecutionPlan = plan;
        }

        IStorageEngine IInternalQuery.GetStorageEngine()
        {
            return _storageEngine;
        }

        void IInternalQuery.SetStorageEngine(IStorageEngine storageEngine)
        {
            _storageEngine = storageEngine;
        }

        #endregion

        #region IQuery Members

        public abstract IObjectSet<TItem> Execute<TItem>() where TItem : class;
        public abstract IObjectSet<TItem> Execute<TItem>(bool inMemory) where TItem : class;
        public abstract IObjectSet<TItem> Execute<TItem>(bool inMemory, int startIndex, int endIndex) where TItem : class;

        public IQuery OrderByDesc(string fields)
        {
            _orderByType = OrderByConstants.OrderByDesc;
            OrderByFields = fields.Split(',');

            return this;
        }

        public IQuery OrderByAsc(string fields)
        {
            _orderByType = OrderByConstants.OrderByAsc;
            OrderByFields = fields.Split(',');
            return this;
        }

        public string[] GetOrderByFieldNames()
        {
            return OrderByFields;
        }

        public OrderByConstants GetOrderByType()
        {
            return _orderByType;
        }

        public bool HasOrderBy()
        {
            return !_orderByType.IsOrderByNone();
        }

        public OID GetOidOfObjectToQuery()
        {
            return _oidOfObjectToQuery;
        }

        public abstract bool Match(object @object);

        public Type UnderlyingType
        {
            get { return _underlyingType; }
        }

        public abstract IQuery Descend(string attributeName);
        public abstract void Add(IConstraint criterion);

        /// <summary>
        ///   Returns true is query must apply on a single object OID
        /// </summary>
        public bool IsForSingleOid()
        {
            return _oidOfObjectToQuery != null;
        }

        public long Count()
        {
            var valuesCriteriaQuery = new ValuesCriteriaQuery(_underlyingType);
            valuesCriteriaQuery.Add(Constraint);

            var valuesQuery = valuesCriteriaQuery.Count("count");
            var values = ((IInternalQuery)this).GetStorageEngine().GetValues(valuesQuery, -1, -1);

            var count = (Decimal)values.NextValues().GetByIndex(0);

            return Decimal.ToInt64(count);
        }

        public abstract IConstraint SizeEq(int size);
        public abstract IConstraint SizeNe(int size);
        public abstract IConstraint SizeGt(int size);
        public abstract IConstraint SizeGe(int size);
        public abstract IConstraint SizeLt(int size);
        public abstract IConstraint SizeLe(int size);

        #endregion

        internal void SetOidOfObjectToQuery(OID oidOfObjectToQuery)
        {
            _oidOfObjectToQuery = oidOfObjectToQuery;
        }

        public abstract IConstraint Constrain(object value);
    }
}