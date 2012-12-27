using System.Collections;
using NDatabase2.Odb;
using NDatabase2.Odb.Core.Layers.Layer1.Introspector;
using NDatabase2.Odb.Core.Layers.Layer2.Meta;
using NDatabase2.Odb.Core.Layers.Layer2.Meta.Compare;
using NDatabase2.Odb.Core.Layers.Layer3.Engine;
using NDatabase2.Odb.Core.Oid;
using NDatabase2.Odb.Main;
using NDatabase2.Tool.Wrappers;
using NDatabase2.Tool.Wrappers.Map;
using NUnit.Framework;
using Test.NDatabase.Odb.Test.IO;
using Test.NDatabase.Odb.Test.VO.Inheritance;
using Test.NDatabase.Odb.Test.VO.Login;

namespace Test.NDatabase.Odb.Test.Intropector
{
    public class InstrospectorTest : ODBTest
    {
        public override void SetUp()
        {
            base.SetUp();
            new StorageEngine(new MockFileIdentification()).AddSession(
                new MockSession("test"), false);
        }

        [Test]
        public virtual void TestClassInfo()
        {
            var user = new User("olivier smadja", "olivier@neodatis.com",
                                new Profile("operator", new VO.Login.Function("login")));
            var classInfoList = ClassIntrospector.Introspect(user.GetType(), true);
            AssertEquals(OdbClassUtil.GetFullName(user.GetType()), classInfoList.GetMainClassInfo().FullClassName);
            AssertEquals(3, classInfoList.GetMainClassInfo().Attributes.Count);
            AssertEquals(3, classInfoList.GetClassInfos().Count);
        }

        [Test]
        public virtual void TestInstanceInfo()
        {
            var dbName = "TestInstanceInfo.odb";
            DeleteBase(dbName);
            var odb = OdbFactory.Open(dbName);

            var user = new User("olivier smadja", "olivier@neodatis.com",
                                new Profile("operator", new VO.Login.Function("login")));
            var ci = ClassIntrospector.Introspect(user.GetType(), true).GetMainClassInfo();

            var storageEngine = ((OdbAdapter)odb).GetStorageEngine();

            var instanceInfo =
                (NonNativeObjectInfo)
                new ObjectIntrospector(storageEngine).GetMetaRepresentation(user, ci, true, null,
                                                                            new InstrumentationCallbackForStore(null,
                                                                                                                false));
            AssertEquals(OdbClassUtil.GetFullName(user.GetType()), instanceInfo.GetClassInfo().FullClassName);
            AssertEquals("olivier smadja", instanceInfo.GetAttributeValueFromId(ci.GetAttributeId("name")).ToString());
            AssertEquals(typeof (AtomicNativeObjectInfo),
                         instanceInfo.GetAttributeValueFromId(ci.GetAttributeId("name")).GetType());

            odb.Close();
        }

        [Test]
        public virtual void TestInstanceInfo2()
        {
            var dbName = "TestInstanceInfo2.odb";
            DeleteBase(dbName);
            var odb = OdbFactory.Open(dbName);

            var user = new User("olivier smadja", "olivier@neodatis.com",
                                new Profile("operator", new VO.Login.Function("login")));
            var ci = ClassIntrospector.Introspect(user.GetType(), true).GetMainClassInfo();

            var storageEngine = ((OdbAdapter)odb).GetStorageEngine();

            var instanceInfo =
                (NonNativeObjectInfo)
                new ObjectIntrospector(storageEngine).GetMetaRepresentation(user, ci, true, null,
                                                                            new InstrumentationCallbackForStore(null,
                                                                                                                false));
            AssertEquals(instanceInfo.GetClassInfo().FullClassName, OdbClassUtil.GetFullName(user.GetType()));
            AssertEquals(instanceInfo.GetAttributeValueFromId(ci.GetAttributeId("name")).ToString(), "olivier smadja");

            odb.Close();
        }

