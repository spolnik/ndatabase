using System;
using System.Collections;
using NDatabase2.Tool.Wrappers.List;

namespace NDatabase2.Odb.Core.Query.Criteria
{
    internal class CriteriaQuery : AbstractQuery
    {
        private string _attributeName;

        public CriteriaQuery(Type underlyingType) 
            : base(underlyingType)
        {
        }

        public bool HasCriteria()
        {
            return Constraint != null;
        }

        public bool Match(IDictionary map)
        {
            return Constraint == null || Constraint.Match(map);
        }

        public override bool Match(object @object)
        {
            return Constraint.Match(@object);
        }

        public IConstraint GetCriteria()
        {
            return Constraint;
        }

        public override string ToString()
        {
            return Constraint == null
                       ? "no criterion"
                       : Constraint.ToString();
        }

        public virtual IOdbList<string> GetAllInvolvedFields()
        {
            return Constraint == null
                       ? new OdbList<string>()
                       : Constraint.GetAllInvolvedFields();
        }

        public override void Add(IConstraint criterion)
        {
            if (criterion == null)
                return;

            Constraint = (IInternalConstraint) criterion;
        }

        public override IQuery Descend(string attributeName)
        {
            if (string.IsNullOrEmpty(attributeName))
                throw new ArgumentNullException("attributeName", "Attribute name name cannot be null or empty");

            _attributeName = _attributeName == null ? attributeName : string.Join(".", _attributeName, attributeName);

            return this;
        }

        public override IConstraint Constrain(object value)
        {
            return new QueryConstraint(this, ApplyAttributeName(), value);
        }

        public override IConstraint LessOrEqual<TItem>(TItem value)
        {
            return
                new ComparisonCriterion(this, ApplyAttributeName(), value,
                                        ComparisonCirerion.ComparisonTypeLe);
        }

        public override IConstraint GreaterThan<TItem>(TItem value)
        {
            return
                new ComparisonCriterion(this, ApplyAttributeName(), value, ComparisonCirerion.ComparisonTypeGt);
        }

        public override IConstraint GreaterOrEqual<TItem>(TItem value)
        {
            return
                new ComparisonCriterion(this, ApplyAttributeName(), value, ComparisonCirerion.ComparisonTypeGe);
        }

        public override IConstraint LessThan<TItem>(TItem value)
        {
            return
                new ComparisonCriterion(this, ApplyAttributeName(), value, ComparisonCirerion.ComparisonTypeLt);
        }

        public override IConstraint Contain(object value)
        {
            return new ContainsCriterion(this, ApplyAttributeName(), value);
        }

        public override IConstraint SizeEq(int size)
        {
            return new CollectionSizeCriterion(this, ApplyAttributeName(), size, CollectionSizeCriterion.SizeEq);
        }

        public override IConstraint SizeNe(int size)
        {
            return new CollectionSizeCriterion(this, ApplyAttributeName(), size, CollectionSizeCriterion.SizeNe);
        }

        public override IConstraint SizeGt(int size)
        {
            return new CollectionSizeCriterion(this, ApplyAttributeName(), size, CollectionSizeCriterion.SizeGt);
        }

        public override IConstraint SizeGe(int size)
        {
            return new CollectionSizeCriterion(this, ApplyAttributeName(), size, CollectionSizeCriterion.SizeGe);
        }

        public override IConstraint SizeLt(int size)
        {
            return new CollectionSizeCriterion(this, ApplyAttributeName(), size, CollectionSizeCriterion.SizeLt);
        }

        public override IConstraint SizeLe(int size)
        {
            return new CollectionSizeCriterion(this, ApplyAttributeName(), size, CollectionSizeCriterion.SizeLe);
        }

        public override IObjectSet<TItem> Execute<TItem>()
        {
            return ((IInternalQuery)this).GetStorageEngine().GetObjects<TItem>(this, true, -1, -1);
        }

        public override IObjectSet<TItem> Execute<TItem>(bool inMemory)
        {
            return ((IInternalQuery)this).GetStorageEngine().GetObjects<TItem>(this, inMemory, -1, -1);
        }

        public override IObjectSet<TItem> Execute<TItem>(bool inMemory, int startIndex, int endIndex)
        {
            return ((IInternalQuery)this).GetStorageEngine().GetObjects<TItem>(this, inMemory, startIndex, endIndex);
        }

        private string ApplyAttributeName()
        {
            if (string.IsNullOrEmpty(_attributeName))
                throw new ArgumentException(
                    "Attribute name name cannot be null or empty. Use query.Descend(<param_name>) firstly.");

            var attributeName = String.Copy(_attributeName);
            _attributeName = null;

            return attributeName;
        }
    }
}