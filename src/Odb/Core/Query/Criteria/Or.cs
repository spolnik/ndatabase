using System.Linq;
using System.Text;

namespace NDatabase2.Odb.Core.Query.Criteria
{
    public sealed class Or : ComposedExpression
    {
        public override bool Match(object @object)
        {
            return Constraints.Any(constraint => constraint.Match(@object));
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
                {
                    buffer.Append(" or ").Append(constraint);
                }
            }

            buffer.Append(")");
            return buffer.ToString();
        }
    }
}
