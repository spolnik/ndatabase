using System;

namespace NDatabase.Odb.Core.Layers.Layer2.Instance
{
    public interface IClassPool
    {
        Type GetClass(string className);
        void Reset();
    }
}
