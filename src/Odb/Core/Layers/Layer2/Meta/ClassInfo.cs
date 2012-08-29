using System;
using System.Collections.Generic;
using System.Text;
using NDatabase.Tool.Wrappers;
using NDatabase.Tool.Wrappers.List;
using NDatabase.Tool.Wrappers.Map;

namespace NDatabase.Odb.Core.Layers.Layer2.Meta
{
    /// <summary>
    ///   A meta representation of a class
    /// </summary>
    [Serializable]
    public sealed class ClassInfo
    {
        /// <summary>
        ///   Constant used for the classCategory variable to indicate a system class
        /// </summary>
        public const byte CategorySystemClass = 1;

        /// <summary>
        ///   Constant used for the classCategory variable to indicate a user class
        /// </summary>
        public const byte CategoryUserClass = 2;

        /// <summary>
        ///   To keep session numbers, number of committed objects,first and last object position
        /// </summary>
        private readonly CommittedCIZoneInfo _committed;

        /// <summary>
        ///   To keep session original numbers, original number of committed objects,first and last object position
        /// </summary>
        private readonly CommittedCIZoneInfo _original;

        /// <summary>
        ///   To keep session uncommitted numbers, number of uncommitted objects,first and last object position
        /// </summary>
        private readonly CIZoneInfo _uncommitted;

        private IOdbList<ClassAttributeInfo> _attributes;

        private readonly AttributesCache _attributesCache;

        /// <summary>
        ///   Where starts the block of attributes definition of this class ?
        /// </summary>
        private long _attributesDefinitionPosition;

        /// <summary>
        ///   The size (in bytes) of the class block
        /// </summary>
        private int _blockSize;

        /// <summary>
        ///   To specify the type of the class : system class or user class
        /// </summary>
        private byte _classCategory;

        /// <summary>
        ///   The full class name with package
        /// </summary>
        private string _fullClassName;

        private IOdbList<ClassInfoIndex> _indexes;

        /// <summary>
        ///   Infos about the last object of this class
        /// </summary>
        private ObjectInfoHeader _lastObjectInfoHeader;

        /// <summary>
        ///   The max id is used to give a unique id for each attribute and allow refactoring like new field and/or removal
        /// </summary>
        private int _maxAttributeId;

        /// <summary>
        ///   Physical location of this class in the file (in byte)
        /// </summary>
        private long _position;

        private readonly OidInfo _oidInfo;
        private readonly Dictionary<string, Type> _typeCache = new Dictionary<string, Type>();

        public ClassInfo()
        {
            _original = new CommittedCIZoneInfo(this, null, null, 0);
            _committed = new CommittedCIZoneInfo(this, null, null, 0);
            _uncommitted = new CIZoneInfo(this, null, null, 0);
            _oidInfo = new OidInfo();
            _blockSize = -1;
            _position = -1;
            _maxAttributeId = -1;
            _classCategory = CategoryUserClass;
            _attributesCache = new AttributesCache();
        }

        public ClassInfo(string className) : this(className, null)
        {
        }

        private ClassInfo(string fullClassName, IOdbList<ClassAttributeInfo> attributes) : this()
        {
            CheckIfTypeIsInstantiable(fullClassName);

            _fullClassName = fullClassName;
            _attributes = attributes;

            if (attributes != null)
                FillAttributesMap();

            _maxAttributeId = (attributes == null
                                  ? 1
                                  : attributes.Count + 1);
        }

        private void CheckIfTypeIsInstantiable(string fullClassName)
        {
            Type type;
            var success = _typeCache.TryGetValue(fullClassName, out type);

            if (success)
                return;

            type = Type.GetType(fullClassName);

            if (type == null)
            {
                throw new ArgumentException(
                    string.Format("Given full class name is not enough to create the Type from that: {0}", fullClassName));
            }

            _typeCache.Add(fullClassName, type);
        }