        [Test]
        public virtual void TestCompareCollection1()
        {
            var dbName = "introspectortest1.odb";
            DeleteBase(dbName);
            var odb = OdbFactory.Open(dbName);

            var user = new User("olivier smadja", "olivier@neodatis.com",
                                new Profile("operator", new VO.Login.Function("login")));
            IObjectInfoComparator comparator = new ObjectInfoComparator();
            var ci = ClassIntrospector.Introspect(user.GetType(), true).GetMainClassInfo();

            var storageEngine = ((OdbAdapter)odb).GetStorageEngine();

            var instanceInfo =
                (NonNativeObjectInfo)
                new ObjectIntrospector(storageEngine).GetMetaRepresentation(user, ci, true, null,
                                                                            new InstrumentationCallbackForStore(null,
                                                                                                                false));
            // Sets attributes offsets - this is normally done by reading then from
            // disk, but in this junit,
            // we must set them manually
            var offsets = new[] {1L, 2L, 3L};
            var ids = new[] {1, 2, 3};
            instanceInfo.GetHeader().SetAttributesIdentification(offsets);
            instanceInfo.GetHeader().SetAttributesIds(ids);
            instanceInfo.GetHeader().SetOid(OIDFactory.BuildObjectOID(1));
            var nnoiProfile = (NonNativeObjectInfo) instanceInfo.GetAttributeValueFromId(3);
            nnoiProfile.SetOid(OIDFactory.BuildObjectOID(2));

            var nnoiFunctions = (NonNativeObjectInfo) nnoiProfile.GetAttributeValueFromId(1);
            nnoiFunctions.SetOid(OIDFactory.BuildObjectOID(3));

            user.SetName("Olivier Smadja");
            var instanceInfo3 =
                (NonNativeObjectInfo)
                new ObjectIntrospector(storageEngine).GetMetaRepresentation(user, ci, true, null,
                                                                            new InstrumentationCallbackForStore(null,
                                                                                                                false));
            instanceInfo3.GetHeader().SetOid(OIDFactory.BuildObjectOID(1));
            nnoiProfile = (NonNativeObjectInfo) instanceInfo3.GetAttributeValueFromId(3);
            nnoiProfile.SetOid(OIDFactory.BuildObjectOID(2));

            nnoiFunctions = (NonNativeObjectInfo)nnoiProfile.GetAttributeValueFromId(1);
            nnoiFunctions.SetOid(OIDFactory.BuildObjectOID(3));

            AssertTrue(comparator.HasChanged(instanceInfo, instanceInfo3));
            AssertEquals(1, comparator.GetNbChanges());
            AssertEquals(1, comparator.GetChangedAttributeActions().Count);
            var cnaa = (ChangedNativeAttributeAction) comparator.GetChangedAttributeActions()[0];
            AssertEquals("Olivier Smadja", cnaa.GetNoiWithNewValue().GetObject());

            odb.Close();
        }

        [Test]
        public virtual void TestCompareCollection11()
        {
            var dbName = "introspectortest2.odb";
            DeleteBase(dbName);
            var odb = OdbFactory.Open(dbName);

            var user = new User("olivier smadja", "olivier@neodatis.com",
                                new Profile("operator", new VO.Login.Function("login")));
            IObjectInfoComparator comparator = new ObjectInfoComparator();
            var ci = ClassIntrospector.Introspect(user.GetType(), true).GetMainClassInfo();

            var storageEngine = ((OdbAdapter)odb).GetStorageEngine();

            var instanceInfo =
                (NonNativeObjectInfo)
                new ObjectIntrospector(storageEngine).GetMetaRepresentation(user, ci, true, null,
                                                                            new InstrumentationCallbackForStore(null,
                                                                                                                false));
            // Sets attributes offsets - this is normally done by reading then from
            // disk, but in this junit,
            // we must set them manually
            var offsets = new[] {1L, 2L, 3L};
            var ids = new[] {1, 2, 3};
            instanceInfo.GetHeader().SetAttributesIdentification(offsets);
            instanceInfo.GetHeader().SetAttributesIds(ids);
            instanceInfo.GetHeader().SetOid(OIDFactory.BuildObjectOID(1));
            object o = instanceInfo.GetAttributeValueFromId(3);
            var nnoiProfile = (NonNativeObjectInfo) o;
            nnoiProfile.SetOid(OIDFactory.BuildObjectOID(2));

            var nnoiFunctions = (NonNativeObjectInfo)nnoiProfile.GetAttributeValueFromId(1);
            nnoiFunctions.SetOid(OIDFactory.BuildObjectOID(3));

            user.SetName("Olivier Smadja");
            user.SetEmail("olivier@neodatis.org");
            var instanceInfo3 =
                (NonNativeObjectInfo)
                new ObjectIntrospector(storageEngine).GetMetaRepresentation(user, ci, true, null,
                                                                            new InstrumentationCallbackForStore(null,
                                                                                                                false));
            instanceInfo3.GetHeader().SetOid(OIDFactory.BuildObjectOID(1));
            nnoiProfile = (NonNativeObjectInfo) instanceInfo3.GetAttributeValueFromId(3);
            nnoiProfile.SetOid(OIDFactory.BuildObjectOID(2));

            nnoiFunctions = (NonNativeObjectInfo)nnoiProfile.GetAttributeValueFromId(1);
            nnoiFunctions.SetOid(OIDFactory.BuildObjectOID(3));

            AssertTrue(comparator.HasChanged(instanceInfo, instanceInfo3));
            AssertEquals(2, comparator.GetNbChanges());
            AssertEquals(2, comparator.GetChangedAttributeActions().Count);
            var cnaa = (ChangedNativeAttributeAction) comparator.GetChangedAttributeActions()[1];
            AssertEquals("Olivier Smadja", cnaa.GetNoiWithNewValue().GetObject());
            cnaa = (ChangedNativeAttributeAction) comparator.GetChangedAttributeActions()[0];
            AssertEquals("olivier@neodatis.org", cnaa.GetNoiWithNewValue().GetObject());

            odb.Close();
        }

