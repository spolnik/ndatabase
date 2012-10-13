using System;

namespace NDatabase2.Odb.Core.Query.NQ
{
    public abstract class NativeQuery<T> : AbstractQuery<T> where T : class
    {
        public virtual Type[] GetObjectTypes()
        {
            var classes = new Type[1];
            classes[0] = UnderlyingType;
            return classes;
        }

        public virtual string[] GetIndexFields()
        {
            return new string[0];
        }

        public override bool Match(object @object)
        {
            return Match((T)@object);
        }

        public abstract bool Match(T @object);
    }
}
