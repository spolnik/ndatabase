using System;
using System.Collections;
using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Odb.Core.Query.Execution;
using NDatabase.Tool.Wrappers.List;

namespace NDatabase.Odb.Core.Query.Criteria
{
    public class CriteriaQuery : AbstractQuery
    {
        private ICriterion _criterion;
        private readonly Type _underlyingType;

        public CriteriaQuery(Type underlyingType, ICriterion criteria)
        {
            _underlyingType = underlyingType;
            if (criteria == null)
                return;

            _criterion = criteria;
            _criterion.SetQuery(this);
        }

        public CriteriaQuery(Type underlyingType) : this(underlyingType, null)
        {
        }

        public static CriteriaQuery New<T>()
        {
            return new CriteriaQuery(typeof (T));
        }

        public bool HasCriteria()
        {
            return _criterion != null;
        }

        public bool Match(AbstractObjectInfo aoi)
        {
            if (_criterion == null)
                return true;
            return _criterion.Match(aoi);
        }

        public bool Match(IDictionary map)
        {
            if (_criterion == null)
                return true;
            return _criterion.Match(map);
        }

        public Type UnderlyingType
        {
            get { return _underlyingType; }
        }

        public ICriterion GetCriteria()
        {
            return _criterion;
        }

        public override string ToString()
        {
            if (_criterion == null)
                return "no criterion";
            return _criterion.ToString();
        }

        public virtual IOdbList<string> GetAllInvolvedFields()
        {
            if (_criterion == null)
                return new OdbList<string>();
            return _criterion.GetAllInvolvedFields();
        }

        public void SetCriterion(ICriterion criterion)
        {
            _criterion = criterion;
        }

        internal new void SetExecutionPlan(IQueryExecutionPlan plan)
        {
            ExecutionPlan = plan;
        }
    }
}
