using NDatabase2.Odb.Core.Layers.Layer2.Meta;
using NDatabase2.Tool.Wrappers.List;

namespace NDatabase2.Odb.Core.Layers.Layer3.Engine
{
    internal sealed class CheckMetaModelResult
    {
        private bool _modelHasBeenUpdated;

        private IOdbList<ClassInfoCompareResult> _results;

        public CheckMetaModelResult()
        {
            _modelHasBeenUpdated = false;
            _results = new OdbList<ClassInfoCompareResult>();
        }

        public bool IsModelHasBeenUpdated()
        {
            return _modelHasBeenUpdated;
        }

        public void SetModelHasBeenUpdated(bool modelHasBeenUpdated)
        {
            _modelHasBeenUpdated = modelHasBeenUpdated;
        }

        public IOdbList<ClassInfoCompareResult> GetResults()
        {
            return _results;
        }

        public void SetResults(IOdbList<ClassInfoCompareResult> results)
        {
            _results = results;
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
