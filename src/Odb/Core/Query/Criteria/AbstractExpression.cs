using System;
using NDatabase2.Odb.Core.Layers.Layer2.Meta;
using NDatabase2.Tool.Wrappers.List;

namespace NDatabase2.Odb.Core.Query.Criteria
{
    internal abstract class AbstractExpression : IInternalConstraint
    {
        private readonly IQuery _query;

        protected AbstractExpression(IQuery query)
        {
            _query = query;
            ((IInternalQuery)_query).Add(this);
        }

        #region IExpression Members

        public virtual bool CanUseIndex()
        {
            return false;
        }

        public abstract IOdbList<string> GetAllInvolvedFields();

        public abstract AttributeValuesMap GetValues();

        public abstract bool Match(object arg1);

        public IConstraint And(IConstraint with)
        {
            return new And(_query).Add(this).Add(with);
        }

        public IConstraint Or(IConstraint with)
        {
            return new Or(_query).Add(this).Add(with);
        }

        public IConstraint Not()
        {
            return new Not(_query, this);
        }

        public IConstraint Equals()
        {
            throw new NotSupportedException();
        }

        public IConstraint InvariantEquals()
        {
            throw new NotSupportedException();
        }

        public virtual IConstraint Like()
        {
            throw new NotSupportedException();
        }

        public virtual IConstraint InvariantLike()
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}