        private void FillAttributesMap()
        {
            if (_attributesCache.AttributesByName == null)
            {
                _attributesCache.AttributesByName = new OdbHashMap<string, ClassAttributeInfo>();
                _attributesCache.AttributesById = new OdbHashMap<int, ClassAttributeInfo>();
            }
            // attributesMap.clear();
            foreach (var classAttributeInfo in _attributes)
            {
                _attributesCache.AttributesByName[classAttributeInfo.GetName()] = classAttributeInfo;
                _attributesCache.AttributesById[classAttributeInfo.GetId()] = classAttributeInfo;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != typeof (ClassInfo))
                return false;
            var classInfo = (ClassInfo) obj;
            return classInfo._fullClassName.Equals(_fullClassName);
        }

        public override string ToString()
        {
            var buffer = new StringBuilder();

            buffer.Append(" [ ").Append(_fullClassName).Append(" - id=").Append(_oidInfo.ID);
            buffer.Append(" - previousClass=").Append(_oidInfo.PreviousClassOID).Append(" - nextClass=").Append(_oidInfo.NextClassOID).
                Append(" - attributes=(");

            // buffer.append(" | position=").append(position);
            // buffer.append(" | class=").append(className).append(" | attributes=[");

            if (_attributes != null)
            {
                foreach (var classAttributeInfo in _attributes)
                    buffer.Append(classAttributeInfo.GetName()).Append(",");
            }
            else
            {
                buffer.Append("not yet defined");
            }

            buffer.Append(") ]");

            return buffer.ToString();
        }

        public IOdbList<ClassAttributeInfo> GetAttributes()
        {
            return _attributes;
        }

        public void SetAttributes(IOdbList<ClassAttributeInfo> attributes)
        {
            _attributes = attributes;
            _maxAttributeId = attributes.Count;
            FillAttributesMap();
        }

        public CommittedCIZoneInfo GetCommitedZoneInfo()
        {
            return _committed;
        }

        public long GetAttributesDefinitionPosition()
        {
            return _attributesDefinitionPosition;
        }

        public void SetAttributesDefinitionPosition(long definitionPosition)
        {
            _attributesDefinitionPosition = definitionPosition;
        }

        public OID GetNextClassOID()
        {
            return _oidInfo.NextClassOID;
        }

        public void SetNextClassOID(OID nextClassOID)
        {
            _oidInfo.NextClassOID = nextClassOID;
        }

        public OID GetPreviousClassOID()
        {
            return _oidInfo.PreviousClassOID;
        }

        public void SetPreviousClassOID(OID previousClassOID)
        {
            _oidInfo.PreviousClassOID = previousClassOID;
        }

        public long GetPosition()
        {
            return _position;
        }

        public void SetPosition(long position)
        {
            _position = position;
        }

        public int GetBlockSize()
        {
            return _blockSize;
        }

        public void SetBlockSize(int blockSize)
        {
            _blockSize = blockSize;
        }

        public string GetFullClassName()
        {
            return _fullClassName;
        }

        /// <summary>
        ///   This method could be optimized, but it is only on Class creation, one time in the database life time...
        /// </summary>
        /// <remarks>
        ///   This method could be optimized, but it is only on Class creation, one time in the database life time... This is used to get all (non native) attributes a class info have to store them in the meta model before storing the class itself
        /// </remarks>
        /// <returns> </returns>
        public IOdbList<ClassAttributeInfo> GetAllNonNativeAttributes()
        {
            IOdbList<ClassAttributeInfo> result = new OdbArrayList<ClassAttributeInfo>(_attributes.Count);
            
            foreach (var classAttributeInfo in _attributes)
            {
                if (!classAttributeInfo.IsNative() || classAttributeInfo.GetAttributeType().IsEnum())
                {
                    result.Add(classAttributeInfo);
                }
                else
                {
                    if (classAttributeInfo.GetAttributeType().IsArray() && !classAttributeInfo.GetAttributeType().GetSubType().IsNative())
                        result.Add(new ClassAttributeInfo(-1, "subtype", classAttributeInfo.GetAttributeType().GetSubType().GetName(),
                                                          null));
                }
            }

            return result;
        }

