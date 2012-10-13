using System.Collections;
using System.Diagnostics;
using System.Text;

namespace NDatabase2.Odb.Core.Query.Criteria
{
    
    public sealed class Or : ComposedExpression
    {
        public override bool Match(object @object)
        {
            IEnumerator iterator = Criteria.GetEnumerator();
            while (iterator.MoveNext())
            {
                var criterion = (ICriterion) iterator.Current;
                Debug.Assert(criterion != null, "criterion != null");

                // For OR Expression, if one is true, then the whole expression
                // will be true
                if (criterion.Match(@object))
                    return true;
            }
            return false;
        }

        public override string ToString()
        {
            var buffer = new StringBuilder();
            IEnumerator iterator = Criteria.GetEnumerator();
            buffer.Append("(");
            var isFirst = true;

            while (iterator.MoveNext())
            {
                var criterion = (ICriterion) iterator.Current;
                Debug.Assert(criterion != null, "criterion != null");

                if (isFirst)
                {
                    buffer.Append(criterion);
                    isFirst = false;
                }
                else
                {
                    buffer.Append(" or ").Append(criterion);
                }
            }

            buffer.Append(")");
            return buffer.ToString();
        }
    }
}
