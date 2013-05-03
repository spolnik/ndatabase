using System.Collections.Generic;
using NDatabase.Core.Layers.Layer2.Meta;

namespace NDatabase.Services
{
    internal interface IMetaModelService
    {
        IEnumerable<ClassInfo> GetAllClasses();
    }
}