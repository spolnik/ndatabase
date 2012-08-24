using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Odb.Core.Oid;
using NDatabase.Odb.Impl.Core.Layers.Layer3.Engine;
using NDatabase.Odb.Impl.Core.Transaction;
using NDatabase.Tool.Wrappers;
using NUnit.Framework;

namespace Test.Odb.Test.Cache
{
    [TestFixture]
    public class TestInsertingObject : ODBTest
    {
        /// <exception cref="System.IO.IOException"></exception>
        [Test]
        public virtual void Test1()
        {
            var cache = CacheFactory.GetLocalCache("test");
            var s1 = "ola1";
            var s2 = "ola2";
            var s3 = "ola3";
            cache.StartInsertingObjectWithOid(s1, OIDFactory.BuildObjectOID(1), null);
            cache.StartInsertingObjectWithOid(s2, OIDFactory.BuildObjectOID(2), null);
            cache.StartInsertingObjectWithOid(s3, OIDFactory.BuildObjectOID(3), null);
            AssertTrue(cache.IdOfInsertingObject(s1) != StorageEngineConstant.NullObjectId);
            AssertTrue(cache.IdOfInsertingObject(s2) != StorageEngineConstant.NullObjectId);
            AssertTrue(cache.IdOfInsertingObject(s3) != StorageEngineConstant.NullObjectId);
            cache.EndInsertingObject(s3);
            cache.EndInsertingObject(s2);
            cache.EndInsertingObject(s1);
            AssertTrue(cache.IdOfInsertingObject(s1) == StorageEngineConstant.NullObjectId);
            AssertTrue(cache.IdOfInsertingObject(s2) == StorageEngineConstant.NullObjectId);
            AssertTrue(cache.IdOfInsertingObject(s3) == StorageEngineConstant.NullObjectId);
        }

        /// <exception cref="System.IO.IOException"></exception>
        [Test]
        public virtual void Test2()
        {
            var cache = CacheFactory.GetLocalCache("temp");
            var s1 = "ola1";
            var s2 = "ola2";
            var s3 = "ola3";
            for (var i = 0; i < 1000 * 3; i += 3)
            {
                cache.StartInsertingObjectWithOid(s1, OIDFactory.BuildObjectOID(i + 1), null);
                cache.StartInsertingObjectWithOid(s2, OIDFactory.BuildObjectOID(i + 2), null);
                cache.StartInsertingObjectWithOid(s3, OIDFactory.BuildObjectOID(i + 3), null);
            }
            AssertEquals(1000, cache.InsertingLevelOf(s1));
            AssertEquals(1000, cache.InsertingLevelOf(s2));
            AssertEquals(1000, cache.InsertingLevelOf(s3));
            for (var i = 0; i < 1000; i++)
            {
                cache.EndInsertingObject(s1);
                cache.EndInsertingObject(s2);
                cache.EndInsertingObject(s3);
            }
            AssertEquals(0, cache.InsertingLevelOf(s1));
            AssertEquals(0, cache.InsertingLevelOf(s2));
            AssertEquals(0, cache.InsertingLevelOf(s3));
            cache.StartInsertingObjectWithOid(s1, OIDFactory.BuildObjectOID(1), null);
            cache.StartInsertingObjectWithOid(s1, OIDFactory.BuildObjectOID(1), null);
            cache.StartInsertingObjectWithOid(s1, OIDFactory.BuildObjectOID(1), null);
            cache.StartInsertingObjectWithOid(s2, OIDFactory.BuildObjectOID(2), null);
            cache.StartInsertingObjectWithOid(s3, OIDFactory.BuildObjectOID(3), null);
            AssertTrue(cache.IdOfInsertingObject(s1) != StorageEngineConstant.NullObjectId);
            AssertTrue(cache.IdOfInsertingObject(s2) != StorageEngineConstant.NullObjectId);
            AssertTrue(cache.IdOfInsertingObject(s3) != StorageEngineConstant.NullObjectId);
            cache.EndInsertingObject(s3);
            cache.EndInsertingObject(s2);
            cache.EndInsertingObject(s1);
            AssertTrue(cache.IdOfInsertingObject(s1) != StorageEngineConstant.NullObjectId);
            AssertTrue(cache.IdOfInsertingObject(s2) == StorageEngineConstant.NullObjectId);
            AssertTrue(cache.IdOfInsertingObject(s3) == StorageEngineConstant.NullObjectId);
        }

        /// <exception cref="System.IO.IOException"></exception>
        [Test]
        public virtual void Test3()
        {
            var cache = CacheFactory.GetLocalCache("temp");
            var ci = new ClassInfo(OdbClassUtil.GetFullName(GetType()));
            ci.SetPosition(1);
            var oih1 = new ObjectInfoHeader();
            var oih2 = new ObjectInfoHeader();
            var oih3 = new ObjectInfoHeader();
            oih1.SetOid(OIDFactory.BuildObjectOID(1));
            oih2.SetOid(OIDFactory.BuildObjectOID(10));
            oih3.SetOid(OIDFactory.BuildObjectOID(100));
            var nnoi1 = new NonNativeObjectInfo(oih1, ci);
            var nnoi2 = new NonNativeObjectInfo(oih2, ci);
            var nnoi3 = new NonNativeObjectInfo(oih3, ci);
            cache.StartReadingObjectInfoWithOid(OIDFactory.BuildObjectOID(1), nnoi1);
            cache.StartReadingObjectInfoWithOid(OIDFactory.BuildObjectOID(10), nnoi2);
            cache.StartReadingObjectInfoWithOid(OIDFactory.BuildObjectOID(100), nnoi3);
            AssertTrue(cache.IsReadingObjectInfoWithOid(OIDFactory.BuildObjectOID(1)));
            AssertTrue(cache.IsReadingObjectInfoWithOid(OIDFactory.BuildObjectOID(10)));
            AssertTrue(cache.IsReadingObjectInfoWithOid(OIDFactory.BuildObjectOID(100)));
            cache.EndReadingObjectInfo(nnoi1.GetOid());
            cache.EndReadingObjectInfo(nnoi2.GetOid());
            cache.EndReadingObjectInfo(nnoi3.GetOid());
            AssertFalse(cache.IsReadingObjectInfoWithOid(OIDFactory.BuildObjectOID(1)));
            AssertFalse(cache.IsReadingObjectInfoWithOid(OIDFactory.BuildObjectOID(10)));
            AssertFalse(cache.IsReadingObjectInfoWithOid(OIDFactory.BuildObjectOID(100)));
        }
    }
}
