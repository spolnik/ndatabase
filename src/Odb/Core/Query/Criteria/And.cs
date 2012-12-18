using System.Linq;
using System.Text;

namespace NDatabase2.Odb.Core.Query.Criteria
{
    internal sealed class And : ComposedExpression
    {
        public And(IQuery query) : base(query)
        {
        }

        public override bool Match(object @object)
        {
            return Constraints.All(constraint => constraint.Match(@object));
        }

        public override bool CanUseIndex()
        {
            return Constraints.All(constraint => constraint.CanUseIndex());
        }

        public override string ToString()
        {
            var buffer = new StringBuilder();

            buffer.Append("(");
            var isFirst = true;

            foreach (var constraint in Constraints)
            {
                if (isFirst)
                {
                    buffer.Append(constraint);
                    isFirst = false;
                }
                else
                    buffer.Append(" and ").Append(constraint);
            }

            buffer.Append(")");
            return buffer.ToString();
        }
    }
}
