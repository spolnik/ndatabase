using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using NDatabase.Odb.Impl.Core.Layers.Layer3.Engine;
using NDatabase.Tool.Wrappers;

namespace NDatabase.Odb.Core.Layers.Layer2.Meta
{
    /// <summary>
    ///   To keep info about a non native object <pre>- Keeps its class info : a meta information to describe its type
    ///                                            - All its attributes values
    ///                                            - Its Pointers : its position, the previous object OID, the next object OID
    ///                                            - The Object being represented by The meta information</pre>
    /// </summary>
    /// <author>olivier s</author>
    [Serializable]
    public class NonNativeObjectInfo : AbstractObjectInfo
    {
        private readonly int _maxNbattributes;
        private AbstractObjectInfo[] _attributeValues;
        private ClassInfo _classInfo;
        private ObjectInfoHeader _objectHeader;

        /// <summary>
        ///   The object being represented
        /// </summary>
        [NonSerialized]
        private object _theObject;

        public NonNativeObjectInfo() : base(null)
        {
        }

        public NonNativeObjectInfo(ObjectInfoHeader oip, ClassInfo classInfo) : base(null)
        {
            // private List attributeValues;
            _classInfo = classInfo;
            _objectHeader = oip;

            if (classInfo != null)
            {
                _maxNbattributes = classInfo.GetMaxAttributeId();
                _attributeValues = new AbstractObjectInfo[_maxNbattributes];
            }
        }

        public NonNativeObjectInfo(ClassInfo classInfo) : base(null)
        {
            //new OdbArrayList<NonNativeObjectInfo>();
            _classInfo = classInfo;
            _objectHeader = new ObjectInfoHeader(-1, null, null, (classInfo != null
                                                                      ? classInfo.GetId()
                                                                      : null), null, null);
            if (classInfo != null)
            {
                _maxNbattributes = classInfo.GetMaxAttributeId();
                _attributeValues = new AbstractObjectInfo[_maxNbattributes];
            }
        }

        public NonNativeObjectInfo(object @object, ClassInfo info, AbstractObjectInfo[] values,
                                   long[] attributesIdentification, int[] attributeIds)
            : base(Meta.OdbType.GetFromName(info.GetFullClassName()))
        {
            //new OdbArrayList<NonNativeObjectInfo>();
            _theObject = @object;
            _classInfo = info;
            _attributeValues = values;
            _maxNbattributes = _classInfo.GetMaxAttributeId();

            if (_attributeValues == null)
                _attributeValues = new AbstractObjectInfo[_maxNbattributes];

            _objectHeader = new ObjectInfoHeader(-1, null, null, (_classInfo != null
                                                                      ? _classInfo.GetId()
                                                                      : null), attributesIdentification, attributeIds);
        }

        public virtual ObjectInfoHeader GetHeader()
        {
            return _objectHeader;
        }

        public virtual AbstractObjectInfo GetAttributeValueFromId(int attributeId)
        {
            return _attributeValues[attributeId - 1];
        }

        public virtual ClassInfo GetClassInfo()
        {
            return _classInfo;
        }

        public virtual void SetClassInfo(ClassInfo classInfo)
        {
            if (classInfo != null)
            {
                _classInfo = classInfo;
                _objectHeader.SetClassInfoId(classInfo.GetId());
            }
        }

        public override string ToString()
        {
            var buffer =
                new StringBuilder(_classInfo.GetFullClassName()).Append("(").Append(GetOid()).Append(")=");

            if (_attributeValues == null)
            {
                buffer.Append("null attribute values");
                return buffer.ToString();
            }

            for (var i = 0; i < _attributeValues.Length; i++)
            {
                if (i != 0)
                    buffer.Append(",");

                var attributeName = (_classInfo != null
                                         ? (_classInfo.GetAttributeInfo(i)).GetName()
                                         : "?");

                buffer.Append(attributeName).Append("=");
                object @object = _attributeValues[i];

                if (@object == null)
                {
                    buffer.Append(" null object - should not happen , ");
                }
                else
                {
                    var type = Meta.OdbType.GetFromClass(_attributeValues[i].GetType());
                    if (@object is NonNativeNullObjectInfo)
                    {
                        buffer.Append("null");
                        continue;
                    }
                    if (@object is NonNativeDeletedObjectInfo)
                    {
                        buffer.Append("deleted object");
                        continue;
                    }
                    if (@object is NativeObjectInfo)
                    {
                        var noi = (NativeObjectInfo) @object;
                        buffer.Append(noi.ToString());
                        continue;
                    }
                    var nnoi = @object as NonNativeObjectInfo;
                    if (nnoi != null)
                    {
                        buffer.Append("@").Append(nnoi.GetClassInfo().GetFullClassName()).Append("(id=").Append(
                            nnoi.GetOid()).Append(")");
                        continue;
                    }
                    if (@object is ObjectReference)
                    {
                        buffer.Append(@object.ToString());
                        continue;
                    }
                    buffer.Append("@").Append(OdbClassUtil.GetClassName(type.GetName()));
                }
            }

            return buffer.ToString();
        }