        [Test]
        public virtual void TestCompareCollection2()
        {
            var dbName = "introspectortest3.odb";
            DeleteBase(dbName);
            var odb = OdbFactory.Open(dbName);

            var user = new User("olivier smadja", "olivier@neodatis.com",
                                new Profile("operator", new VO.Login.Function("login")));

            IObjectInfoComparator comparator = new ObjectInfoComparator();
            var ci = ClassIntrospector.Introspect(user.GetType(), true).GetMainClassInfo();

            var storageEngine = ((OdbAdapter)odb).GetStorageEngine();

            var instanceInfo =
                (NonNativeObjectInfo)
                new ObjectIntrospector(storageEngine).GetMetaRepresentation(user, ci, true, null,
                                                                            new InstrumentationCallbackForStore(null,
                                                                                                                false));
            // Sets attributes offsets - this is normally done by reading them from
            // disk, but in this junit,
            // we must set them manually
            var offsets = new[] {1L, 2L, 3L};
            var ids = new[] {1, 2, 3};
            instanceInfo.GetHeader().SetAttributesIdentification(offsets);
            instanceInfo.GetHeader().SetAttributesIds(ids);
            instanceInfo.GetHeader().SetOid(OIDFactory.BuildObjectOID(1));

            var nnoiProfile = (NonNativeObjectInfo) instanceInfo.GetAttributeValueFromId(3);
            nnoiProfile.SetOid(OIDFactory.BuildObjectOID(2));

            var nnoiFunctions = (NonNativeObjectInfo)nnoiProfile.GetAttributeValueFromId(1);
            nnoiFunctions.SetOid(OIDFactory.BuildObjectOID(3));

            user.SetName(null);

            var instanceInfo3 =
                (NonNativeObjectInfo)
                new ObjectIntrospector(storageEngine).GetMetaRepresentation(user, ci, true, null,
                                                                            new InstrumentationCallbackForStore(null,
                                                                                                                false));
            instanceInfo3.GetHeader().SetAttributesIdentification(offsets);
            instanceInfo3.GetHeader().SetAttributesIds(ids);
            instanceInfo3.GetHeader().SetOid(OIDFactory.BuildObjectOID(1));

            nnoiProfile = (NonNativeObjectInfo) instanceInfo3.GetAttributeValueFromId(3);
            nnoiProfile.SetOid(OIDFactory.BuildObjectOID(2));

            nnoiFunctions = (NonNativeObjectInfo)nnoiProfile.GetAttributeValueFromId(1);
            nnoiFunctions.SetOid(OIDFactory.BuildObjectOID(3));

            AssertTrue(comparator.HasChanged(instanceInfo, instanceInfo3));
            AssertEquals(1, comparator.GetNbChanges());
            AssertEquals(1, comparator.GetAttributeToSetToNull().Count);
            var cnaa = comparator.GetAttributeToSetToNull()[0];
            AssertEquals(2, cnaa.GetAttributeId());

            odb.Close();
        }

        [Test]
        [Ignore("To be removed if collections will be native")]
        public virtual void TestCompareCollection3CollectionContentChange()
        {
            var dbName = "introspectortest4.odb";
            DeleteBase(dbName);
            var odb = OdbFactory.Open(dbName);

            var function = new VO.Login.Function("login");
            var user = new User("olivier smadja", "olivier@neodatis.com", new Profile("operator", function));
            IObjectInfoComparator comparator = new ObjectInfoComparator();
            var ci = ClassIntrospector.Introspect(user.GetType(), true).GetMainClassInfo();

            var storageEngine = ((OdbAdapter)odb).GetStorageEngine();

            var instanceInfo =
                (NonNativeObjectInfo)
                new ObjectIntrospector(storageEngine).GetMetaRepresentation(user, ci, true, null,
                                                                            new InstrumentationCallbackForStore(null,
                                                                                                                false));
            // Sets attributes offsets - this is normally done by reading then from
            // disk, but in this junit,
            // we must set them manually
            var offsets = new[] {1L, 2L, 3L};
            var ids = new[] {1, 2, 3};
            instanceInfo.GetHeader().SetAttributesIdentification(offsets);
            instanceInfo.GetHeader().SetAttributesIds(ids);
            instanceInfo.GetHeader().SetOid(OIDFactory.BuildObjectOID(1));
            var nnoiProfile = (NonNativeObjectInfo) instanceInfo.GetAttributeValueFromId(3);
            nnoiProfile.SetOid(OIDFactory.BuildObjectOID(2));
            var nnoi = (NonNativeObjectInfo) instanceInfo.GetAttributeValueFromId(ci.GetAttributeId("profile"));
            nnoi.GetHeader().SetAttributesIdentification(offsets);
            nnoi.GetHeader().SetAttributesIds(ids);
            nnoi.SetOid(OIDFactory.BuildObjectOID(2));
            var nnoi2 =
                (CollectionObjectInfo) nnoi.GetAttributeValueFromId(nnoi.GetClassInfo().GetAttributeId("functions"));

            var enumerator = nnoi2.GetCollection().GetEnumerator();
            enumerator.MoveNext();
            var nnoi3 = (NonNativeObjectInfo) enumerator.Current;
            nnoi3.GetHeader().SetAttributesIdentification(offsets);
            nnoi3.GetHeader().SetAttributesIds(ids);
            function.SetName("login function");
            var instanceInfo3 =
                (NonNativeObjectInfo)
                new ObjectIntrospector(storageEngine).GetMetaRepresentation(user, ci, true, null,
                                                                            new InstrumentationCallbackForStore(null,
                                                                                                                false));
            instanceInfo3.GetHeader().SetOid(OIDFactory.BuildObjectOID(1));
            nnoiProfile = (NonNativeObjectInfo) instanceInfo3.GetAttributeValueFromId(3);
            nnoiProfile.SetOid(OIDFactory.BuildObjectOID(2));
            AssertTrue(comparator.HasChanged(instanceInfo, instanceInfo3));
            AssertEquals(1, comparator.GetNbChanges());
            var cnaa = (ChangedNativeAttributeAction) comparator.GetChangedAttributeActions()[0];
            AssertEquals(1, comparator.GetChangedAttributeActions().Count);
            AssertEquals(function.GetName(), cnaa.GetNoiWithNewValue().GetObject());

            odb.Close();
        }

