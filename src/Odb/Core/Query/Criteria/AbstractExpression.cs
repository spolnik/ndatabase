using NDatabase2.Odb.Core.Layers.Layer2.Meta;
using NDatabase2.Tool.Wrappers.List;

namespace NDatabase2.Odb.Core.Query.Criteria
{
    public abstract class AbstractExpression : IConstraint
    {
        private IQuery _query;

        #region IExpression Members

        /// <summary>
        ///   Gets thes whole query
        /// </summary>
        /// <returns> The owner query </returns>
        public IQuery GetQuery()
        {
            return _query;
        }

        public void SetQuery(IQuery query)
        {
            _query = query;
        }

        public virtual bool CanUseIndex()
        {
            return false;
        }

        public abstract IOdbList<string> GetAllInvolvedFields();

        public abstract AttributeValuesMap GetValues();

        public abstract bool Match(object arg1);

        public virtual void Ready()
        {
        }

        public IConstraint And(IConstraint with)
        {
            return new And().Add(this).Add(with);
        }

        public IConstraint Or(IConstraint with)
        {
            return new Or().Add(this).Add(with);
        }

        public IConstraint Not()
        {
            return new Not(this);
        }

        #endregion
    }
}
