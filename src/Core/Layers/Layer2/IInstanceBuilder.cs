using NDatabase.Core.Layers.Layer2.Meta;
using NDatabase.Core.Layers.Layer3;

namespace NDatabase.Core.Layers.Layer2
{
    internal interface IInstanceBuilder
    {
        object BuildOneInstance(NonNativeObjectInfo objectInfo, IOdbCache cache);

        object BuildOneInstance(NonNativeObjectInfo objectInfo);
    }
}