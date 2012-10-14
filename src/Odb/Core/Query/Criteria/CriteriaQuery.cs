using System.Collections;
using NDatabase2.Odb.Core.Layers.Layer2.Meta;
using NDatabase2.Tool.Wrappers.List;

namespace NDatabase2.Odb.Core.Query.Criteria
{
    public class CriteriaQuery<T> : AbstractQuery<T> where T : class
    {
        private IConstraint _criterion;
        
        public CriteriaQuery(IConstraint criteria)
        {
            SetCriterion(criteria);
        }

        public CriteriaQuery()
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

        public IConstraint GetCriteria()
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

        public void SetCriterion(IConstraint criterion)
        {
            if (criterion == null)
                return;

            _criterion = criterion;
            _criterion.SetQuery(this);
        }
    }
}
