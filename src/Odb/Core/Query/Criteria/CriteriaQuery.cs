using System.Collections;
using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Tool.Wrappers.List;

namespace NDatabase.Odb.Core.Query.Criteria
{
    public class CriteriaQuery<T> : AbstractQuery<T> where T : class
    {
        private ICriterion _criterion;
        
        public CriteriaQuery(ICriterion criteria)
        {
            if (criteria == null)
                return;

            _criterion = criteria;
            _criterion.SetQuery(this);
        }

        public CriteriaQuery() : this(null)
        {
        }

        public bool HasCriteria()
        {
            return _criterion != null;
        }

        public bool Match(AbstractObjectInfo aoi)
        {
            return _criterion == null || _criterion.Match(aoi);
        }

        public bool Match(IDictionary map)
        {
            return _criterion == null || _criterion.Match(map);
        }

        public override bool Match(object @object)
        {
            return Match((AbstractObjectInfo) @object);
        }

        public ICriterion GetCriteria()
        {
            return _criterion;
        }

        public override string ToString()
        {
            return _criterion == null
                       ? "no criterion"
                       : _criterion.ToString();
        }

        public virtual IOdbList<string> GetAllInvolvedFields()
        {
            return _criterion == null
                       ? new OdbList<string>()
                       : _criterion.GetAllInvolvedFields();
        }

        public void SetCriterion(ICriterion criterion)
        {
            _criterion = criterion;
        }
    }
}
