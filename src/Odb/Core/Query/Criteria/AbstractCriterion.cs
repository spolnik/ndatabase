using NDatabase2.Odb.Core.Layers.Layer2.Meta;
using NDatabase2.Tool.Wrappers.List;

namespace NDatabase2.Odb.Core.Query.Criteria
{
    /// <summary>
    ///   An adapter for Criterion.
    /// </summary>
    public abstract class AbstractCriterion : IConstraint
    {
        /// <summary>
        ///   The name of the attribute involved by this criterion
        /// </summary>
        protected string AttributeName;

        /// <summary>
        ///   The query containing the criterion
        /// </summary>
        private IQuery _query;

        protected AbstractCriterion(string fieldName)
        {
            AttributeName = fieldName;
        }

        #region IConstraint Members

        public virtual bool CanUseIndex()
        {
            return false;
        }

        public abstract bool Match(object valueToMatch);

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

        /// <summary>
        ///   An abstract criterion only restrict one field =&gt; it returns a list of one field!
        /// </summary>
        /// <returns> The list of involved field of the criteria </returns>
        public virtual IOdbList<string> GetAllInvolvedFields()
        {
            IOdbList<string> list = new OdbList<string>(1);
            list.Add(AttributeName);
            return list;
        }

        public virtual AttributeValuesMap GetValues()
        {
            return new AttributeValuesMap();
        }

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

        public virtual bool Match(AbstractObjectInfo aoi)
        {
            var nnoi = (NonNativeObjectInfo) aoi;
            var aoiValue = nnoi.GetValueOf(AttributeName);
            return Match(aoiValue);
        }

        public virtual bool Match(AttributeValuesMap attributeValues)
        {
            return Match(attributeValues.GetAttributeValue(AttributeName));
        }

        /// <returns> The attribute involved in the criterion </returns>
        public virtual string GetAttributeName()
        {
            return AttributeName;
        }

        public virtual void SetAttributeName(string attributeName)
        {
            AttributeName = attributeName;
        }

        protected object AsAttributeValuesMapValue(object valueToMatch)
        {
            // If it is a AttributeValuesMap, then gets the real value from the map
            var attributeValues = valueToMatch as AttributeValuesMap;
            
            return attributeValues != null
                       ? attributeValues[AttributeName]
                       : valueToMatch;
        }
    }
}
