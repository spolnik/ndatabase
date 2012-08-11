using System;
using System.Text;
using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Tool.Wrappers.List;

namespace NDatabase.Odb.Core.Query.Criteria
{
    [Serializable]
    public class Not : AbstractExpression
    {
        private readonly ICriterion _criterion;

        public Not(ICriterion criterion)
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

        public override void Ready()
        {
        }
    }
}
