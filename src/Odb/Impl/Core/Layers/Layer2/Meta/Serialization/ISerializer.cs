namespace NDatabase.Odb.Impl.Core.Layers.Layer2.Meta.Serialization
{
    public interface ISerializer
    {
        string ToString(object @object);

        
        object FromString(string data);
    }
}