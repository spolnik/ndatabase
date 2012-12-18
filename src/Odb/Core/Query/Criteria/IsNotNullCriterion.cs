namespace NDatabase2.Odb.Core.Query.Criteria
{
    public sealed class IsNotNullCriterion : AConstraint
    {
        public IsNotNullCriterion(string attributeName) : base(attributeName, null)
        {
        }

        public override bool Match(object valueToMatch)
        {
            return AsAttributeValuesMapValue(valueToMatch) != null;
        }
    }
}
