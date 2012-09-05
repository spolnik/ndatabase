using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Tool.Wrappers.List;

namespace NDatabase.Odb.Core.Query.Criteria
{
    
    public abstract class AbstractExpression : IExpression
    {
        private IQuery _query;

        #region IExpression Members

        /// <summary>
        ///   Gets thes whole query
        /// </summary>
        /// <returns> The owner query </returns>
        public virtual IQuery GetQuery()
        {
            return _query;
        }

        public virtual void SetQuery(IQuery query)
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

        public abstract void Ready();

        #endregion
    }
}
