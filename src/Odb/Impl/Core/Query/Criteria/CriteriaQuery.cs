using System;
using System.Collections;
using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Odb.Core.Query;
using NDatabase.Odb.Core.Query.Criteria;
using NDatabase.Odb.Core.Query.Execution;
using NDatabase.Tool.Wrappers;
using NDatabase.Tool.Wrappers.List;

namespace NDatabase.Odb.Impl.Core.Query.Criteria
{
    [Serializable]
    public class CriteriaQuery : AbstractQuery
    {
        private ICriterion _criterion;
        private string _fullClassName;

        public CriteriaQuery(Type aClass, ICriterion criteria) : this((string) OdbClassUtil.GetFullName(aClass), criteria)
        {
        }

        public CriteriaQuery(Type aClass) : this((string) OdbClassUtil.GetFullName(aClass))
        {
        }

        public CriteriaQuery(ICriterion criteria)
        {
            if (criteria != null)
            {
                _criterion = criteria;
                _criterion.SetQuery(this);
            }
        }

        public CriteriaQuery(string aFullClassName)
        {
            _fullClassName = aFullClassName;
            _criterion = null;
        }

        public CriteriaQuery(string aFullClassName, ICriterion criteria)
        {
            _fullClassName = aFullClassName;
            if (criteria != null)
            {
                _criterion = criteria;
                _criterion.SetQuery(this);
            }
        }

        public virtual bool HasCriteria()
        {
            return _criterion != null;
        }

        public virtual bool Match(AbstractObjectInfo aoi)
        {
            if (_criterion == null)
                return true;
            return _criterion.Match(aoi);
        }

        public virtual bool Match(IDictionary map)
        {
            if (_criterion == null)
                return true;
            return _criterion.Match(map);
        }

        public override void SetFullClassName(Type type)
        {
            _fullClassName = OdbClassUtil.GetFullName(type);
        }

        public virtual string GetFullClassName()
        {
            return _fullClassName;
        }

        public virtual ICriterion GetCriteria()
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
                return new OdbArrayList<string>();
            return _criterion.GetAllInvolvedFields();
        }

        public virtual void SetCriterion(ICriterion criterion)
        {
            _criterion = criterion;
        }

        public override void SetExecutionPlan(IQueryExecutionPlan plan)
        {
            ExecutionPlan = plan;
        }
    }
}
