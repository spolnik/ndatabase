using System;
using System.Collections.Generic;

namespace NDatabase.Odb.Core.Layers.Layer2.Meta
{
    /// <summary>
    ///   To keep meta informations about an object
    /// </summary>
    /// <author>olivier smadja</author>
    
    public abstract class AbstractObjectInfo
    {
        /// <summary>
        ///   The Type of the object
        /// </summary>
        protected OdbType OdbType;

        /// <summary>
        ///   The Type Id of the object
        /// </summary>
        protected int OdbTypeId;

        /// <summary>
        ///   The position of the object
        /// </summary>
        protected long Position;

        protected AbstractObjectInfo(int typeId)
        {
            OdbTypeId = typeId;
        }

        protected AbstractObjectInfo(OdbType type)
        {
            if (type != null)
                OdbTypeId = type.GetId();

            OdbType = type;
        }

        public virtual bool IsNative()
        {
            return IsAtomicNativeObject() || IsArrayObject() || IsCollectionObject() || IsMapObject();
        }

        public virtual bool IsGroup()
        {
            return IsCollectionObject() || IsMapObject() || IsArrayObject();
        }

        public virtual bool IsNull()
        {
            return GetObject() == null;
        }

        public abstract object GetObject();

        public abstract void SetObject(object @object);

        public virtual int GetOdbTypeId()
        {
            return OdbTypeId;
        }

        public virtual void SetOdbTypeId(int odbTypeId)
        {
            OdbTypeId = odbTypeId;
        }

        public virtual long GetPosition()
        {
            return Position;
        }

        public virtual void SetPosition(long position)
        {
            Position = position;
        }

        public virtual OdbType GetOdbType()
        {
            return OdbType ?? (OdbType = OdbType.GetFromId(OdbTypeId));
        }

        public virtual void SetOdbType(OdbType odbType)
        {
            OdbType = odbType;
        }

        public virtual bool IsNonNativeObject()
        {
            return false;
        }

        public virtual bool IsAtomicNativeObject()
        {
            return false;
        }

        public virtual bool IsCollectionObject()
        {
            return false;
        }

        public virtual bool IsMapObject()
        {
            return false;
        }

        public virtual bool IsArrayObject()
        {
            return false;
        }

        public virtual bool IsDeletedObject()
        {
            return false;
        }

        public virtual bool IsObjectReference()
        {
            return false;
        }

        public virtual bool IsEnumObject()
        {
            return false;
        }

        public abstract AbstractObjectInfo CreateCopy(IDictionary<OID, AbstractObjectInfo> cache, bool onlyData);
    }
}
