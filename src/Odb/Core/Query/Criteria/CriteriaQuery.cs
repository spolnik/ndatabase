using System;
using System.Collections;
using NDatabase2.Odb.Core.Query.Values;
using NDatabase2.Tool.Wrappers.List;

namespace NDatabase2.Odb.Core.Query.Criteria
{
    public class CriteriaQuery<T> : AbstractQuery<T> where T : class
    {
        private IConstraint _criterion;

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

        public override void Constrain(IConstraint criterion)
        {
            if (criterion == null)
                return;

            _criterion = criterion;
            _criterion.SetQuery(this);
        }

        public override long Count()
        {
            var valuesCriteriaQuery = new ValuesCriteriaQuery<T>();
            valuesCriteriaQuery.Constrain(GetCriteria());

            var valuesQuery = valuesCriteriaQuery.Count("count");
            var values = ((IInternalQuery)this).GetStorageEngine().GetValues<T>(valuesQuery, -1, -1);

            var count = (Decimal)values.NextValues().GetByIndex(0);

            return Decimal.ToInt64(count);
        }

        public override IConstraint Equal<TItem>(string attributeName, TItem value)
        {
            return ApplyConstraint(new EqualCriterion<TItem>(attributeName, value));
        }

        public override IConstraint LessOrEqual<TItem>(string attributeName, TItem value)
        {
            return
                ApplyConstraint(new ComparisonCriterion<TItem>(attributeName, value, ComparisonCirerion.ComparisonTypeLe));
        }

        public override IConstraint InvariantEqual(string attributeName, string value)
        {
            return
                ApplyConstraint(EqualCriterion<string>.CreateInvartiantStringEqualCriterion(attributeName, value, false));
        }

        public override IConstraint Like(string attributeName, string value)
        {
            return ApplyConstraint(new LikeCriterion(attributeName, value, true));
        }

        public override IConstraint InvariantLike(string attributeName, string value)
        {
            return ApplyConstraint(new LikeCriterion(attributeName, value, false));
        }

        public override IConstraint GreaterThan<TItem>(string attributeName, TItem value)
        {
            return
                ApplyConstraint(new ComparisonCriterion<TItem>(attributeName, value, ComparisonCirerion.ComparisonTypeGt));
        }

        public override IConstraint GreaterOrEqual<TItem>(string attributeName, TItem value)
        {
            return
                ApplyConstraint(new ComparisonCriterion<TItem>(attributeName, value, ComparisonCirerion.ComparisonTypeGe));
        }

        public override IConstraint LessThan<TItem>(string attributeName, TItem value)
        {
            return
                ApplyConstraint(new ComparisonCriterion<TItem>(attributeName, value, ComparisonCirerion.ComparisonTypeLt));
        }

        public override IConstraint Contain<TItem>(string attributeName, TItem value)
        {
            return ApplyConstraint(new ContainsCriterion<TItem>(attributeName, value));
        }

        public override IConstraint IsNull(string attributeName)
        {
            return ApplyConstraint(new IsNullCriterion(attributeName));
        }

        public override IConstraint IsNotNull(string attributeName)
        {
            return ApplyConstraint(new IsNotNullCriterion(attributeName));
        }

        public override IConstraint SizeEq(string attributeName, int size)
        {
            return ApplyConstraint(new CollectionSizeCriterion(attributeName, size, CollectionSizeCriterion.SizeEq));
        }

        public override IConstraint SizeNe(string attributeName, int size)
        {
            return ApplyConstraint(new CollectionSizeCriterion(attributeName, size, CollectionSizeCriterion.SizeNe));
        }

        public override IConstraint SizeGt(string attributeName, int size)
        {
            return ApplyConstraint(new CollectionSizeCriterion(attributeName, size, CollectionSizeCriterion.SizeGt));
        }

        public override IConstraint SizeGe(string attributeName, int size)
        {
            return ApplyConstraint(new CollectionSizeCriterion(attributeName, size, CollectionSizeCriterion.SizeGe));
        }

        public override IConstraint SizeLt(string attributeName, int size)
        {
            return ApplyConstraint(new CollectionSizeCriterion(attributeName, size, CollectionSizeCriterion.SizeLt));
        }

        public override IConstraint SizeLe(string attributeName, int size)
        {
            return ApplyConstraint(new CollectionSizeCriterion(attributeName, size, CollectionSizeCriterion.SizeLe));
        }

        public override IObjectSet<TItem> Execute<TItem>()
        {
            return ((IInternalQuery)this).GetStorageEngine().GetObjects<TItem>(this, true, -1, -1);
        }

        public override IObjectSet<TItem> Execute<TItem>(bool inMemory)
        {
            return ((IInternalQuery)this).GetStorageEngine().GetObjects<TItem>(this, inMemory, -1, -1);
        }

        private IConstraint ApplyConstraint(IConstraint constraint)
        {
            Constrain(constraint);
            constraint.Ready();
            return constraint;
        }
    }
}