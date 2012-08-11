using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace NDatabase.Odb.Core.Layers.Layer2.Meta
{
    /// <summary>
    ///   A meta representation of an Array
    /// </summary>
    /// <author>osmadja</author>
    [Serializable]
    public class ArrayObjectInfo : GroupObjectInfo
    {
        private int _componentTypeId;
        private string _realArrayComponentClassName;

        public ArrayObjectInfo(IEnumerable array) : base(array, Meta.OdbType.ArrayId)
        {
            _realArrayComponentClassName = Meta.OdbType.DefaultArrayComponentClassName;
        }

        public ArrayObjectInfo(IEnumerable array, OdbType type, int componentId) : base(array, type)
        {
            _realArrayComponentClassName = Meta.OdbType.DefaultArrayComponentClassName;
            _componentTypeId = componentId;
        }

        public virtual object[] GetArray()
        {
            return (object[]) TheObject;
        }

        public override string ToString()
        {
            if (TheObject != null)
            {
                var buffer = new StringBuilder();
                var array = GetArray();
                var length = array.Length;

                buffer.Append("[").Append(length).Append("]=(");

                for (var i = 0; i < length; i++)
                {
                    if (i != 0)
                        buffer.Append(",");

                    buffer.Append(array[i]);
                }

                buffer.Append(")");

                return buffer.ToString();
            }

            return "null array";
        }

        public override bool IsArrayObject()
        {
            return true;
        }

        public virtual string GetRealArrayComponentClassName()
        {
            return _realArrayComponentClassName;
        }

        public virtual void SetRealArrayComponentClassName(string realArrayComponentClassName)
        {
            _realArrayComponentClassName = realArrayComponentClassName;
        }

        public virtual int GetArrayLength()
        {
            return GetArray().Length;
        }

        public virtual int GetComponentTypeId()
        {
            return _componentTypeId;
        }

        public virtual void SetComponentTypeId(int componentTypeId)
        {
            _componentTypeId = componentTypeId;
        }

        public override AbstractObjectInfo CreateCopy(IDictionary<OID, AbstractObjectInfo> cache, bool onlyData)
        {
            var array = GetArray();
            var length = array.Length;

            var atomicNativeObjectInfos = new AtomicNativeObjectInfo[length];
            for (var i = 0; i < length; i++)
            {
                var aoi = (AbstractObjectInfo) array[i];
                atomicNativeObjectInfos[i] = aoi.CreateCopy(cache, onlyData) as AtomicNativeObjectInfo;
            }

            var arrayOfAoi = new ArrayObjectInfo(atomicNativeObjectInfos);
            arrayOfAoi.SetRealArrayComponentClassName(_realArrayComponentClassName);
            arrayOfAoi.SetComponentTypeId(_componentTypeId);

            return arrayOfAoi;
        }
    }
}
