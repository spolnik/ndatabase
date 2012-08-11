using System.Collections.Generic;
using NDatabase.Odb.Core.Layers.Layer1.Introspector;
using NDatabase.Tool.Wrappers.Map;

namespace NDatabase.Odb.Impl.Core.Layers.Layer1.Introspector
{
    /// <author>olivier</author>
    public class GetDependentObjectIntrospectingCallback : IIntrospectionCallback
    {
        private readonly OdbHashMap<object, object> _objects;

        public GetDependentObjectIntrospectingCallback()
        {
            _objects = new OdbHashMap<object, object>();
        }

        #region IIntrospectionCallback Members

        public virtual bool ObjectFound(object o)
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

        public virtual ICollection<object> GetObjects()
        {
            return _objects.Values;
        }
    }
}