        public virtual OID GetNextObjectOID()
        {
            return _objectHeader.GetNextObjectOID();
        }

        public virtual void SetNextObjectOID(OID nextObjectOID)
        {
            _objectHeader.SetNextObjectOID(nextObjectOID);
        }

        public virtual OID GetPreviousObjectOID()
        {
            return _objectHeader.GetPreviousObjectOID();
        }

        public virtual void SetPreviousInstanceOID(OID previousObjectOID)
        {
            _objectHeader.SetPreviousObjectOID(previousObjectOID);
        }

        public override long GetPosition()
        {
            return _objectHeader.GetPosition();
        }

        public override void SetPosition(long position)
        {
            _objectHeader.SetPosition(position);
        }

        public override object GetObject()
        {
            return _theObject;
        }

        public virtual object GetValueOf(string attributeName)
        {
            Debug.Assert(attributeName != null);

            int attributeId;
            var isRelation = attributeName.IndexOf(".", System.StringComparison.Ordinal) != -1;

            if (!isRelation)
            {
                attributeId = GetClassInfo().GetAttributeId(attributeName);
                return GetAttributeValueFromId(attributeId).GetObject();
            }

            var firstDotIndex = attributeName.IndexOf(".", System.StringComparison.Ordinal);
            var firstAttributeName = attributeName.Substring(0, firstDotIndex);
            attributeId = GetClassInfo().GetAttributeId(firstAttributeName);

            object @object = _attributeValues[attributeId];
            var nnoi = @object as NonNativeObjectInfo;

            if (nnoi != null)
            {
                int beginIndex = firstDotIndex + 1;
                return nnoi.GetValueOf(attributeName.Substring(beginIndex, attributeName.Length - beginIndex));
            }

            throw new OdbRuntimeException(
                NDatabaseError.ClassInfoDoNotHaveTheAttribute.AddParameter(GetClassInfo().GetFullClassName()).
                    AddParameter(attributeName));
        }

        /// <summary>
        ///   Used to change the value of an attribute
        /// </summary>
        /// <param name="attributeName"> </param>
        /// <param name="aoi"> </param>
        public virtual void SetValueOf(string attributeName, AbstractObjectInfo aoi)
        {
            int attributeId;
            var isRelation = attributeName.IndexOf(".", StringComparison.Ordinal) != -1;

            if (!isRelation)
            {
                attributeId = GetClassInfo().GetAttributeId(attributeName);
                SetAttributeValue(attributeId, aoi);
                return;
            }

            var firstDotIndex = attributeName.IndexOf(".", StringComparison.Ordinal);

            var firstAttributeName = attributeName.Substring(0, firstDotIndex);
            attributeId = GetClassInfo().GetAttributeId(firstAttributeName);
            object @object = _attributeValues[attributeId];
            var nnoi = @object as NonNativeObjectInfo;
            
            if (nnoi != null)
            {
                int beginIndex = firstDotIndex + 1;
                nnoi.SetValueOf(attributeName.Substring(beginIndex, attributeName.Length - beginIndex), aoi);
            }

            throw new OdbRuntimeException(
                NDatabaseError.ClassInfoDoNotHaveTheAttribute.AddParameter(GetClassInfo().GetFullClassName()).
                    AddParameter(attributeName));
        }

        public virtual OID GetOid()
        {
            if (GetHeader() == null)
                throw new OdbRuntimeException(NDatabaseError.UnexpectedSituation.AddParameter("Null Object Info Header"));
            return GetHeader().GetOid();
        }

        public virtual void SetOid(OID oid)
        {
            if (GetHeader() != null)
                GetHeader().SetOid(oid);
        }

        public override bool IsNonNativeObject()
        {
            return true;
        }

        public override bool IsNull()
        {
            return false;
        }

        public virtual void Clear()
        {
            _attributeValues = null;
        }