        [Test]
        public virtual void TestCompareCollection4CollectionContentChange()
        {
            var dbName = "introspectortest22.odb";
            DeleteBase(dbName);
            var odb = OdbFactory.Open(dbName);

            var function = new VO.Login.Function("login");
            var user = new User("olivier smadja", "olivier@neodatis.com", new Profile("operator", function));

            var ci = ClassIntrospector.Introspect(user.GetType(), true).GetMainClassInfo();

            var storageEngine = ((OdbAdapter)odb).GetStorageEngine();

            var instanceInfo =
                (NonNativeObjectInfo)
                new ObjectIntrospector(storageEngine).GetMetaRepresentation(user, ci, true, null,
                                                                            new InstrumentationCallbackForStore(null,
                                                                                                                false));
            // Sets attributes offsets - this is normally done by reading then from
            // disk, but in this junit,
            // we must set them manually
            var offsets = new[] {1L, 2L, 3L};
            var ids = new[] {1, 2, 3};
            instanceInfo.GetHeader().SetAttributesIdentification(offsets);
            instanceInfo.GetHeader().SetAttributesIds(ids);
            instanceInfo.GetHeader().SetOid(OIDFactory.BuildObjectOID(1));
            var nnoiProfile = (NonNativeObjectInfo) instanceInfo.GetAttributeValueFromId(3);
            nnoiProfile.SetOid(OIDFactory.BuildObjectOID(2));

            var nnoiFunctions = (NonNativeObjectInfo)nnoiProfile.GetAttributeValueFromId(1);
            nnoiFunctions.SetOid(OIDFactory.BuildObjectOID(3));

            function.SetName(null);

            var instanceInfo3 =
                (NonNativeObjectInfo)
                new ObjectIntrospector(storageEngine).GetMetaRepresentation(user, ci, true, null,
                                                                            new InstrumentationCallbackForStore(null,
                                                                                                                false));
            instanceInfo3.GetHeader().SetAttributesIdentification(offsets);
            instanceInfo3.GetHeader().SetAttributesIds(ids);
            instanceInfo3.GetHeader().SetOid(OIDFactory.BuildObjectOID(1));

            nnoiProfile = (NonNativeObjectInfo) instanceInfo3.GetAttributeValueFromId(3);
            nnoiProfile.SetOid(OIDFactory.BuildObjectOID(2));

            nnoiFunctions = (NonNativeObjectInfo)nnoiProfile.GetAttributeValueFromId(1);
            nnoiFunctions.SetOid(OIDFactory.BuildObjectOID(3));

            IObjectInfoComparator comparator = new ObjectInfoComparator();
            AssertTrue(comparator.HasChanged(instanceInfo, instanceInfo3));
            AssertEquals(2, comparator.GetNbChanges());
            AssertEquals(1, comparator.GetAttributeToSetToNull().Count);
            AssertEquals(1, comparator.GetArrayChanges().Count);
            var cnaa = comparator.GetAttributeToSetToNull()[0];
            AssertEquals(function.GetName(), ((VO.Login.Function) cnaa.GetNnoi().GetObject()).GetName());

            odb.Close();
        }

