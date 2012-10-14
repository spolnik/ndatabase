using System.Collections;
using System.Diagnostics;
using NDatabase2.Odb.Core.Layers.Layer2.Meta;
using NDatabase2.Tool.Wrappers.List;

namespace NDatabase2.Odb.Core.Query.Criteria
{
    
    public abstract class ComposedExpression : AbstractExpression
    {
        protected IOdbList<IConstraint> Criteria;

        protected ComposedExpression()
        {
            Criteria = new OdbList<IConstraint>(5);
        }

        public virtual ComposedExpression Add(IConstraint criterion)
        {
            Criteria.Add(criterion);
            return this;
        }

        public override IOdbList<string> GetAllInvolvedFields()
        {
            IEnumerator iterator = Criteria.GetEnumerator();
            IOdbList<string> fields = new OdbList<string>(10);
            while (iterator.MoveNext())
            {
                var criterion = (IConstraint) iterator.Current;
                Debug.Assert(criterion != null, "criterion != null");

                var allInvolvedFields = criterion.GetAllInvolvedFields();

                // check duplicate
                for (var i = 0; i < allInvolvedFields.Count; i++)
                {
                    var involvedFields = allInvolvedFields[i];
                    if (!fields.Contains(involvedFields))
                        fields.Add(involvedFields);
                }
            }

            return fields;
        }

        public virtual bool IsEmpty()
        {
            return Criteria.IsEmpty();
        }

        public override AttributeValuesMap GetValues()
        {
            var map = new AttributeValuesMap();
            IEnumerator iterator = Criteria.GetEnumerator();

            while (iterator.MoveNext())
            {
                var criterion = (IConstraint) iterator.Current;
                Debug.Assert(criterion != null, "criterion != null");

                map.PutAll(criterion.GetValues());
            }

            return map;
        }

        public virtual int GetNbCriteria()
        {
            return Criteria.Count;
        }

        public virtual IConstraint GetCriterion(int index)
        {
            return Criteria[index];
        }

        public override void Ready()
        {
        }
    }
}
