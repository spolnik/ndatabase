using System;
using NDatabase2.Odb.Core.Layers.Layer1.Introspector;
using NDatabase2.Odb.Core.Layers.Layer3;
using NDatabase2.Odb.Core.Query.Criteria;
using NDatabase2.Odb.Core.Query.Execution;

namespace NDatabase2.Odb.Core.Query
{
    public abstract class AbstractQuery<T> : IQuery, IInternalQuery where T : class
    {
        private readonly Type _underlyingType = typeof (T);
        internal IQueryExecutionPlan ExecutionPlan;
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

        public virtual OID GetOidOfObjectToQuery()
        {
            return _oidOfObjectToQuery;
        }

        public abstract bool Match(object @object);

        public Type UnderlyingType
        {
            get { return _underlyingType; }
        }

        public abstract IConstraint Equal<TItem>(string attributeName, TItem value);
        public abstract void Constrain(IConstraint criterion);
        public abstract IConstraint LessOrEqual<TItem>(string attributeName, TItem value) where TItem : IComparable;
        public abstract IConstraint InvariantEqual(string attributeName, string value);
        public abstract IConstraint Like(string attributeName, string value);
        public abstract IConstraint InvariantLike(string attributeName, string value);
        public abstract IConstraint GreaterThan<TItem>(string attributeName, TItem value) where TItem : IComparable;
        public abstract IConstraint GreaterOrEqual<TItem>(string attributeName, TItem value) where TItem : IComparable;
        public abstract IConstraint LessThan<TItem>(string attributeName, TItem value) where TItem : IComparable;
        public abstract IConstraint Contain<TItem>(string attributeName, TItem value);
        public abstract IConstraint IsNull(string attributeName);
        public abstract IConstraint IsNotNull(string attributeName);
        public abstract IConstraint SizeEq(string attributeName, int size);
        public abstract IConstraint SizeNe(string attributeName, int size);
        public abstract IConstraint SizeGt(string attributeName, int size);
        public abstract IConstraint SizeGe(string attributeName, int size);
        public abstract IConstraint SizeLt(string attributeName, int size);
        public abstract IConstraint SizeLe(string attributeName, int size);

        /// <summary>
        ///   Returns true is query must apply on a single object OID
        /// </summary>
        public virtual bool IsForSingleOid()
        {
            return _oidOfObjectToQuery != null;
        }

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

        public abstract long Count();
    }
}
