using System;

namespace NDatabase.Odb.Core.Query.NQ
{
    public abstract class SimpleNativeQuery<T> : AbstractQuery where T : class
    {
        private readonly Type _type = typeof (T);

        public override Type UnderlyingType
        {
            get { return _type; }
        }

        public override bool Match(object @object)
        {
            return Match((T) @object);
        }

        public abstract bool Match(T @object);
    }
}
