using NDatabase.Odb.Core.Layers.Layer2.Meta;

namespace NDatabase.Odb.Core.Layers.Layer2.Instance
{
    public interface IFullInstantiationHelper
    {
        object Instantiate(NonNativeObjectInfo nnoi);
    }
}