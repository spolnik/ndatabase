using System.Collections.Generic;
using NDatabase2.Tool.Wrappers.Map;

namespace NDatabase2.Odb.Core.Layers.Layer2.Meta
{
    /// <summary>
    ///   Meta representation of a Map
    /// </summary>
    internal sealed class MapObjectInfo : GroupObjectInfo
    {
        private string _realMapClassName;

        public MapObjectInfo(IDictionary<AbstractObjectInfo, AbstractObjectInfo> map, string realMapClassName)
            : base(map, OdbType.MapId)
        {
            _realMapClassName = realMapClassName;
        }

        public MapObjectInfo(IDictionary<AbstractObjectInfo, AbstractObjectInfo> map, OdbType type,
                             string realMapClassName) : base(map, type)
        {
            _realMapClassName = realMapClassName;
        }

        public IDictionary<AbstractObjectInfo, AbstractObjectInfo> GetMap()
        {
            return (IDictionary<AbstractObjectInfo, AbstractObjectInfo>) TheObject;
        }

        public override string ToString()
        {
            return TheObject != null ? TheObject.ToString() : "null map";
        }

        public override bool IsMapObject()
        {
            return true;
        }

        public string GetRealMapClassName()
        {
            return _realMapClassName;
        }

        public void SetRealMapClassName(string realMapClassName)
        {
            _realMapClassName = realMapClassName;
        }

        public override AbstractObjectInfo CreateCopy(IDictionary<OID, AbstractObjectInfo> cache, bool onlyData)
        {
            var map = GetMap();
            
            IDictionary<AbstractObjectInfo, AbstractObjectInfo> newMap =
                new OdbHashMap<AbstractObjectInfo, AbstractObjectInfo>();

            foreach (var keyAoi in map.Keys)
            {
                var valueAoi = map[keyAoi];
                // create copies
                var keyAoiCopy = keyAoi.CreateCopy(cache, onlyData);
                valueAoi = valueAoi.CreateCopy(cache, onlyData);
                newMap.Add(keyAoiCopy, valueAoi);
            }

            return new MapObjectInfo(newMap, OdbType, _realMapClassName);
        }
    }
}
