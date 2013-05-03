using NDatabase.Core.Layers.Layer1.Introspector;

namespace NDatabase.Core.Layers.Layer3
{
    internal interface IClassInfoProvider
    {
        IObjectIntrospectionDataProvider GetClassInfoProvider();
    }
}