        [Test]
        public virtual void TestCompareCollection5()
        {
            var dbName = "introspectortest5.odb";
            DeleteBase(dbName);
            var odb = OdbFactory.Open(dbName);

            var function = new VO.Login.Function("login");
            var profile = new Profile("operator", function);
            var user = new User("olivier smadja", "olivier@neodatis.com", profile);
            IObjectInfoComparator comparator = new ObjectInfoComparator();
            var ci = ClassIntrospector.Introspect(user.GetType(), true).GetMainClassInfo();

            var storageEngine = ((OdbAdapter)odb).GetStorageEngine();

            var instanceInfo =
                (NonNativeObjectInfo)
                new ObjectIntrospector(storageEngine).GetMetaRepresentation(user, ci, true, null,
                                                                            new InstrumentationCallbackForStore(null,
                                                                                                                false));
            // Sets attributes offsets - this is normally done by reading then from
            // disk, but in this junit,
            // we must set them manually
            var offsets = new[] {1L, 2L, 3L};
            var ids = new[] {1, 2, 3};
            instanceInfo.GetHeader().SetAttributesIdentification(offsets);
            instanceInfo.GetHeader().SetAttributesIds(ids);
            instanceInfo.GetHeader().SetOid(OIDFactory.BuildObjectOID(1));
            var nnoiProfile = (NonNativeObjectInfo) instanceInfo.GetAttributeValueFromId(3);
            nnoiProfile.SetOid(OIDFactory.BuildObjectOID(2));

            var nnoiFunctions = (NonNativeObjectInfo)nnoiProfile.GetAttributeValueFromId(1);
            nnoiFunctions.SetOid(OIDFactory.BuildObjectOID(3));

            profile.GetFunctions().Add(new VO.Login.Function("logout"));
            var instanceInfo3 =
                (NonNativeObjectInfo)
                new ObjectIntrospector(storageEngine).GetMetaRepresentation(user, ci, true, null,
                                                                            new InstrumentationCallbackForStore(null,
                                                                                                                false));
            instanceInfo3.GetHeader().SetOid(OIDFactory.BuildObjectOID(1));
            nnoiProfile = (NonNativeObjectInfo) instanceInfo3.GetAttributeValueFromId(3);
            nnoiProfile.SetOid(OIDFactory.BuildObjectOID(2));

            nnoiFunctions = (NonNativeObjectInfo)nnoiProfile.GetAttributeValueFromId(1);
            nnoiFunctions.SetOid(OIDFactory.BuildObjectOID(3));

            AssertTrue(comparator.HasChanged(instanceInfo, instanceInfo3));
            AssertEquals(3, comparator.GetNbChanges());
            
            odb.Close();
        }

        [Test]
        public virtual void TestCompareCollection6()
        {
            var dbName = "introspectortest6.odb";
            DeleteBase(dbName);
            var odb = OdbFactory.Open(dbName);

            var function = new VO.Login.Function("login");
            var profile = new Profile("operator", function);
            var user = new User("olivier smadja", "olivier@neodatis.com", profile);
            IObjectInfoComparator comparator = new ObjectInfoComparator();
            var ci = ClassIntrospector.Introspect(user.GetType(), true).GetMainClassInfo();

            var storageEngine = ((OdbAdapter)odb).GetStorageEngine();

            var instanceInfo =
                (NonNativeObjectInfo)
                new ObjectIntrospector(storageEngine).GetMetaRepresentation(user, ci, true, null,
                                                                            new InstrumentationCallbackForStore(null,
                                                                                                                false));
            // Sets attributes offsets - this is normally done by reading then from
            // disk, but in this junit,
            // we must set them manually
            var offsets = new[] {1L, 2L, 3L};
            var ids = new[] {1, 2, 3};
            instanceInfo.GetHeader().SetAttributesIdentification(offsets);
            instanceInfo.GetHeader().SetAttributesIds(ids);
            instanceInfo.GetHeader().SetOid(OIDFactory.BuildObjectOID(1));
            var nnoiProfile = (NonNativeObjectInfo) instanceInfo.GetAttributeValueFromId(3);
            nnoiProfile.SetOid(OIDFactory.BuildObjectOID(2));

            var nnoiFunctions = (NonNativeObjectInfo)nnoiProfile.GetAttributeValueFromId(1);
            nnoiFunctions.SetOid(OIDFactory.BuildObjectOID(3));

            var nnoi = (NonNativeObjectInfo) instanceInfo.GetAttributeValueFromId(ci.GetAttributeId("profile"));
            nnoi.GetHeader().SetAttributesIdentification(offsets);
            nnoi.GetHeader().SetAttributesIds(ids);
            profile.SetName("ope");
            var instanceInfo3 =
                (NonNativeObjectInfo)
                new ObjectIntrospector(storageEngine).GetMetaRepresentation(user, ci, true, null,
                                                                            new InstrumentationCallbackForStore(null,
                                                                                                                false));
            instanceInfo3.GetHeader().SetOid(OIDFactory.BuildObjectOID(1));
            nnoiProfile = (NonNativeObjectInfo) instanceInfo3.GetAttributeValueFromId(3);
            nnoiProfile.SetOid(OIDFactory.BuildObjectOID(2));

            nnoiFunctions = (NonNativeObjectInfo)nnoiProfile.GetAttributeValueFromId(1);
            nnoiFunctions.SetOid(OIDFactory.BuildObjectOID(3));

            AssertTrue(comparator.HasChanged(instanceInfo, instanceInfo3));
            AssertEquals(1, comparator.GetNbChanges());
            var cnaa = (ChangedNativeAttributeAction) comparator.GetChangedAttributeActions()[0];
            AssertEquals(1, comparator.GetChangedAttributeActions().Count);
            AssertEquals(profile.GetName(), cnaa.GetNoiWithNewValue().GetObject());

            odb.Close();
        }

