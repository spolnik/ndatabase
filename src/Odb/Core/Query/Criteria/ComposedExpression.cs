using System.Collections.Generic;
using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Tool.Wrappers.List;

namespace NDatabase.Odb.Core.Query.Criteria
{
    internal abstract class ComposedExpression : AbstractExpression
    {
        protected readonly IOdbList<IConstraint> Constraints;

        protected ComposedExpression(IQuery query) : base(query)
        {
            Constraints = new OdbList<IConstraint>(5);
        }

        public ComposedExpression Add(IConstraint constraint)
        {
            Constraints.Add(constraint);
            return this;
        }

        public override IOdbList<string> GetAllInvolvedFields()
        {
            var fields = new OdbList<string>(10);

            foreach (var constraint in Constraints)
                FilterOutDuplicates(((IInternalConstraint)constraint).GetAllInvolvedFields(), fields);

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
                map.PutAll(((IInternalConstraint)constraint).GetValues());

            return map;
        }
    }
}
