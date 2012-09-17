using System;
using System.Collections.Generic;
using NDatabase.Odb;
using NDatabase.Odb.Core.Layers.Layer2.Instance;
using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Odb.Core.Layers.Layer3;
using NDatabase.Odb.Core.Layers.Layer3.Oid;
using NDatabase.Odb.Core.Query;
using NDatabase.Odb.Core.Query.Execution;
using NDatabase.Tool.Wrappers.List;

namespace Test.NDatabase.Odb.Test.IO
{
    public class MockObjectReader : IObjectReader
    {
        public MockObjectReader(IStorageEngine engine)
        {
        }

        #region IObjectReader Members

        public void ReadDatabaseHeader()
        {
            //empty
        }

        public void LoadMetaModel(MetaModel metaModel, bool full)
        {
            //empty
        }

        public Object BuildOneInstance(NonNativeObjectInfo objectInfo)
        {
            return null;
        }

        public void Close()
        {
        }

        public IList<FullIDInfo> GetAllIdInfos(String objectTypeToDisplay, byte idType, bool displayObject)
        {
            return null;
        }

        public IList<long> GetAllIds(byte idType)
        {
            return null;
        }

        public String GetBaseIdentification()
        {
            return null;
        }

        public OID GetIdOfObjectAt(long position, bool includeDeleted)
        {
            return null;
        }

        public IInstanceBuilder GetInstanceBuilder()
        {
            return null;
        }

        public OID GetNextObjectOID(OID oid)
        {
            return null;
        }

        public Object GetObjectFromOid(OID oid, bool returnInstance, bool useCache)
        {
            return null;
        }

        public IObjects<T> GetObjectInfos<T>(IQuery query, bool inMemory, int startIndex, int endIndex,
                                             bool returnObjects, IMatchingObjectAction queryResultAction)
        {
            return null;
        }

        public long GetObjectPositionFromItsOid(OID oid, bool useCache, bool throwException)
        {
            return 0;
        }

        public IObjects<T> GetObjects<T>(IQuery query, bool inMemory, int startIndex, int endIndex)
        {
            return null;
        }

        public IValues GetValues(IValuesQuery query, int startIndex, int endIndex)
        {
            return null;
        }

        public AtomicNativeObjectInfo ReadAtomicNativeObjectInfo(long position, int odbTypeId)
        {
            return null;
        }

        public AttributeValuesMap ReadObjectInfoValuesFromOID(ClassInfo classInfo, OID oid, bool useCache,
                                                              IOdbList<string> attributeNames,
                                                              IOdbList<string> relationAttributeNames,
                                                              int recursionLevel, string[] orderByFields)
        {
            return null;
        }

        public Object ReadAtomicNativeObjectInfoAsObject(long position, int odbTypeId)
        {
            return null;
        }

        public IOdbList<ClassInfoIndex> ReadClassInfoIndexesAt(long position, ClassInfo classInfo)
        {
            return null;
        }

        public NonNativeObjectInfo ReadNonNativeObjectInfoFromOid(ClassInfo classInfo, OID oid, bool useCache,
                                                                  bool returnObjects)
        {
            return null;
        }

        public NonNativeObjectInfo ReadNonNativeObjectInfoFromPosition(ClassInfo classInfo, OID oid, long position,
                                                                       bool useCache, bool returnInstance)
        {
            return null;
        }

        public ObjectInfoHeader ReadObjectInfoHeaderFromOid(OID oid, bool useCache)
        {
            return null;
        }

        public long ReadOidPosition(OID oid)
        {
            return 0;
        }

        #endregion

        public void ReadDatabaseHeader(String user, String password)
        {
        }

        public AttributeValuesMap ReadObjectInfoValuesFromOID(ClassInfo classInfo, OID oid, bool useCache,
                                                              IOdbList<String> attributeNames,
                                                              IOdbList<String> relationAttributeNames,
                                                              int recursionLevel, String[] orderByFields,
                                                              bool useOidForObject)
        {
            return null;
        }
    }
}
