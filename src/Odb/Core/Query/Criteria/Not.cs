using System.Text;
using NDatabase2.Odb.Core.Layers.Layer2.Meta;
using NDatabase2.Tool.Wrappers.List;

namespace NDatabase2.Odb.Core.Query.Criteria
{
    
    public sealed class Not : AbstractExpression
    {
        private readonly IConstraint _criterion;

        public Not(IConstraint criterion)
        {
            _criterion = criterion;
        }

        public override bool Match(object @object)
        {
            return !_criterion.Match(@object);
        }

        public override string ToString()
        {
            var buffer = new StringBuilder();
            buffer.Append(" not ").Append(_criterion);
            return buffer.ToString();
        }

        public override IOdbList<string> GetAllInvolvedFields()
        {
            return _criterion.GetAllInvolvedFields();
        }

        public override AttributeValuesMap GetValues()
        {
            return new AttributeValuesMap();
        }
    }
}
