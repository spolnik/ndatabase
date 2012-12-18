namespace NDatabase2.Odb.Core.Query.Criteria
{
    public sealed class IsNullCriterion : AConstraint
    {
        public IsNullCriterion(string attributeName) : base(attributeName, null)
        {
        }

        public override bool Match(object valueToMatch)
        {
            return AsAttributeValuesMapValue(valueToMatch) == null;
        }
    }
}
