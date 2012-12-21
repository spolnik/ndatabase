using NDatabase2.Odb.Core.Layers.Layer2.Meta;
using NDatabase2.Odb.Core.Query.Criteria.Evaluations;

namespace NDatabase2.Odb.Core.Query.Criteria
{
    internal class QueryConstraint : AConstraint
    {
        private IEvaluation _evaluation;

        public QueryConstraint(IQuery query, string fieldName, object theObject) 
            : base(query, fieldName, theObject)
        {
        }

        public override bool Match(object valueToMatch)
        {
            if (_evaluation == null)
                return true;

            return _evaluation.Evaluate(valueToMatch);
        }

        public override IConstraint Equals()
        {
            _evaluation = new EqualsEvaluation(TheObject, Query, AttributeName);
            return this;
        }

        public override IConstraint InvariantEquals()
        {
            _evaluation = new EqualsEvaluation(TheObject, Query, AttributeName, false);
            return this;
        }

        public override IConstraint Like()
        {
            _evaluation = new LikeEvaluation(TheObject, AttributeName);
            return this;
        }

        public override IConstraint InvariantLike()
        {
            _evaluation = new LikeEvaluation(TheObject, AttributeName, false);
            return this;
        }

        public override IConstraint Contains()
        {
            _evaluation = new ContainsEvaluation(TheObject, AttributeName, Query);
            return this;
        }

        public override IConstraint SmallerOrEqual()
        {
            _evaluation = new ComparisonEvaluation(TheObject, AttributeName, Query, ComparisonCirerion.ComparisonTypeLe);
            return this;
        }

        public override IConstraint GreaterOrEqual()
        {
            _evaluation = new ComparisonEvaluation(TheObject, AttributeName, Query, ComparisonCirerion.ComparisonTypeGe);
            return this;
        }

        public override IConstraint Greater()
        {
            _evaluation = new ComparisonEvaluation(TheObject, AttributeName, Query, ComparisonCirerion.ComparisonTypeGt);
            return this;
        }

        public override IConstraint Smaller()
        {
            _evaluation = new ComparisonEvaluation(TheObject, AttributeName, Query, ComparisonCirerion.ComparisonTypeLt);
            return this;
        }

        public override bool CanUseIndex()
        {
            return _evaluation is EqualsEvaluation;
        }

        public override string ToString()
        {
            return _evaluation == null ? base.ToString() : _evaluation.ToString();
        }

        public override AttributeValuesMap GetValues()
        {
            var equalsEvaluation = _evaluation as EqualsEvaluation;
            if (equalsEvaluation != null)
                return equalsEvaluation.GetValues();

            return new AttributeValuesMap();
        }
    }
}