using System.Collections.Generic;
using NDatabase2.Odb.Core.Layers.Layer2.Meta;
using NDatabase2.Tool.Wrappers.List;

namespace NDatabase2.Odb.Core.Query.Criteria
{
    public abstract class ComposedExpression : AbstractExpression
    {
        protected IOdbList<IConstraint> Constraints;

        protected ComposedExpression()
        {
            Constraints = new OdbList<IConstraint>(5);
        }

        public virtual ComposedExpression Add(IConstraint constraint)
        {
            Constraints.Add(constraint);
            return this;
        }

        public override IOdbList<string> GetAllInvolvedFields()
        {
            var fields = new OdbList<string>(10);

            foreach (var constraint in Constraints)
                FilterOutDuplicates(constraint.GetAllInvolvedFields(), fields);

            return fields;
        }

        private static void FilterOutDuplicates(IEnumerable<string> allInvolvedFields, ICollection<string> fields)
        {
            foreach (var involvedField in allInvolvedFields)
            {
                if (!fields.Contains(involvedField))
                    fields.Add(involvedField);
            }
        }

        public bool IsEmpty()
        {
            return Constraints.IsEmpty();
        }

        public override AttributeValuesMap GetValues()
        {
            var map = new AttributeValuesMap();

            foreach (var constraint in Constraints)
                map.PutAll(constraint.GetValues());

            return map;
        }
    }
}
