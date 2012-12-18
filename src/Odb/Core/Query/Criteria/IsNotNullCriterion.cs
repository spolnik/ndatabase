namespace NDatabase2.Odb.Core.Query.Criteria
{
    internal sealed class IsNotNullCriterion : AConstraint
    {
        public IsNotNullCriterion(IQuery query, string attributeName) 
            : base(query, attributeName, null)
        {
        }

        public override bool Match(object valueToMatch)
        {
            return AsAttributeValuesMapValue(valueToMatch) != null;
        }
    }
}