        public OID GetId()
        {
            return _oidInfo.ID;
        }

        public void SetId(OID id)
        {
            _oidInfo.ID = id;
        }

        public ClassAttributeInfo GetAttributeInfoFromId(int id)
        {
            return _attributesCache.AttributesById[id];
        }

        public int GetAttributeId(string name)
        {
            var classAttributeInfo = _attributesCache.AttributesByName[name];

            if (classAttributeInfo != null)
                return classAttributeInfo.GetId();

            var enrichedName = EnrichNameForAutoProperty(name);

            classAttributeInfo = _attributesCache.AttributesByName[enrichedName];

            return classAttributeInfo != null
                       ? classAttributeInfo.GetId()
                       : -1;
        }

        private static string EnrichNameForAutoProperty(string name)
        {
            return string.Format("<{0}>k__BackingField", name);
        }

        public ClassAttributeInfo GetAttributeInfoFromName(string name)
        {
            return _attributesCache.AttributesByName[name];
        }

        public ClassAttributeInfo GetAttributeInfo(int index)
        {
            return _attributes[index];
        }

        public int GetMaxAttributeId()
        {
            return _maxAttributeId;
        }

        public void SetMaxAttributeId(int maxAttributeId)
        {
            _maxAttributeId = maxAttributeId;
        }

        public ClassInfoCompareResult ExtractDifferences(ClassInfo newCI, bool update)
        {
            string attributeName;
            ClassAttributeInfo cai1;
            ClassAttributeInfo cai2;

            var result = new ClassInfoCompareResult(GetFullClassName());
            IOdbList<ClassAttributeInfo> attributesToRemove = new OdbArrayList<ClassAttributeInfo>(10);
            IOdbList<ClassAttributeInfo> attributesToAdd = new OdbArrayList<ClassAttributeInfo>(10);

            var attributesCount = _attributes.Count;
            for (var id = 0; id < attributesCount; id++)
            {
                // !!!WARNING : ID start with 1 and not 0
                cai1 = _attributes[id];
                if (cai1 == null)
                    continue;
                attributeName = cai1.GetName();
                cai2 = newCI.GetAttributeInfoFromId(cai1.GetId());
                if (cai2 == null)
                {
                    result.AddCompatibleChange(string.Format("Field '{0}' has been removed", attributeName));
                    if (update)
                    {
                        // Simply remove the attribute from meta-model
                        attributesToRemove.Add(cai1);
                    }
                }
                else
                {
                    if (!OdbType.TypesAreCompatible(cai1.GetAttributeType(), cai2.GetAttributeType()))
                    {
                        result.AddIncompatibleChange(
                            string.Format("Type of Field '{0}' has changed : old='{1}' - new='{2}'", attributeName,
                                          cai1.GetFullClassname(), cai2.GetFullClassname()));
                    }
                }
            }

            var nbNewAttributes = newCI._attributes.Count;
            for (var id = 0; id < nbNewAttributes; id++)
            {
                // !!!WARNING : ID start with 1 and not 0
                cai2 = newCI._attributes[id];
                if (cai2 == null)
                    continue;
                attributeName = cai2.GetName();
                cai1 = GetAttributeInfoFromId(cai2.GetId());
                if (cai1 == null)
                {
                    result.AddCompatibleChange("Field '" + attributeName + "' has been added");
                    if (update)
                    {
                        // Sets the right id of attribute
                        cai2.SetId(_maxAttributeId + 1);
                        _maxAttributeId++;
                        // Then adds the new attribute to the meta-model
                        attributesToAdd.Add(cai2);
                    }
                }
            }
            _attributes.RemoveAll(attributesToRemove);
            _attributes.AddAll(attributesToAdd);
            FillAttributesMap();
            return result;
        }

