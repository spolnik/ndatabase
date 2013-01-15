using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Odb.Core.Layers.Layer3;

namespace NDatabase.Odb.Core.Layers.Layer2
{
    internal interface IInstanceBuilder
    {
        object BuildOneInstance(NonNativeObjectInfo objectInfo, IOdbCache cache);

        object BuildOneInstance(NonNativeObjectInfo objectInfo);
    }
}