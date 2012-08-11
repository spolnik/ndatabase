using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Odb.Core.Oid;
using NDatabase.Odb.Core.Transaction;
using NDatabase.Odb.Impl.Core.Layers.Layer3.Engine;
using NDatabase.Odb.Impl.Core.Transaction;
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
			ICache cache = CacheFactory
				.GetLocalCache("test");
			string s1 = "ola1";
			string s2 = "ola2";
			string s3 = "ola3";
			cache.StartInsertingObjectWithOid(s1, OIDFactory.BuildObjectOID
				(1), null);
			cache.StartInsertingObjectWithOid(s2, OIDFactory.BuildObjectOID
				(2), null);
			cache.StartInsertingObjectWithOid(s3, OIDFactory.BuildObjectOID
				(3), null);
			AssertTrue(cache.IdOfInsertingObject(s1) != StorageEngineConstant
				.NullObjectId);
			AssertTrue(cache.IdOfInsertingObject(s2) != StorageEngineConstant
				.NullObjectId);
			AssertTrue(cache.IdOfInsertingObject(s3) != StorageEngineConstant
				.NullObjectId);
			cache.EndInsertingObject(s3);
			cache.EndInsertingObject(s2);
			cache.EndInsertingObject(s1);
			AssertTrue(cache.IdOfInsertingObject(s1) == StorageEngineConstant
				.NullObjectId);
			AssertTrue(cache.IdOfInsertingObject(s2) == StorageEngineConstant
				.NullObjectId);
			AssertTrue(cache.IdOfInsertingObject(s3) == StorageEngineConstant
				.NullObjectId);
		}

		/// <exception cref="System.IO.IOException"></exception>
		[Test]
        public virtual void Test2()
		{
			ICache cache = CacheFactory
				.GetLocalCache("temp");
			string s1 = "ola1";
			string s2 = "ola2";
			string s3 = "ola3";
			for (int i = 0; i < 1000 * 3; i += 3)
			{
				cache.StartInsertingObjectWithOid(s1, OIDFactory.BuildObjectOID(i + 1), null);
				cache.StartInsertingObjectWithOid(s2, OIDFactory.BuildObjectOID(i + 2), null);
				cache.StartInsertingObjectWithOid(s3, OIDFactory.BuildObjectOID(i + 3), null);
			}
			AssertEquals(1000, cache.InsertingLevelOf(s1));
			AssertEquals(1000, cache.InsertingLevelOf(s2));
			AssertEquals(1000, cache.InsertingLevelOf(s3));
			for (int i = 0; i < 1000; i++)
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
			ICache cache = CacheFactory
				.GetLocalCache("temp");
			ClassInfo ci = new ClassInfo
				(this.GetType().FullName);
			ci.SetPosition(1);
			ObjectInfoHeader oih1 = new ObjectInfoHeader
				();
			ObjectInfoHeader oih2 = new ObjectInfoHeader
				();
			ObjectInfoHeader oih3 = new ObjectInfoHeader
				();
			oih1.SetOid(OIDFactory.BuildObjectOID(1));
			oih2.SetOid(OIDFactory.BuildObjectOID(10));
			oih3.SetOid(OIDFactory.BuildObjectOID(100));
			NonNativeObjectInfo nnoi1 = new NonNativeObjectInfo
				(oih1, ci);
			NonNativeObjectInfo nnoi2 = new NonNativeObjectInfo
				(oih2, ci);
			NonNativeObjectInfo nnoi3 = new NonNativeObjectInfo
				(oih3, ci);
			cache.StartReadingObjectInfoWithOid(OIDFactory.BuildObjectOID
				(1), nnoi1);
			cache.StartReadingObjectInfoWithOid(OIDFactory.BuildObjectOID
				(10), nnoi2);
			cache.StartReadingObjectInfoWithOid(OIDFactory.BuildObjectOID
				(100), nnoi3);
			AssertTrue(cache.IsReadingObjectInfoWithOid(OIDFactory.BuildObjectOID
				(1)));
			AssertTrue(cache.IsReadingObjectInfoWithOid(OIDFactory.BuildObjectOID
				(10)));
			AssertTrue(cache.IsReadingObjectInfoWithOid(OIDFactory.BuildObjectOID
				(100)));
			cache.EndReadingObjectInfo(nnoi1.GetOid());
			cache.EndReadingObjectInfo(nnoi2.GetOid());
			cache.EndReadingObjectInfo(nnoi3.GetOid());
			AssertFalse(cache.IsReadingObjectInfoWithOid(OIDFactory.BuildObjectOID
				(1)));
			AssertFalse(cache.IsReadingObjectInfoWithOid(OIDFactory.BuildObjectOID
				(10)));
			AssertFalse(cache.IsReadingObjectInfoWithOid(OIDFactory.BuildObjectOID
				(100)));
		}
	}
}