        public int GetNumberOfAttributes()
        {
            return _attributes.Count;
        }

        public ClassInfoIndex AddIndexOn(string name, string[] indexFields, bool acceptMultipleValuesForSameKey)
        {
            if (_indexes == null)
                _indexes = new OdbArrayList<ClassInfoIndex>();

            var cii = new ClassInfoIndex
                {
                    ClassInfoId = _oidInfo.ID,
                    CreationDate = OdbTime.GetCurrentTimeInTicks(),
                    Name = name,
                    Status = ClassInfoIndex.Enabled,
                    IsUnique = !acceptMultipleValuesForSameKey
                };

            cii.LastRebuild = cii.CreationDate;
            var attributeIds = new int[indexFields.Length];

            for (var i = 0; i < indexFields.Length; i++)
                attributeIds[i] = GetAttributeId(indexFields[i]);

            cii.AttributeIds = attributeIds;
            _indexes.Add(cii);
            return cii;
        }

        /// <summary>
        ///   Removes an index
        /// </summary>
        /// <param name="cii"> </param>
        public void RemoveIndex(ClassInfoIndex cii)
        {
            _indexes.Remove(cii);
        }

        public int GetNumberOfIndexes()
        {
            if (_indexes == null)
                return 0;
            return _indexes.Count;
        }

        public ClassInfoIndex GetIndex(int index)
        {
            if (_indexes == null || index >= _indexes.Count)
                throw new OdbRuntimeException(
                    NDatabaseError.IndexNotFound.AddParameter(GetFullClassName()).AddParameter(index));
            return _indexes[index];
        }

        public void SetIndexes(IOdbList<ClassInfoIndex> indexes2)
        {
            _indexes = indexes2;
        }

        /// <summary>
        ///   To detect if a class has cyclic reference
        /// </summary>
        /// <returns> true if this class info has cyclic references </returns>
        public bool HasCyclicReference()
        {
            return HasCyclicReference(new OdbHashMap<string, ClassInfo>());
        }

        /// <summary>
        ///   To detect if a class has cyclic reference
        /// </summary>
        /// <param name="alreadyVisitedClasses"> A hashmap containg all the already visited classes </param>
        /// <returns> true if this class info has cyclic references </returns>
        private bool HasCyclicReference(IDictionary<string, ClassInfo> alreadyVisitedClasses)
        {
            if (alreadyVisitedClasses[_fullClassName] != null)
                return true;

            alreadyVisitedClasses.Add(_fullClassName, this);
            
            for (var i = 0; i < _attributes.Count; i++)
            {
                var classAttributeInfo = GetAttributeInfo(i);
                if (!classAttributeInfo.IsNative())
                {
                    IDictionary<string, ClassInfo> localMap = new OdbHashMap<string, ClassInfo>(alreadyVisitedClasses);
                    var hasCyclicRef = classAttributeInfo.GetClassInfo().HasCyclicReference(localMap);
                    if (hasCyclicRef)
                        return true;
                }
            }
            return false;
        }

        public byte GetClassCategory()
        {
            return _classCategory;
        }

        public void SetClassCategory(byte classInfoType)
        {
            _classCategory = classInfoType;
        }

        public ObjectInfoHeader GetLastObjectInfoHeader()
        {
            return _lastObjectInfoHeader;
        }

        public void SetLastObjectInfoHeader(ObjectInfoHeader lastObjectInfoHeader)
        {
            _lastObjectInfoHeader = lastObjectInfoHeader;
        }

        public CIZoneInfo GetUncommittedZoneInfo()
        {
            return _uncommitted;
        }

        /// <summary>
        ///   Get number of objects: committed and uncommitted
        /// </summary>
        /// <returns> The number of committed and uncommitted objects </returns>
        public long GetNumberOfObjects()
        {
            return _committed.GetNbObjects() + _uncommitted.GetNbObjects();
        }

