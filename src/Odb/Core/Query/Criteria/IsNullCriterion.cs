namespace NDatabase2.Odb.Core.Query.Criteria
{
    public sealed class IsNullCriterion : AbstractCriterion
    {
        public IsNullCriterion(string attributeName) : base(attributeName)
        {
        }

        public override bool Match(object valueToMatch)
        {
            return AsAttributeValuesMapValue(valueToMatch) == null;
        }
    }
}
