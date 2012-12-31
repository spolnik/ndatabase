using System.Collections.Generic;
using NDatabase.Odb.Core.Layers.Layer2.Meta;

namespace NDatabase.Odb.Core.Layers.Layer3.Engine
{
    internal sealed class CheckMetaModelResult
    {
        private readonly IList<ClassInfoCompareResult> _results;

        public CheckMetaModelResult()
        {
            _results = new List<ClassInfoCompareResult>();
        }

        public void SetModelHasBeenUpdated(bool modelHasBeenUpdated)
        {
        }

        public IList<ClassInfoCompareResult> GetResults()
        {
            return _results;
        }

        public void Add(ClassInfoCompareResult result)
        {
            _results.Add(result);
        }

        public int Size()
        {
            return _results.Count;
        }
    }
}
