namespace NDatabase2.Odb.Core.Query.Criteria
{
    internal sealed class IsNullCriterion : AConstraint
    {
        public IsNullCriterion(IQuery query, string attributeName) 
            : base(query, attributeName, null)
        {
        }

        public override bool Match(object valueToMatch)
        {
            return AsAttributeValuesMapValue(valueToMatch) == null;
        }
    }
}
