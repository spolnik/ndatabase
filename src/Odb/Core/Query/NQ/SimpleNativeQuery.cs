namespace NDatabase2.Odb.Core.Query.NQ
{
    public abstract class SimpleNativeQuery<T> : AbstractQuery<T> where T : class
    {
        public override bool Match(object @object)
        {
            return Match((T) @object);
        }

        public abstract bool Match(T @object);
    }
}
