using NDatabase.Core.Introspector;

namespace NDatabase.Core.Layer3
{
    internal interface IClassInfoProvider
    {
        IObjectIntrospectionDataProvider GetClassInfoProvider();
    }
}