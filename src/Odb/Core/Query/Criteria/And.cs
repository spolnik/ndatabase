using System;
using System.Collections;
using System.Diagnostics;
using System.Text;

namespace NDatabase.Odb.Core.Query.Criteria
{
    [Serializable]
    public sealed class And : ComposedExpression
    {
        public override bool Match(object @object)
        {
            IEnumerator iterator = Criteria.GetEnumerator();
            while (iterator.MoveNext())
            {
                var criterion = (ICriterion) iterator.Current;
                Debug.Assert(criterion != null, "criterion != null");

                // For AND Expression, if one is false, then the whole
                // expression will be false
                if (!criterion.Match(@object))
                    return false;
            }
            return true;
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
                    buffer.Append(" and ").Append(criterion);
                }
            }

            buffer.Append(")");
            return buffer.ToString();
        }

        public override bool CanUseIndex()
        {
            IEnumerator iterator = Criteria.GetEnumerator();

            while (iterator.MoveNext())
            {
                var criterion = (ICriterion) iterator.Current;
                Debug.Assert(criterion != null, "criterion != null");

                if (!criterion.CanUseIndex())
                    return false;
            }

            return true;
        }
    }
}
