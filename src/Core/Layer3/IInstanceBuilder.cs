using NDatabase.Meta;

namespace NDatabase.Core.Layer3
{
    internal interface IInstanceBuilder
    {
        object BuildOneInstance(NonNativeObjectInfo objectInfo, IOdbCache cache);

        object BuildOneInstance(NonNativeObjectInfo objectInfo);
    }
}