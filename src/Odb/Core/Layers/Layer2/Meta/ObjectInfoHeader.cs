using System;
using System.Text;
using NDatabase.Odb.Impl.Core.Layers.Layer3.Engine;
using NDatabase.Tool.Wrappers;

namespace NDatabase.Odb.Core.Layers.Layer2.Meta
{
    /// <summary>
    ///   Some basic info about an object info like position, its class info,...
    /// </summary>
    /// <remarks>
    ///   Some basic info about an object info like position, its class info,...
    /// </remarks>
    /// <author>osmadja</author>
    [Serializable]
    public class ObjectInfoHeader
    {
        private int[] _attributeIds;

        /// <summary>
        ///   Can be position(for native object) or id(for non native object, positions are positive e ids are negative
        /// </summary>
        private long[] _attributesIdentification;

        private OID _classInfoId;
        private long _creationDate;
        private OID _nextObjectOID;
        private int _objectVersion;
        private OID _oid;
        private long _position;
        private OID _previousObjectOID;
        private long _updateDate;

        public ObjectInfoHeader(long position, OID previousObjectOID, OID nextObjectOID, OID classInfoId,
                                long[] attributesIdentification, int[] attributeIds)
        {
            _position = position;
            _oid = null;
            _previousObjectOID = previousObjectOID;
            _nextObjectOID = nextObjectOID;
            _classInfoId = classInfoId;
            _attributesIdentification = attributesIdentification;
            _attributeIds = attributeIds;
            _objectVersion = 1;
            _creationDate = OdbTime.GetCurrentTimeInTicks();
        }

        public ObjectInfoHeader()
        {
            _position = -1;
            _oid = null;
            _objectVersion = 1;
            _creationDate = OdbTime.GetCurrentTimeInTicks();
        }

        public virtual int GetNbAttributes()
        {
            return _attributesIdentification.Length;
        }

        public virtual OID GetNextObjectOID()
        {
            return _nextObjectOID;
        }

        public virtual void SetNextObjectOID(OID nextObjectOID)
        {
            _nextObjectOID = nextObjectOID;
        }

        public virtual long GetPosition()
        {
            return _position;
        }

        public virtual void SetPosition(long position)
        {
            _position = position;
        }

        //	/**
        //     * @return Returns the classInfoId.
        //     */
        //    public long getClassInfoId() {
        //        return classInfoId;
        //    }
        //    /**
        //     * @param classInfoId The classInfoId to set.
        //     */
        //    public void setClassInfoId(long classInfoId) {
        //        this.classInfoId = classInfoId;
        //    }
        public virtual OID GetPreviousObjectOID()
        {
            return _previousObjectOID;
        }

        public virtual void SetPreviousObjectOID(OID previousObjectOID)
        {
            _previousObjectOID = previousObjectOID;
        }

        public virtual OID GetClassInfoId()
        {
            return _classInfoId;
        }

        public override string ToString()
        {
            var buffer = new StringBuilder();
            buffer.Append("oid=").Append(_oid).Append(" - ");
            //.append("class info id=").append(classInfoId);
            buffer.Append(" - position=").Append(_position).Append(" | prev=").Append(_previousObjectOID);
            buffer.Append(" | next=").Append(_nextObjectOID);
            buffer.Append(" attrs =[");
            if (_attributesIdentification != null)
            {
                foreach (var value in _attributesIdentification)
                    buffer.Append(value).Append(" ");
            }
            else
            {
                buffer.Append(" nulls ");
            }

            buffer.Append(" ]");
            return buffer.ToString();
        }

        public virtual long[] GetAttributesIdentification()
        {
            return _attributesIdentification;
        }

        public virtual void SetAttributesIdentification(long[] attributesIdentification)
        {
            _attributesIdentification = attributesIdentification;
        }

        public virtual OID GetOid()
        {
            return _oid;
        }

        public virtual void SetOid(OID oid)
        {
            _oid = oid;
        }

        public virtual long GetCreationDate()
        {
            return _creationDate;
        }

        public virtual void SetCreationDate(long creationDate)
        {
            _creationDate = creationDate;
        }

        public virtual long GetUpdateDate()
        {
            return _updateDate;
        }

        public virtual void SetUpdateDate(long updateDate)
        {
            _updateDate = updateDate;
        }

        /// <summary>
        ///   Return the attribute identification (position or id) from the attribute id FIXME Remove dependency from StorageEngineConstant
        /// </summary>
        /// <param name="attributeId"> </param>
        /// <returns> -1 if attribute with this id does not exist </returns>
        public virtual long GetAttributeIdentificationFromId(int attributeId)
        {
            if (_attributeIds == null)
                return StorageEngineConstant.NullObjectIdId;

            for (var i = 0; i < _attributeIds.Length; i++)
            {
                if (_attributeIds[i] == attributeId)
                    return _attributesIdentification[i];
            }

            return StorageEngineConstant.NullObjectIdId;
        }

        public virtual long GetAttributeId(int attributeIndex)
        {
            return _attributeIds[attributeIndex];
        }

        public virtual void SetAttributesIds(int[] ids)
        {
            _attributeIds = ids;
        }

        public virtual int[] GetAttributeIds()
        {
            return _attributeIds;
        }

        public virtual void SetClassInfoId(OID classInfoId2)
        {
            _classInfoId = classInfoId2;
        }

        public virtual int GetObjectVersion()
        {
            return _objectVersion;
        }

        public virtual void SetObjectVersion(int objectVersion)
        {
            _objectVersion = objectVersion;
        }

        public override int GetHashCode()
        {
            var result = 1;
            result = 31 * result + (int) (_position ^ ((_position) >> (32 & 0x1f)));
            return result;
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;

            if (obj == null)
                return false;

            if (GetType() != obj.GetType())
                return false;

            var other = (ObjectInfoHeader) obj;
            
            if (_position != other._position)
                return false;
            return true;
        }

        public virtual void IncrementVersionAndUpdateDate()
        {
            _objectVersion++;
            _updateDate = OdbTime.GetCurrentTimeInTicks();
        }

        public virtual ObjectInfoHeader Duplicate()
        {
            var objectInfoHeader = new ObjectInfoHeader();

            objectInfoHeader.SetAttributesIdentification(_attributesIdentification);
            objectInfoHeader.SetAttributesIds(_attributeIds);
            objectInfoHeader.SetClassInfoId(_classInfoId);
            objectInfoHeader.SetCreationDate(_creationDate);
            objectInfoHeader.SetNextObjectOID(_nextObjectOID);
            objectInfoHeader.SetObjectVersion(_objectVersion);
            objectInfoHeader.SetOid(_oid);
            objectInfoHeader.SetPosition(_position);
            objectInfoHeader.SetPreviousObjectOID(_previousObjectOID);
            objectInfoHeader.SetUpdateDate(_updateDate);

            return objectInfoHeader;
        }
    }
}