        [Test]
        public virtual void TestCompareCollection7()
        {
            var dbName = "introspectortest7.odb";
            DeleteBase(dbName);
            var odb = OdbFactory.Open(dbName);

            var function = new VO.Login.Function("login");
            var profile = new Profile("operator", function);
            var user = new User("olivier smadja", "olivier@neodatis.com", profile);
            IObjectInfoComparator comparator = new ObjectInfoComparator();
            var ci = ClassIntrospector.Introspect(user.GetType(), true).GetMainClassInfo();

            var storageEngine = ((OdbAdapter)odb).GetStorageEngine();

            var instanceInfo =
                (NonNativeObjectInfo)
                new ObjectIntrospector(storageEngine).GetMetaRepresentation(user, ci, true, null,
                                                                            new InstrumentationCallbackForStore(null,
                                                                                                                false));
            // Sets attributes offsets - this is normally done by reading then from
            // disk, but in this junit,
            // we must set them manually
            var offsets = new[] {1L, 2L, 3L};
            var ids = new[] {1, 2, 3};
            instanceInfo.GetHeader().SetAttributesIdentification(offsets);
            instanceInfo.GetHeader().SetAttributesIds(ids);
            instanceInfo.GetHeader().SetOid(OIDFactory.BuildObjectOID(1));
            var nnoiProfile = (NonNativeObjectInfo) instanceInfo.GetAttributeValueFromId(3);
            nnoiProfile.SetOid(OIDFactory.BuildObjectOID(2));

            var nnoiFunctions = (NonNativeObjectInfo)nnoiProfile.GetAttributeValueFromId(1);
            nnoiFunctions.SetOid(OIDFactory.BuildObjectOID(3));

            // / Set the same name
            profile.SetName("operator");
            var instanceInfo3 =
                (NonNativeObjectInfo)
                new ObjectIntrospector(storageEngine).GetMetaRepresentation(user, ci, true, null,
                                                                            new InstrumentationCallbackForStore(null,
                                                                                                                false));
            instanceInfo3.GetHeader().SetOid(OIDFactory.BuildObjectOID(1));
            nnoiProfile = (NonNativeObjectInfo) instanceInfo3.GetAttributeValueFromId(3);
            nnoiProfile.SetOid(OIDFactory.BuildObjectOID(2));

            nnoiFunctions = (NonNativeObjectInfo)nnoiProfile.GetAttributeValueFromId(1);
            nnoiFunctions.SetOid(OIDFactory.BuildObjectOID(3));

            AssertFalse(comparator.HasChanged(instanceInfo, instanceInfo3));
            AssertEquals(0, comparator.GetNbChanges());

            odb.Close();
        }

        [Test]
        public virtual void TestCompareCollection8()
        {
            var dbName = "introspectortest8.odb";
            DeleteBase(dbName);
            var odb = OdbFactory.Open(dbName);

            var function = new VO.Login.Function("login");
            var profile = new Profile("operator", function);
            var user = new User("olivier smadja", "olivier@neodatis.com", profile);
            IObjectInfoComparator comparator = new ObjectInfoComparator();
            var ci = ClassIntrospector.Introspect(user.GetType(), true).GetMainClassInfo();

            var storageEngine = ((OdbAdapter)odb).GetStorageEngine();

            var instanceInfo =
                (NonNativeObjectInfo)
                new ObjectIntrospector(storageEngine).GetMetaRepresentation(user, ci, true, null,
                                                                            new InstrumentationCallbackForStore(null,
                                                                                                                false));
            // Sets attributes offsets - this is normally done by reading then from
            // disk, but in this junit,
            // we must set them manually
            var offsets = new[] {1L, 2L, 3L};
            var ids = new[] {1, 2, 3};
            instanceInfo.GetHeader().SetAttributesIdentification(offsets);
            instanceInfo.GetHeader().SetAttributesIds(ids);
            user.SetProfile(null);
            var instanceInfo3 =
                (NonNativeObjectInfo)
                new ObjectIntrospector(storageEngine).GetMetaRepresentation(user, ci, true, null,
                                                                            new InstrumentationCallbackForStore(null,
                                                                                                                false));
            AssertTrue(comparator.HasChanged(instanceInfo, instanceInfo3));
            AssertEquals(1, comparator.GetNbChanges());
            AssertEquals(1, comparator.GetAttributeToSetToNull().Count);
            var o = comparator.GetAttributeToSetToNull()[0];
            AssertEquals(0, comparator.GetChangedAttributeActions().Count);
            AssertEquals(3, o.GetAttributeId());

            odb.Close();
        }