        public CommittedCIZoneInfo GetOriginalZoneInfo()
        {
            return _original;
        }

        public bool IsSystemClass()
        {
            return _classCategory == CategorySystemClass;
        }

        public ClassInfoIndex GetIndexWithName(string name)
        {
            if (_indexes == null)
                return null;
            
            for (var i = 0; i < _indexes.Count; i++)
            {
                var classInfoIndex = _indexes[i];
                if (classInfoIndex.Name.Equals(name))
                    return classInfoIndex;
            }
            return null;
        }

        public ClassInfoIndex GetIndexForAttributeId(int attributeId)
        {
            if (_indexes == null)
                return null;
            for (var i = 0; i < _indexes.Count; i++)
            {
                var classInfoIndex = _indexes[i];
                if (classInfoIndex.AttributeIds.Length == 1 && classInfoIndex.AttributeIds[0] == attributeId)
                    return classInfoIndex;
            }
            return null;
        }

        public ClassInfoIndex GetIndexForAttributeIds(int[] attributeIds)
        {
            if (_indexes == null)
                return null;
            for (var i = 0; i < _indexes.Count; i++)
            {
                var classInfoIndex = _indexes[i];
                if (classInfoIndex.MatchAttributeIds(attributeIds))
                    return classInfoIndex;
            }
            return null;
        }

        public string[] GetAttributeNames(int[] attributeIds)
        {
            var attributeIdsLength = attributeIds.Length;
            var names = new string[attributeIdsLength];
            
            for (var i = 0; i < attributeIdsLength; i++)
                names[i] = GetAttributeInfoFromId(attributeIds[i]).GetName();

            return names;
        }

        public IList<string> GetAttributeNamesAsList(int[] attributeIds)
        {
            var nbIds = attributeIds.Length;

            IList<string> names = new List<string>(attributeIds.Length);
            for (var i = 0; i < nbIds; i++)
                names.Add(GetAttributeInfoFromId(attributeIds[i]).GetName());

            return names;
        }

        public IOdbList<ClassInfoIndex> GetIndexes()
        {
            if (_indexes == null)
                return new OdbArrayList<ClassInfoIndex>();
            return _indexes;
        }

        public void RemoveAttribute(ClassAttributeInfo cai)
        {
            _attributes.Remove(cai);
            _attributesCache.AttributesByName.Remove(cai.GetName());
        }

        public void AddAttribute(ClassAttributeInfo cai)
        {
            cai.SetId(_maxAttributeId++);
            _attributes.Add(cai);
            _attributesCache.AttributesByName.Add(cai.GetName(), cai);
        }

        public void SetFullClassName(string fullClassName)
        {
            _fullClassName = fullClassName;
        }

        public bool HasIndex(string indexName)
        {
            if (_indexes == null)
                return false;
            
            for (var i = 0; i < _indexes.Count; i++)
            {
                var classInfoIndex = _indexes[i];
                if (indexName.Equals(classInfoIndex.Name))
                    return true;
            }

            return false;
        }

        public bool HasIndex()
        {
            return _indexes != null && !_indexes.IsEmpty();
        }

        public ClassInfo Duplicate(bool onlyData)
        {
            var ci = new ClassInfo(_fullClassName);

            ci.SetAttributes(_attributes);
            ci.SetClassCategory(_classCategory);
            ci.SetMaxAttributeId(_maxAttributeId);
            
            if (onlyData)
                return ci;

            ci.SetAttributesDefinitionPosition(_attributesDefinitionPosition);
            ci.SetBlockSize(_blockSize);
            ci.SetId(_oidInfo.ID);
            ci.SetPreviousClassOID(_oidInfo.PreviousClassOID);
            ci.SetNextClassOID(_oidInfo.NextClassOID);
            ci.SetLastObjectInfoHeader(_lastObjectInfoHeader);
            ci.SetPosition(_position);
            ci.SetIndexes(_indexes);

            return ci;
        }
    }
}
