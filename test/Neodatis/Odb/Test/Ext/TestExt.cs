using System.Threading;
using NDatabase.Odb;
using NUnit.Framework;
using Test.Odb.Test;
using Test.Odb.Test.VO.Login;

namespace Ext
{
    [TestFixture]
    public class TestExt : ODBTest
    {
        
        [Test]
        public virtual void TestGetObjectId()
        {
            DeleteBase("extb");
            var odb = OdbFactory.Open("extb");
            var f = new Function("Test Function");
            var oid = odb.Store(f);
            var extOid = odb.Ext().GetObjectExternalOID(f);
            AssertEquals(oid.ObjectId, extOid.ObjectId);
            AssertEquals(odb.Ext().GetDatabaseId(), extOid.GetDatabaseId());
            odb.Close();
            odb = Open("extb");
            // Getting object via external oid
            var f2 = (Function) odb.GetObjectFromId(extOid);
            var lastOid = odb.GetObjectId(f2);
            AssertEquals(oid, lastOid);
            AssertEquals(f.GetName(), f2.GetName());
            odb.Close();
        }

        
        [Test]
        public virtual void TestObjectVersion()
        {
            DeleteBase("extc");
            var odb = Open("extc");
            var size = 100;
            long updateDate = 0;
            long creationDate = 0;
            var oid = odb.Store(new Function("f"));
            odb.Close();
            Thread.Sleep(20);
            // LogUtil.allOn(true);
            for (var i = 0; i < size; i++)
            {
                odb = Open("extc");
                var f = (Function) odb.GetObjectFromId(oid);
                var version = odb.Ext().GetObjectVersion(oid);
                // System.out.println("i="+i + " - v="+ version+ " - oid="+oid);
                updateDate = odb.Ext().GetObjectUpdateDate(oid);
                creationDate = odb.Ext().GetObjectCreationDate(oid);
                f.SetName(f.GetName() + "-" + i);
                // update the object, should increase the version number
                odb.Store(f);
                odb.Close();
                AssertEquals(i + 1, version);
                // System.out.println(creationDate + " - "+ updateDate+ "- "+
                // OdbTime.getCurrentTimeInMs());
                // in first iteration, creation & update date may be equal
                if (i > 0)
                    AssertTrue(creationDate < updateDate);
            }
        }

        
        [Test]
        public virtual void TestObjectVersionWithoutClose()
        {
            DeleteBase("extd");
            var odb = Open("extd");
            var size = 100;
            var oid = odb.Store(new Function("f"));
            odb.Close();
            odb = Open("extd");
            Thread.Sleep(20);
            for (var i = 0; i < size; i++)
            {
                // odb = open("ext");
                var f = (Function) odb.GetObjectFromId(oid);
                var version = odb.Ext().GetObjectVersion(oid);
                // System.out.println("i="+i + " - v="+ version+ " - oid="+oid);
                AssertEquals(i + 1, version);
                f.SetName("f" + i);
                // update the object, should increase the version number
                odb.Store(f);
                odb.Commit();
            }
            odb.Close();
        }

        
        [Test]
        public virtual void TestObjectVersionWithoutClose2()
        {
            DeleteBase("ext2");
            var odb = Open("ext2");
            var size = 100;
            
            var oid = odb.Store(new Function("f"));
            odb.Close();
            odb = Open("ext2");
            Thread.Sleep(20);
            // LogUtil.allOn(true);
            for (var i = 0; i < size; i++)
            {
                // odb = open("ext");
                var f = (Function) odb.GetObjectFromId(oid);
                f.SetName("f" + i);
                odb.Store(f);
                odb.Commit();
            }
            odb.Close();
        }

        
        [Test]
        public virtual void TestTransactionId()
        {
            DeleteBase("ext0");
            var odb = Open("ext0");
            var transactionId = odb.Ext().GetCurrentTransactionId();
            Println(transactionId);
            AssertTrue(transactionId.ToString().StartsWith("tid=01"));
            odb.Close();
            odb = Open("ext0");
            transactionId = odb.Ext().GetCurrentTransactionId();
            Println(transactionId);
            AssertTrue(transactionId.ToString().StartsWith("tid=02"));
            odb.Close();
        }

        
        [Test]
        public virtual void TestTransactionId2()
        {
            DeleteBase("exta");
            IOdb odb = null;
            ITransactionId transactionId = null;
            var size = 200;
            for (var i = 0; i < size; i++)
            {
                odb = Open("exta");
                transactionId = odb.Ext().GetCurrentTransactionId();
                // println(transactionId);
                AssertTrue(transactionId.ToString().StartsWith("tid=0" + (i + 1)));
                odb.Close();
                if (i % 10 == 0)
                    Println("Transaction " + i);
            }
        }
    }
}