        [Test]
        public virtual void TestCompareCollection9()
        {
            var dbName = "introspectortest9.odb";
            DeleteBase(dbName);
            var odb = OdbFactory.Open(dbName);

            var function = new VO.Login.Function("login");
            var profile = new Profile("operator", function);
            var user = new User("olivier smadja", "olivier@neodatis.com", profile);
            IObjectInfoComparator comparator = new ObjectInfoComparator();
            var ci = ClassIntrospector.Introspect(user.GetType(), true).GetMainClassInfo();

            var storageEngine = ((OdbAdapter)odb).GetStorageEngine();

            var instanceInfo =
                (NonNativeObjectInfo)
                new ObjectIntrospector(storageEngine).GetMetaRepresentation(user, ci, true, null,
                                                                            new InstrumentationCallbackForStore(null,
                                                                                                                false));
            // Sets attributes offsets - this is normally done by reading then from
            // disk, but in this junit,
            // we must set them manually
            var offsets = new[] {1L, 2L, 3L};
            var ids = new[] {1, 2, 3};
            instanceInfo.GetHeader().SetAttributesIdentification(offsets);
            instanceInfo.GetHeader().SetAttributesIds(ids);
            instanceInfo.GetHeader().SetOid(OIDFactory.BuildObjectOID(1));
            var nnoiProfile = (NonNativeObjectInfo) instanceInfo.GetAttributeValueFromId(3);
            nnoiProfile.SetOid(OIDFactory.BuildObjectOID(2));

            var nnoiFunctions = (NonNativeObjectInfo)nnoiProfile.GetAttributeValueFromId(1);
            nnoiFunctions.SetOid(OIDFactory.BuildObjectOID(3));

            user.SetName("Kiko");
            var instanceInfo3 =
                (NonNativeObjectInfo)
                new ObjectIntrospector(storageEngine).GetMetaRepresentation(user, ci, true, null,
                                                                            new InstrumentationCallbackForStore(null,
                                                                                                                false));
            instanceInfo3.GetHeader().SetOid(OIDFactory.BuildObjectOID(1));
            nnoiProfile = (NonNativeObjectInfo) instanceInfo3.GetAttributeValueFromId(3);
            nnoiProfile.SetOid(OIDFactory.BuildObjectOID(2));

            nnoiFunctions = (NonNativeObjectInfo)nnoiProfile.GetAttributeValueFromId(1);
            nnoiFunctions.SetOid(OIDFactory.BuildObjectOID(3));

            AssertTrue(comparator.HasChanged(instanceInfo, instanceInfo3));
            AssertEquals(1, comparator.GetNbChanges());
            var cnaa = (ChangedNativeAttributeAction) comparator.GetChangedAttributeActions()[0];
            AssertEquals(1, comparator.GetChangedAttributeActions().Count);
            AssertEquals(user.GetName(), cnaa.GetNoiWithNewValue().GetObject());

            odb.Close();
        }

        [Test]
        public virtual void TestGetAllFields()
        {
            var allFields = ClassIntrospector.GetAllFieldsFrom(typeof (FootballPlayer));
            AssertEquals(3, allFields.Count);
            AssertEquals("groundName", (allFields[0]).Name);
            AssertEquals("name", (allFields[1]).Name);
            AssertEquals("role", (allFields[2]).Name);
        }

        [Test]
        public virtual void TestIntrospectWithNull()
        {
            var dbName = "TestIntrospectWithNull.odb";
            DeleteBase(dbName);
            var odb = OdbFactory.Open(dbName);

            var user = new User("olivier smadja", "olivier@neodatis.com", null);
            IObjectInfoComparator comparator = new ObjectInfoComparator();
            var ci = ClassIntrospector.Introspect(user.GetType(), true).GetMainClassInfo();

            var storageEngine = ((OdbAdapter)odb).GetStorageEngine();

            var instanceInfo =
                (NonNativeObjectInfo)
                new ObjectIntrospector(storageEngine).GetMetaRepresentation(user, ci, true, null,
                                                                            new InstrumentationCallbackForStore(null,
                                                                                                                false));
            // Sets attributes offsets - this is normally done by reading then from
            // disk, but in this junit,
            // we must set them manually
            var offsets = new[] {1L, 2L, 3L};
            var ids = new[] {1, 2, 3};
            instanceInfo.GetHeader().SetAttributesIdentification(offsets);
            instanceInfo.GetHeader().SetAttributesIds(ids);
            instanceInfo.GetHeader().SetOid(OIDFactory.BuildObjectOID(1));
            object o = instanceInfo.GetAttributeValueFromId(3);
            var nnoiProfile = (NonNativeObjectInfo) o;
            nnoiProfile.SetOid(OIDFactory.BuildObjectOID(2));
            user.SetName("Olivier Smadja");
            user.SetEmail("olivier@neodatis.org");
            user.SetProfile(new Profile("pname"));
            var instanceInfo3 =
                (NonNativeObjectInfo)
                new ObjectIntrospector(storageEngine).GetMetaRepresentation(user, ci, true, null,
                                                                            new InstrumentationCallbackForStore(null,
                                                                                                                false));
            instanceInfo3.GetHeader().SetOid(OIDFactory.BuildObjectOID(1));
            nnoiProfile = (NonNativeObjectInfo) instanceInfo3.GetAttributeValueFromId(3);
            nnoiProfile.SetOid(OIDFactory.BuildObjectOID(2));
            AssertTrue(comparator.HasChanged(instanceInfo, instanceInfo3));
            AssertEquals(3, comparator.GetNbChanges());
            AssertEquals(2, comparator.GetChangedAttributeActions().Count);
            var cnaa = (ChangedNativeAttributeAction) comparator.GetChangedAttributeActions()[1];
            AssertEquals("Olivier Smadja", cnaa.GetNoiWithNewValue().GetObject());
            cnaa = (ChangedNativeAttributeAction) comparator.GetChangedAttributeActions()[0];
            AssertEquals("olivier@neodatis.org", cnaa.GetNoiWithNewValue().GetObject());

            odb.Close();
        }

