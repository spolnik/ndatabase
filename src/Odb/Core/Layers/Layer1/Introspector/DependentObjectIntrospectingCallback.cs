using System.Collections.Generic;
using NDatabase2.Tool.Wrappers.Map;

namespace NDatabase2.Odb.Core.Layers.Layer1.Introspector
{
    public sealed class DependentObjectIntrospectingCallback : IIntrospectionCallback
    {
        private readonly OdbHashMap<object, object> _objects;

        public DependentObjectIntrospectingCallback()
        {
            _objects = new OdbHashMap<object, object>();
        }

        #region IIntrospectionCallback Members

        public bool ObjectFound(object o)
        {
            if (o == null)
            {
                return false;
            }
            if (_objects.ContainsKey(o))
            {
                return false;
            }
            _objects.Add(o, o);
            return true;
        }

        #endregion

        public ICollection<object> GetObjects()
        {
            return _objects.Values;
        }
    }
}