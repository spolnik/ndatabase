using System;
using System.Collections;
using NDatabase2.Odb.Core.Query.Values;
using NDatabase2.Tool.Wrappers.List;

namespace NDatabase2.Odb.Core.Query.Criteria
{
    public class CriteriaQuery<T> : AbstractQuery<T> where T : class
    {
        private IConstraint _criterion;
        private string _attributeName;

        public CriteriaQuery()
        {
        }

        public bool HasCriteria()
        {
            return _criterion != null;
        }

        public bool Match(IDictionary map)
        {
            return _criterion == null || _criterion.Match(map);
        }

        public override bool Match(object @object)
        {
            return _criterion.Match(@object);
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

        public override void Join(IConstraint criterion)
        {
            if (criterion == null)
                return;

            _criterion = criterion;
        }

        public override IQuery Descend(string attributeName)
        {
            if (string.IsNullOrEmpty(attributeName))
                throw new ArgumentNullException("attributeName", "Attribute name name cannot be null or empty");

            _attributeName = _attributeName == null ? attributeName : string.Join(".", _attributeName, attributeName);

            return this;
        }

        public override long Count()
        {
            var valuesCriteriaQuery = new ValuesCriteriaQuery<T>();
            valuesCriteriaQuery.Join(GetCriteria());

            var valuesQuery = valuesCriteriaQuery.Count("count");
            var values = ((IInternalQuery)this).GetStorageEngine().GetValues<T>(valuesQuery, -1, -1);

            var count = (Decimal)values.NextValues().GetByIndex(0);

            return Decimal.ToInt64(count);
        }

        public override IConstraint Equal<TItem>(TItem value)
        {
            return ApplyConstraint(new EqualCriterion(this, ApplyAttributeName(), value));
        }

        public override IConstraint LessOrEqual<TItem>(TItem value)
        {
            return
                ApplyConstraint(new ComparisonCriterion(this, ApplyAttributeName(), value,
                                                        ComparisonCirerion.ComparisonTypeLe));
        }

        public override IConstraint InvariantEqual(string value)
        {
            return
                ApplyConstraint(EqualCriterion.CreateInvartiantStringEqualCriterion(this, ApplyAttributeName(), value,
                                                                                    false));
        }

        public override IConstraint Like(string value)
        {
            return ApplyConstraint(new LikeCriterion(this, ApplyAttributeName(), value, true));
        }

        public override IConstraint InvariantLike(string value)
        {
            return ApplyConstraint(new LikeCriterion(this, ApplyAttributeName(), value, false));
        }

        public override IConstraint GreaterThan<TItem>(TItem value)
        {
            return
                ApplyConstraint(new ComparisonCriterion(this, ApplyAttributeName(), value, ComparisonCirerion.ComparisonTypeGt));
        }

        public override IConstraint GreaterOrEqual<TItem>(TItem value)
        {
            return
                ApplyConstraint(new ComparisonCriterion(this, ApplyAttributeName(), value, ComparisonCirerion.ComparisonTypeGe));
        }

        public override IConstraint LessThan<TItem>(TItem value)
        {
            return
                ApplyConstraint(new ComparisonCriterion(this, ApplyAttributeName(), value, ComparisonCirerion.ComparisonTypeLt));
        }

        public override IConstraint Contain<TItem>(TItem value)
        {
            return ApplyConstraint(new ContainsCriterion(this, ApplyAttributeName(), value));
        }

        public override IConstraint IsNull()
        {
            return ApplyConstraint(new IsNullCriterion(this, ApplyAttributeName()));
        }

        public override IConstraint IsNotNull()
        {
            return ApplyConstraint(new IsNotNullCriterion(this, ApplyAttributeName()));
        }

        public override IConstraint SizeEq(int size)
        {
            return ApplyConstraint(new CollectionSizeCriterion(this, ApplyAttributeName(), size, CollectionSizeCriterion.SizeEq));
        }

        public override IConstraint SizeNe(int size)
        {
            return ApplyConstraint(new CollectionSizeCriterion(this, ApplyAttributeName(), size, CollectionSizeCriterion.SizeNe));
        }

        public override IConstraint SizeGt(int size)
        {
            return ApplyConstraint(new CollectionSizeCriterion(this, ApplyAttributeName(), size, CollectionSizeCriterion.SizeGt));
        }

        public override IConstraint SizeGe(int size)
        {
            return ApplyConstraint(new CollectionSizeCriterion(this, ApplyAttributeName(), size, CollectionSizeCriterion.SizeGe));
        }

        public override IConstraint SizeLt(int size)
        {
            return ApplyConstraint(new CollectionSizeCriterion(this, ApplyAttributeName(), size, CollectionSizeCriterion.SizeLt));
        }

        public override IConstraint SizeLe(int size)
        {
            return ApplyConstraint(new CollectionSizeCriterion(this, ApplyAttributeName(), size, CollectionSizeCriterion.SizeLe));
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

        private IConstraint ApplyConstraint(IConstraint constraint)
        {
            Join(constraint);
            constraint.Ready();
            return constraint;
        }
    }
}