        /// <summary>
        ///   Create a copy oh this meta object
        /// </summary>
        /// <param name="onlyData"> if true, only copy attributes values </param>
        /// <returns> </returns>
        public override AbstractObjectInfo CreateCopy(IDictionary<OID, AbstractObjectInfo> cache, bool onlyData)
        {
            var nnoi = (NonNativeObjectInfo) cache[_objectHeader.GetOid()];
            if (nnoi != null)
                return nnoi;

            if (onlyData)
            {
                var oih = new ObjectInfoHeader();
                nnoi = new NonNativeObjectInfo(_theObject, _classInfo, null, oih.GetAttributesIdentification(),
                                               oih.GetAttributeIds());
            }
            else
            {
                nnoi = new NonNativeObjectInfo(_theObject, _classInfo, null, _objectHeader.GetAttributesIdentification(),
                                               _objectHeader.GetAttributeIds());

                nnoi.GetHeader().SetOid(GetHeader().GetOid());
            }

            var newAttributeValues = new AbstractObjectInfo[_attributeValues.Length];

            for (var i = 0; i < _attributeValues.Length; i++)
                newAttributeValues[i] = _attributeValues[i].CreateCopy(cache, onlyData);

            nnoi._attributeValues = newAttributeValues;
            cache.Add(_objectHeader.GetOid(), nnoi);

            return nnoi;
        }

        public virtual void SetAttributeValue(int attributeId, AbstractObjectInfo aoi)
        {
            _attributeValues[attributeId - 1] = aoi;
        }

        public virtual AbstractObjectInfo[] GetAttributeValues()
        {
            return _attributeValues;
        }

        public virtual int GetMaxNbattributes()
        {
            return _maxNbattributes;
        }

        /// <summary>
        ///   The performance of this method is bad.
        /// </summary>
        /// <remarks>
        ///   The performance of this method is bad. But it is not used by the engine, only in the ODBExplorer
        /// </remarks>
        /// <param name="aoi"> </param>
        /// <returns> </returns>
        public virtual int GetAttributeId(AbstractObjectInfo aoi)
        {
            for (var i = 0; i < _attributeValues.Length; i++)
            {
                if (aoi == _attributeValues[i])
                    return i + 1;
            }
            return -1;
        }

        /// <summary>
        ///   Return the position where the position of an attribute is stored.
        /// </summary>
        /// <remarks>
        ///   Return the position where the position of an attribute is stored. <pre>If a object has 3 attributes and if it is stored at position x
        ///                                                                       Then the number of attributes (3) is stored at x+StorageEngineConstant.OBJECT_OFFSET_NB_ATTRIBUTES
        ///                                                                       and first attribute id definition is stored at x+StorageEngineConstant.OBJECT_OFFSET_NB_ATTRIBUTES+size-of(int)
        ///                                                                       and first attribute position is stored at x+StorageEngineConstant.OBJECT_OFFSET_NB_ATTRIBUTES+size-of(int)+size-of(int)
        ///                                                                       the second attribute id is stored at x+StorageEngineConstant.OBJECT_OFFSET_NB_ATTRIBUTES+size-of(int)+size-of(int)+size-of(long)
        ///                                                                       the second attribute position is stored at x+StorageEngineConstant.OBJECT_OFFSET_NB_ATTRIBUTES+size-of(int)+size-of(int)+size-of(long)+size-of(int)
        ///                                                                       <pre>FIXME Remove dependency of StorageEngineConstant!
        /// </remarks>
        /// <param name="attributeId"> </param>
        /// <returns> The position where this attribute is stored </returns>
        public virtual long GetAttributeDefinitionPosition(int attributeId)
        {
            var offset = StorageEngineConstant.ObjectOffsetNbAttributes;
            // delta =
            // Skip NbAttribute (int) +
            // Delta attribute (attributeId-1) * attribute definition size =
            // INT+LONG
            // Skip attribute Id (int)
            long delta = Meta.OdbType.Integer.GetSize() +
                         (attributeId - 1) * (Meta.OdbType.Integer.GetSize() + Meta.OdbType.Long.GetSize()) +
                         Meta.OdbType.Integer.GetSize();

            return GetPosition() + offset + delta;
        }

        public override void SetObject(object @object)
        {
            _theObject = @object;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            // This happens when the object is deleted
            if (_objectHeader == null)
                return -1;

            return _objectHeader.GetHashCode();
        }

        /// <param name="header"> </param>
        public virtual void SetHeader(ObjectInfoHeader header)
        {
            _objectHeader = header;
        }
    }
}
