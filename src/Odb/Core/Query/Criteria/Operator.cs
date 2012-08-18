using System;

namespace NDatabase.Odb.Core.Query.Criteria
{
    [Serializable]
    public sealed class Operator
    {
        public static readonly Operator Equal = new Operator("=");

        public static readonly Operator Contain = new Operator("contain");

        public static readonly Operator Like = new Operator("like");

        public static readonly Operator CaseInsentiveLike = new Operator("ilike");

        public static readonly Operator GreaterThan = new Operator(">");

        public static readonly Operator GreaterOrEqual = new Operator(">=");

        public static readonly Operator LessThan = new Operator("<");

        public static readonly Operator LessOrEqual = new Operator("<=");

        private readonly string _name;

        protected Operator(string name)
        {
            _name = name;
        }

        public string GetName()
        {
            return _name;
        }

        public override string ToString()
        {
            return _name;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Operator))
                return false;

            var @operator = (Operator) obj;
            return _name.Equals(@operator._name);
        }
    }
}
