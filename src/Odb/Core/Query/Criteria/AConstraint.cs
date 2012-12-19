using System;
using NDatabase2.Odb.Core.Layers.Layer2.Meta;
using NDatabase2.Tool.Wrappers.List;

namespace NDatabase2.Odb.Core.Query.Criteria
{
    /// <summary>
    ///   An adapter for Criterion.
    /// </summary>
    internal abstract class AConstraint : IInternalConstraint
    {
        /// <summary>
        ///   The name of the attribute involved by this criterion
        /// </summary>
        protected readonly string AttributeName;

        protected readonly object TheObject;

        /// <summary>
        ///   The query containing the criterion
        /// </summary>
        protected readonly IQuery Query;

        protected AConstraint(IQuery query, string fieldName, object theObject)
        {
            if (query == null)
                throw new ArgumentNullException("query");

            if (string.IsNullOrEmpty(fieldName))
                throw new ArgumentNullException("fieldName");

            Query = query;
            AttributeName = fieldName;
            TheObject = theObject;
        }

        #region IConstraint Members

        public virtual bool CanUseIndex()
        {
            return false;
        }

        public abstract bool Match(object valueToMatch);

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

        public IConstraint And(IConstraint with)
        {
            return new And(Query).Add(this).Add(with);
        }

        public IConstraint Or(IConstraint with)
        {
            return new Or(Query).Add(this).Add(with);
        }

        public IConstraint Not()
        {
            return new Not(Query, this);
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

        protected object AsAttributeValuesMapValue(object valueToMatch)
        {
            // If it is a AttributeValuesMap, then gets the real value from the map
            var attributeValues = valueToMatch as AttributeValuesMap;
            
            return attributeValues != null
                       ? attributeValues[AttributeName]
                       : valueToMatch;
        }

        protected bool IsNative()
        {
            return TheObject == null || OdbType.IsNative(TheObject.GetType());
        }
    }
}
