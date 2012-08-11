using System;
using System.Reflection;

namespace NDatabase.Odb.Core.Layers.Layer2.Instance
{
    public interface IClassPool
    {
        Type GetClass(string className);

        ConstructorInfo GetConstrutor(string className);

        void AddConstrutor(string className, ConstructorInfo constructor);

        void Reset();
    }
}
