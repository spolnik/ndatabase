namespace NDatabase2.Odb.Core.Query.Criteria
{
    public sealed class IsNotNullCriterion : AbstractCriterion
    {
        public IsNotNullCriterion(string attributeName) : base(attributeName)
        {
        }

        public override bool Match(object valueToMatch)
        {
            return AsAttributeValuesMapValue(valueToMatch) != null;
        }
    }
}