        [Test]
        public virtual void TestIntrospectWithNull2()
        {
            var dbName = "TestIntrospectWithNull2.odb";
            DeleteBase(dbName);
            var odb = OdbFactory.Open(dbName);

            var user = new User("olivier smadja", "olivier@neodatis.com", null);
            IObjectInfoComparator comparator = new ObjectInfoComparator();
            var ci = ClassIntrospector.Introspect(user.GetType(), true).GetMainClassInfo();

            var storageEngine = ((OdbAdapter)odb).GetStorageEngine();

            var instanceInfo =
                (NonNativeObjectInfo)
                new ObjectIntrospector(storageEngine).GetMetaRepresentation(user, ci, true, null,
                                                                            new InstrumentationCallbackForStore(null,
                                                                                                                false));
            // Sets attributes offsets - this is normally done by reading then from
            // disk, but in this junit,
            // we must set them manually
            var offsets = new[] {1L, 2L, 3L};
            var ids = new[] {1, 2, 3};
            instanceInfo.GetHeader().SetAttributesIdentification(offsets);
            instanceInfo.GetHeader().SetAttributesIds(ids);
            instanceInfo.GetHeader().SetOid(OIDFactory.BuildObjectOID(1));
            var nnoiProfile = (NonNativeObjectInfo) instanceInfo.GetAttributeValueFromId(3);
            nnoiProfile.SetOid(OIDFactory.BuildObjectOID(2));

            user.SetProfile(new Profile("pname"));

            var instanceInfo3 =
                (NonNativeObjectInfo)
                new ObjectIntrospector(storageEngine).GetMetaRepresentation(user, ci, true, null,
                                                                            new InstrumentationCallbackForStore(null,
                                                                                                                false));
            instanceInfo3.GetHeader().SetAttributesIdentification(offsets);
            instanceInfo3.GetHeader().SetAttributesIds(ids);
            instanceInfo3.GetHeader().SetOid(OIDFactory.BuildObjectOID(1));
            nnoiProfile = (NonNativeObjectInfo) instanceInfo3.GetAttributeValueFromId(3);
            nnoiProfile.SetOid(OIDFactory.BuildObjectOID(2));

            AssertTrue(comparator.HasChanged(instanceInfo, instanceInfo3));
            AssertEquals(1, comparator.GetNbChanges());
            AssertEquals(0, comparator.GetChangedAttributeActions().Count);
            AssertEquals(1, comparator.GetNewObjectMetaRepresentations().Count);

            odb.Close();
        }

        [Test]
        public virtual void TestCopy()
        {
            var dbName = "introspectortest2.odb";
            DeleteBase(dbName);
            var odb = OdbFactory.Open(dbName);

            var function = new VO.Login.Function("login");
            var profile = new Profile("operator", function);
            var user = new User("olivier smadja", "olivier@neodatis.org", profile);
            var ci = ClassIntrospector.Introspect(user.GetType(), true).GetMainClassInfo();

            var storageEngine = ((OdbAdapter)odb).GetStorageEngine();

            var instanceInfo =
                (NonNativeObjectInfo)
                new ObjectIntrospector(storageEngine).GetMetaRepresentation(user, ci, true, null,
                                                                            new InstrumentationCallbackForStore(null,
                                                                                                                false));
            var copy = (NonNativeObjectInfo) instanceInfo.CreateCopy(new OdbHashMap<OID, AbstractObjectInfo>(), true);
            AssertEquals(3, copy.GetAttributeValues().Length);
            var aois = copy.GetAttributeValues();
            for (var i = 0; i < aois.Length; i++)
            {
                var aoi = aois[i];
                AssertEquals(instanceInfo.GetAttributeValues()[i].GetOdbTypeId(), aoi.GetOdbTypeId());
            }

            odb.Close();
        }
    }
}
