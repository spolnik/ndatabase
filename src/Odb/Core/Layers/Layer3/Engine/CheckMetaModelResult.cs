using System;
using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Tool.Wrappers.List;

namespace NDatabase.Odb.Core.Layers.Layer3.Engine
{
    /// <author>olivier</author>
    [Serializable]
    public class CheckMetaModelResult
    {
        private bool _modelHasBeenUpdated;

        private IOdbList<ClassInfoCompareResult> _results;

        public CheckMetaModelResult()
        {
            _modelHasBeenUpdated = false;
            _results = new OdbArrayList<ClassInfoCompareResult>();
        }

        public virtual bool IsModelHasBeenUpdated()
        {
            return _modelHasBeenUpdated;
        }

        public virtual void SetModelHasBeenUpdated(bool modelHasBeenUpdated)
        {
            _modelHasBeenUpdated = modelHasBeenUpdated;
        }

        public virtual IOdbList<ClassInfoCompareResult> GetResults()
        {
            return _results;
        }

        public virtual void SetResults(IOdbList<ClassInfoCompareResult> results)
        {
            _results = results;
        }

        public virtual void Add(ClassInfoCompareResult result)
        {
            _results.Add(result);
        }

        public virtual int Size()
        {
            return _results.Count;
        }
    }
}
