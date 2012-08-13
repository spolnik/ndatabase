using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using IO;
using NDatabase.Odb;
using NDatabase.Odb.Core.Layers.Layer1.Introspector;
using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Odb.Core.Layers.Layer2.Meta.Compare;
using NDatabase.Odb.Core.Oid;
using NDatabase.Odb.Impl.Core.Layers.Layer1.Introspector;
using NDatabase.Odb.Impl.Core.Layers.Layer2.Meta.Compare;
using NDatabase.Tool.Wrappers.List;
using NUnit.Framework;
using Test.Odb.Test;
using Test.Odb.Test.VO.Inheritance;
using Test.Odb.Test.VO.Login;

namespace Intropector
{
    public class InstrospectorTest : ODBTest
    {
        internal static IClassIntrospector classIntrospector = OdbConfiguration.GetCoreProvider().GetClassIntrospector();

        /// <exception cref="System.Exception"></exception>
        public override void SetUp()
        {
            base.SetUp();
            OdbConfiguration.GetCoreProvider().GetStorageEngine(new MockBaseIdentification()).AddSession(
                new MockSession("test"), false);
        }

        [Test]
        public virtual void TestClassInfo()
        {
            var user = new User("olivier smadja", "olivier@neodatis.com", new Profile("operator", new Function("login")));
            var classInfoList = classIntrospector.Introspect(user.GetType(), true);
            AssertEquals(user.GetType().FullName, classInfoList.GetMainClassInfo().GetFullClassName());
            AssertEquals(3, classInfoList.GetMainClassInfo().GetAttributes().Count);
            AssertEquals(2, classInfoList.GetClassInfos().Count);
        }

        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void TestInstanceInfo()
        {
            var user = new User("olivier smadja", "olivier@neodatis.com", new Profile("operator", new Function("login")));
            var ci = classIntrospector.Introspect(user.GetType(), true).GetMainClassInfo();
            var instanceInfo =
                (NonNativeObjectInfo)
                new LocalObjectIntrospector(new MockStorageEngine()).GetMetaRepresentation(user, ci, true, null,
                                                                                           new DefaultInstrumentationCallbackForStore
                                                                                               (null, null, false));
            AssertEquals(user.GetType().FullName, instanceInfo.GetClassInfo().GetFullClassName());
            AssertEquals("olivier smadja", instanceInfo.GetAttributeValueFromId(ci.GetAttributeId("name")).ToString());
            AssertEquals(typeof (AtomicNativeObjectInfo),
                         instanceInfo.GetAttributeValueFromId(ci.GetAttributeId("name")).GetType());
        }

        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void TestInstanceInfo2()
        {
            var user = new User("olivier smadja", "olivier@neodatis.com", new Profile("operator", new Function("login")));
            var ci = classIntrospector.Introspect(user.GetType(), true).GetMainClassInfo();
            var instanceInfo =
                (NonNativeObjectInfo)
                new LocalObjectIntrospector(new MockStorageEngine()).GetMetaRepresentation(user, ci, true, null,
                                                                                           new DefaultInstrumentationCallbackForStore
                                                                                               (null, null, false));
            AssertEquals(instanceInfo.GetClassInfo().GetFullClassName(), user.GetType().FullName);
            AssertEquals(instanceInfo.GetAttributeValueFromId(ci.GetAttributeId("name")).ToString(), "olivier smadja");
        }

        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void TestCompareCollection1()
        {
            var user = new User("olivier smadja", "olivier@neodatis.com", new Profile("operator", new Function("login")));
            IObjectInfoComparator comparator = new ObjectInfoComparator();
            var ci = classIntrospector.Introspect(user.GetType(), true).GetMainClassInfo();
            var instanceInfo =
                (NonNativeObjectInfo)
                new LocalObjectIntrospector(new MockStorageEngine()).GetMetaRepresentation(user, ci, true, null,
                                                                                           new DefaultInstrumentationCallbackForStore
                                                                                               (null, null, false));
            // Sets attributes offsets - this is normally done by reading then from
            // disk, but in this junit,
            // we must set them manually
            var offsets = new[] {1L, 2L, 3L};
            var ids = new[] {1, 2, 3};
            instanceInfo.GetHeader().SetAttributesIdentification(offsets);
            instanceInfo.GetHeader().SetAttributesIds(ids);
            instanceInfo.GetHeader().SetOid(OIDFactory.BuildObjectOID(1));
            var nnoiProfile = (NonNativeObjectInfo) instanceInfo.GetAttributeValueFromId(2);
            nnoiProfile.SetOid(OIDFactory.BuildObjectOID(2));
            user.SetName("Olivier Smadja");
            var instanceInfo3 =
                (NonNativeObjectInfo)
                new LocalObjectIntrospector(new MockStorageEngine()).GetMetaRepresentation(user, ci, true, null,
                                                                                           new DefaultInstrumentationCallbackForStore
                                                                                               (null, null, false));
            instanceInfo3.GetHeader().SetOid(OIDFactory.BuildObjectOID(1));
            nnoiProfile = (NonNativeObjectInfo) instanceInfo3.GetAttributeValueFromId(2);
            nnoiProfile.SetOid(OIDFactory.BuildObjectOID(2));
            AssertTrue(comparator.HasChanged(instanceInfo, instanceInfo3));
            AssertEquals(1, comparator.GetNbChanges());
            AssertEquals(1, comparator.GetChangedAttributeActions().Count);
            var cnaa = (ChangedNativeAttributeAction) comparator.GetChangedAttributeActions()[0];
            AssertEquals("Olivier Smadja", cnaa.GetNoiWithNewValue().GetObject());
        }

        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void TestCompareCollection11()
        {
            var user = new User("olivier smadja", "olivier@neodatis.com", new Profile("operator", new Function("login")));
            IObjectInfoComparator comparator = new ObjectInfoComparator();
            var ci = classIntrospector.Introspect(user.GetType(), true).GetMainClassInfo();
            var instanceInfo =
                (NonNativeObjectInfo)
                new LocalObjectIntrospector(new MockStorageEngine()).GetMetaRepresentation(user, ci, true, null,
                                                                                           new DefaultInstrumentationCallbackForStore
                                                                                               (null, null, false));
            // Sets attributes offsets - this is normally done by reading then from
            // disk, but in this junit,
            // we must set them manually
            var offsets = new[] {1L, 2L, 3L};
            var ids = new[] {1, 2, 3};
            instanceInfo.GetHeader().SetAttributesIdentification(offsets);
            instanceInfo.GetHeader().SetAttributesIds(ids);
            instanceInfo.GetHeader().SetOid(OIDFactory.BuildObjectOID(1));
            object o = instanceInfo.GetAttributeValueFromId(2);
            var nnoiProfile = (NonNativeObjectInfo) o;
            nnoiProfile.SetOid(OIDFactory.BuildObjectOID(2));
            user.SetName("Olivier Smadja");
            user.SetEmail("olivier@neodatis.org");
            var instanceInfo3 =
                (NonNativeObjectInfo)
                new LocalObjectIntrospector(new MockStorageEngine()).GetMetaRepresentation(user, ci, true, null,
                                                                                           new DefaultInstrumentationCallbackForStore
                                                                                               (null, null, false));
            instanceInfo3.GetHeader().SetOid(OIDFactory.BuildObjectOID(1));
            nnoiProfile = (NonNativeObjectInfo) instanceInfo3.GetAttributeValueFromId(2);
            nnoiProfile.SetOid(OIDFactory.BuildObjectOID(2));
            AssertTrue(comparator.HasChanged(instanceInfo, instanceInfo3));
            AssertEquals(2, comparator.GetNbChanges());
            AssertEquals(2, comparator.GetChangedAttributeActions().Count);
            var cnaa = (ChangedNativeAttributeAction) comparator.GetChangedAttributeActions()[0];
            AssertEquals("Olivier Smadja", cnaa.GetNoiWithNewValue().GetObject());
            cnaa = (ChangedNativeAttributeAction) comparator.GetChangedAttributeActions()[1];
            AssertEquals("olivier@neodatis.org", cnaa.GetNoiWithNewValue().GetObject());
        }

        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void TestCompareCollection2()
        {
            var user = new User("olivier smadja", "olivier@neodatis.com", new Profile("operator", new Function("login")));
            IObjectInfoComparator comparator = new ObjectInfoComparator();
            var ci = classIntrospector.Introspect(user.GetType(), true).GetMainClassInfo();
            var instanceInfo =
                (NonNativeObjectInfo)
                new LocalObjectIntrospector(new MockStorageEngine()).GetMetaRepresentation(user, ci, true, null,
                                                                                           new DefaultInstrumentationCallbackForStore
                                                                                               (null, null, false));
            // Sets attributes offsets - this is normally done by reading them from
            // disk, but in this junit,
            // we must set them manually
            var offsets = new[] {1L, 2L, 3L};
            var ids = new[] {1, 2, 3};
            instanceInfo.GetHeader().SetAttributesIdentification(offsets);
            instanceInfo.GetHeader().SetAttributesIds(ids);
            instanceInfo.GetHeader().SetOid(OIDFactory.BuildObjectOID(1));
            var nnoiProfile = (NonNativeObjectInfo) instanceInfo.GetAttributeValueFromId(2);
            nnoiProfile.SetOid(OIDFactory.BuildObjectOID(2));
            user.SetName(null);
            var instanceInfo3 =
                (NonNativeObjectInfo)
                new LocalObjectIntrospector(new MockStorageEngine()).GetMetaRepresentation(user, ci, true, null,
                                                                                           new DefaultInstrumentationCallbackForStore
                                                                                               (null, null, false));
            instanceInfo3.GetHeader().SetOid(OIDFactory.BuildObjectOID(1));
            nnoiProfile = (NonNativeObjectInfo) instanceInfo3.GetAttributeValueFromId(2);
            nnoiProfile.SetOid(OIDFactory.BuildObjectOID(2));
            AssertTrue(comparator.HasChanged(instanceInfo, instanceInfo3));
            AssertEquals(1, comparator.GetNbChanges());
            AssertEquals(1, comparator.GetChangedAttributeActions().Count);
            var cnaa = (ChangedNativeAttributeAction) comparator.GetChangedAttributeActions()[0];
            AssertEquals(null, cnaa.GetNoiWithNewValue().GetObject());
        }

        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void TestCompareCollection3CollectionContentChange()
        {
            var function = new Function("login");
            var user = new User("olivier smadja", "olivier@neodatis.com", new Profile("operator", function));
            IObjectInfoComparator comparator = new ObjectInfoComparator();
            var ci = classIntrospector.Introspect(user.GetType(), true).GetMainClassInfo();
            var instanceInfo =
                (NonNativeObjectInfo)
                new LocalObjectIntrospector(new MockStorageEngine()).GetMetaRepresentation(user, ci, true, null,
                                                                                           new DefaultInstrumentationCallbackForStore
                                                                                               (null, null, false));
            // Sets attributes offsets - this is normally done by reading then from
            // disk, but in this junit,
            // we must set them manually
            var offsets = new[] {1L, 2L, 3L};
            var ids = new[] {1, 2, 3};
            instanceInfo.GetHeader().SetAttributesIdentification(offsets);
            instanceInfo.GetHeader().SetAttributesIds(ids);
            instanceInfo.GetHeader().SetOid(OIDFactory.BuildObjectOID(1));
            var nnoiProfile = (NonNativeObjectInfo) instanceInfo.GetAttributeValueFromId(2);
            nnoiProfile.SetOid(OIDFactory.BuildObjectOID(2));
            var nnoi = (NonNativeObjectInfo) instanceInfo.GetAttributeValueFromId(ci.GetAttributeId("profile"));
            nnoi.GetHeader().SetAttributesIdentification(offsets);
            nnoi.GetHeader().SetAttributesIds(ids);
            nnoi.SetOid(OIDFactory.BuildObjectOID(2));
            var nnoi2 =
                (CollectionObjectInfo) nnoi.GetAttributeValueFromId(nnoi.GetClassInfo().GetAttributeId("functions"));
            var nnoi3 = (NonNativeObjectInfo) nnoi2.GetCollection().GetEnumerator().Current;
            nnoi3.GetHeader().SetAttributesIdentification(offsets);
            nnoi3.GetHeader().SetAttributesIds(ids);
            function.SetName("login function");
            var instanceInfo3 =
                (NonNativeObjectInfo)
                new LocalObjectIntrospector(new MockStorageEngine()).GetMetaRepresentation(user, ci, true, null,
                                                                                           new DefaultInstrumentationCallbackForStore
                                                                                               (null, null, false));
            instanceInfo3.GetHeader().SetOid(OIDFactory.BuildObjectOID(1));
            nnoiProfile = (NonNativeObjectInfo) instanceInfo3.GetAttributeValueFromId(2);
            nnoiProfile.SetOid(OIDFactory.BuildObjectOID(2));
            AssertTrue(comparator.HasChanged(instanceInfo, instanceInfo3));
            AssertEquals(1, comparator.GetNbChanges());
            var cnaa = (ChangedNativeAttributeAction) comparator.GetChangedAttributeActions()[0];
            AssertEquals(1, comparator.GetChangedAttributeActions().Count);
            AssertEquals(function.GetName(), cnaa.GetNoiWithNewValue().GetObject());
        }

        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void TestCompareCollection4CollectionContentChange()
        {
            if (!testNewFeature)
                return;
            var function = new Function("login");
            var user = new User("olivier smadja", "olivier@neodatis.com", new Profile("operator", function));
            IObjectInfoComparator comparator = new ObjectInfoComparator();
            var ci = classIntrospector.Introspect(user.GetType(), true).GetMainClassInfo();
            var instanceInfo =
                (NonNativeObjectInfo)
                new LocalObjectIntrospector(new MockStorageEngine()).GetMetaRepresentation(user, ci, true, null,
                                                                                           new DefaultInstrumentationCallbackForStore
                                                                                               (null, null, false));
            // Sets attributes offsets - this is normally done by reading then from
            // disk, but in this junit,
            // we must set them manually
            var offsets = new[] {1L, 2L, 3L};
            var ids = new[] {1, 2, 3};
            instanceInfo.GetHeader().SetAttributesIdentification(offsets);
            instanceInfo.GetHeader().SetAttributesIds(ids);
            instanceInfo.GetHeader().SetOid(OIDFactory.BuildObjectOID(1));
            var nnoiProfile = (NonNativeObjectInfo) instanceInfo.GetAttributeValueFromId(2);
            nnoiProfile.SetOid(OIDFactory.BuildObjectOID(2));
            function.SetName(null);
            var instanceInfo3 =
                (NonNativeObjectInfo)
                new LocalObjectIntrospector(new MockStorageEngine()).GetMetaRepresentation(user, ci, true, null,
                                                                                           new DefaultInstrumentationCallbackForStore
                                                                                               (null, null, false));
            instanceInfo3.GetHeader().SetOid(OIDFactory.BuildObjectOID(1));
            nnoiProfile = (NonNativeObjectInfo) instanceInfo3.GetAttributeValueFromId(2);
            nnoiProfile.SetOid(OIDFactory.BuildObjectOID(2));
            AssertTrue(comparator.HasChanged(instanceInfo, instanceInfo3));
            AssertEquals(1, comparator.GetNbChanges());
            var cnaa = (ChangedNativeAttributeAction) comparator.GetChangedAttributeActions()[0];
            AssertEquals(1, comparator.GetChangedAttributeActions().Count);
            AssertEquals(function.GetName(), cnaa.GetNoiWithNewValue().GetObject());
        }

        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void TestCompareCollection5()
        {
            var function = new Function("login");
            var profile = new Profile("operator", function);
            var user = new User("olivier smadja", "olivier@neodatis.com", profile);
            IObjectInfoComparator comparator = new ObjectInfoComparator();
            var ci = classIntrospector.Introspect(user.GetType(), true).GetMainClassInfo();
            var instanceInfo =
                (NonNativeObjectInfo)
                new LocalObjectIntrospector(new MockStorageEngine()).GetMetaRepresentation(user, ci, true, null,
                                                                                           new DefaultInstrumentationCallbackForStore
                                                                                               (null, null, false));
            // Sets attributes offsets - this is normally done by reading then from
            // disk, but in this junit,
            // we must set them manually
            var offsets = new[] {1L, 2L, 3L};
            var ids = new[] {1, 2, 3};
            instanceInfo.GetHeader().SetAttributesIdentification(offsets);
            instanceInfo.GetHeader().SetAttributesIds(ids);
            instanceInfo.GetHeader().SetOid(OIDFactory.BuildObjectOID(1));
            var nnoiProfile = (NonNativeObjectInfo) instanceInfo.GetAttributeValueFromId(2);
            nnoiProfile.SetOid(OIDFactory.BuildObjectOID(2));
            profile.GetFunctions().Add(new Function("logout"));
            var instanceInfo3 =
                (NonNativeObjectInfo)
                new LocalObjectIntrospector(new MockStorageEngine()).GetMetaRepresentation(user, ci, true, null,
                                                                                           new DefaultInstrumentationCallbackForStore
                                                                                               (null, null, false));
            instanceInfo3.GetHeader().SetOid(OIDFactory.BuildObjectOID(1));
            nnoiProfile = (NonNativeObjectInfo) instanceInfo3.GetAttributeValueFromId(2);
            nnoiProfile.SetOid(OIDFactory.BuildObjectOID(2));
            AssertTrue(comparator.HasChanged(instanceInfo, instanceInfo3));
            AssertEquals(1, comparator.GetNbChanges());
            var nnoi = (NonNativeObjectInfo) comparator.GetChangedObjectMetaRepresentation(0);
            AssertEquals(2, ((IList) nnoi.GetValueOf("functions")).Count);
        }

        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void TestCompareCollection6()
        {
            var function = new Function("login");
            var profile = new Profile("operator", function);
            var user = new User("olivier smadja", "olivier@neodatis.com", profile);
            IObjectInfoComparator comparator = new ObjectInfoComparator();
            var ci = classIntrospector.Introspect(user.GetType(), true).GetMainClassInfo();
            var instanceInfo =
                (NonNativeObjectInfo)
                new LocalObjectIntrospector(new MockStorageEngine()).GetMetaRepresentation(user, ci, true, null,
                                                                                           new DefaultInstrumentationCallbackForStore
                                                                                               (null, null, false));
            // Sets attributes offsets - this is normally done by reading then from
            // disk, but in this junit,
            // we must set them manually
            var offsets = new[] {1L, 2L, 3L};
            var ids = new[] {1, 2, 3};
            instanceInfo.GetHeader().SetAttributesIdentification(offsets);
            instanceInfo.GetHeader().SetAttributesIds(ids);
            instanceInfo.GetHeader().SetOid(OIDFactory.BuildObjectOID(1));
            var nnoiProfile = (NonNativeObjectInfo) instanceInfo.GetAttributeValueFromId(2);
            nnoiProfile.SetOid(OIDFactory.BuildObjectOID(2));
            var nnoi = (NonNativeObjectInfo) instanceInfo.GetAttributeValueFromId(ci.GetAttributeId("profile"));
            nnoi.GetHeader().SetAttributesIdentification(offsets);
            nnoi.GetHeader().SetAttributesIds(ids);
            profile.SetName("ope");
            var instanceInfo3 =
                (NonNativeObjectInfo)
                new LocalObjectIntrospector(new MockStorageEngine()).GetMetaRepresentation(user, ci, true, null,
                                                                                           new DefaultInstrumentationCallbackForStore
                                                                                               (null, null, false));
            instanceInfo3.GetHeader().SetOid(OIDFactory.BuildObjectOID(1));
            nnoiProfile = (NonNativeObjectInfo) instanceInfo3.GetAttributeValueFromId(2);
            nnoiProfile.SetOid(OIDFactory.BuildObjectOID(2));
            AssertTrue(comparator.HasChanged(instanceInfo, instanceInfo3));
            AssertEquals(1, comparator.GetNbChanges());
            var cnaa = (ChangedNativeAttributeAction) comparator.GetChangedAttributeActions()[0];
            AssertEquals(1, comparator.GetChangedAttributeActions().Count);
            AssertEquals(profile.GetName(), cnaa.GetNoiWithNewValue().GetObject());
        }

        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void TestCompareCollection7()
        {
            var function = new Function("login");
            var profile = new Profile("operator", function);
            var user = new User("olivier smadja", "olivier@neodatis.com", profile);
            IObjectInfoComparator comparator = new ObjectInfoComparator();
            var ci = classIntrospector.Introspect(user.GetType(), true).GetMainClassInfo();
            var instanceInfo =
                (NonNativeObjectInfo)
                new LocalObjectIntrospector(new MockStorageEngine()).GetMetaRepresentation(user, ci, true, null,
                                                                                           new DefaultInstrumentationCallbackForStore
                                                                                               (null, null, false));
            // Sets attributes offsets - this is normally done by reading then from
            // disk, but in this junit,
            // we must set them manually
            var offsets = new[] {1L, 2L, 3L};
            var ids = new[] {1, 2, 3};
            instanceInfo.GetHeader().SetAttributesIdentification(offsets);
            instanceInfo.GetHeader().SetAttributesIds(ids);
            instanceInfo.GetHeader().SetOid(OIDFactory.BuildObjectOID(1));
            var nnoiProfile = (NonNativeObjectInfo) instanceInfo.GetAttributeValueFromId(2);
            nnoiProfile.SetOid(OIDFactory.BuildObjectOID(2));
            // / Set the same name
            profile.SetName("operator");
            var instanceInfo3 =
                (NonNativeObjectInfo)
                new LocalObjectIntrospector(new MockStorageEngine()).GetMetaRepresentation(user, ci, true, null,
                                                                                           new DefaultInstrumentationCallbackForStore
                                                                                               (null, null, false));
            instanceInfo3.GetHeader().SetOid(OIDFactory.BuildObjectOID(1));
            nnoiProfile = (NonNativeObjectInfo) instanceInfo3.GetAttributeValueFromId(2);
            nnoiProfile.SetOid(OIDFactory.BuildObjectOID(2));
            AssertFalse(comparator.HasChanged(instanceInfo, instanceInfo3));
            AssertEquals(0, comparator.GetNbChanges());
        }

        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void TestCompareCollection8()
        {
            var function = new Function("login");
            var profile = new Profile("operator", function);
            var user = new User("olivier smadja", "olivier@neodatis.com", profile);
            IObjectInfoComparator comparator = new ObjectInfoComparator();
            var ci = classIntrospector.Introspect(user.GetType(), true).GetMainClassInfo();
            var instanceInfo =
                (NonNativeObjectInfo)
                new LocalObjectIntrospector(new MockStorageEngine()).GetMetaRepresentation(user, ci, true, null,
                                                                                           new DefaultInstrumentationCallbackForStore
                                                                                               (null, null, false));
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
                new LocalObjectIntrospector(new MockStorageEngine()).GetMetaRepresentation(user, ci, true, null,
                                                                                           new DefaultInstrumentationCallbackForStore
                                                                                               (null, null, false));
            AssertTrue(comparator.HasChanged(instanceInfo, instanceInfo3));
            AssertEquals(1, comparator.GetNbChanges());
            AssertEquals(1, comparator.GetAttributeToSetToNull().Count);
            var o = (SetAttributeToNullAction) comparator.GetAttributeToSetToNull()[0];
            AssertEquals(0, comparator.GetChangedAttributeActions().Count);
            AssertEquals(2, o.GetAttributeId());
        }

        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void TestCompareCollection9()
        {
            var function = new Function("login");
            var profile = new Profile("operator", function);
            var user = new User("olivier smadja", "olivier@neodatis.com", profile);
            IObjectInfoComparator comparator = new ObjectInfoComparator();
            var ci = classIntrospector.Introspect(user.GetType(), true).GetMainClassInfo();
            var instanceInfo =
                (NonNativeObjectInfo)
                new LocalObjectIntrospector(new MockStorageEngine()).GetMetaRepresentation(user, ci, true, null,
                                                                                           new DefaultInstrumentationCallbackForStore
                                                                                               (null, null, false));
            // Sets attributes offsets - this is normally done by reading then from
            // disk, but in this junit,
            // we must set them manually
            var offsets = new[] {1L, 2L, 3L};
            var ids = new[] {1, 2, 3};
            instanceInfo.GetHeader().SetAttributesIdentification(offsets);
            instanceInfo.GetHeader().SetAttributesIds(ids);
            instanceInfo.GetHeader().SetOid(OIDFactory.BuildObjectOID(1));
            var nnoiProfile = (NonNativeObjectInfo) instanceInfo.GetAttributeValueFromId(2);
            nnoiProfile.SetOid(OIDFactory.BuildObjectOID(2));
            user.SetName("Kiko");
            var instanceInfo3 =
                (NonNativeObjectInfo)
                new LocalObjectIntrospector(new MockStorageEngine()).GetMetaRepresentation(user, ci, true, null,
                                                                                           new DefaultInstrumentationCallbackForStore
                                                                                               (null, null, false));
            instanceInfo3.GetHeader().SetOid(OIDFactory.BuildObjectOID(1));
            nnoiProfile = (NonNativeObjectInfo) instanceInfo3.GetAttributeValueFromId(2);
            nnoiProfile.SetOid(OIDFactory.BuildObjectOID(2));
            AssertTrue(comparator.HasChanged(instanceInfo, instanceInfo3));
            AssertEquals(1, comparator.GetNbChanges());
            var cnaa = (ChangedNativeAttributeAction) comparator.GetChangedAttributeActions()[0];
            AssertEquals(1, comparator.GetChangedAttributeActions().Count);
            AssertEquals(user.GetName(), cnaa.GetNoiWithNewValue().GetObject());
        }

        [Test]
        public virtual void TestGetSuperClasses()
        {
            var superclasses = classIntrospector.GetSuperClasses(typeof (FootballPlayer).FullName, true);
            AssertEquals(3, superclasses.Count);
            AssertEquals(typeof (FootballPlayer), superclasses[0]);
            AssertEquals(typeof (OutdoorPlayer), superclasses[1]);
            AssertEquals(typeof (Player), superclasses[2]);
        }

        [Test]
        public virtual void TestGetAllFields()
        {
            IOdbList<FieldInfo> allFields = classIntrospector.GetAllFields(typeof (FootballPlayer).FullName);
            AssertEquals(3, allFields.Count);
            AssertEquals("role", ((FieldInfo) allFields[0]).Name);
            AssertEquals("groundName", ((FieldInfo) allFields[1]).Name);
            AssertEquals("name", ((FieldInfo) allFields[2]).Name);
        }

        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void TestIntrospectWithNull()
        {
            var user = new User("olivier smadja", "olivier@neodatis.com", null);
            IObjectInfoComparator comparator = new ObjectInfoComparator();
            var ci = classIntrospector.Introspect(user.GetType(), true).GetMainClassInfo();
            var instanceInfo =
                (NonNativeObjectInfo)
                new LocalObjectIntrospector(new MockStorageEngine()).GetMetaRepresentation(user, ci, true, null,
                                                                                           new DefaultInstrumentationCallbackForStore
                                                                                               (null, null, false));
            // Sets attributes offsets - this is normally done by reading then from
            // disk, but in this junit,
            // we must set them manually
            var offsets = new[] {1L, 2L, 3L};
            var ids = new[] {1, 2, 3};
            instanceInfo.GetHeader().SetAttributesIdentification(offsets);
            instanceInfo.GetHeader().SetAttributesIds(ids);
            instanceInfo.GetHeader().SetOid(OIDFactory.BuildObjectOID(1));
            object o = instanceInfo.GetAttributeValueFromId(2);
            var nnoiProfile = (NonNativeObjectInfo) o;
            nnoiProfile.SetOid(OIDFactory.BuildObjectOID(2));
            user.SetName("Olivier Smadja");
            user.SetEmail("olivier@neodatis.org");
            user.SetProfile(new Profile("pname"));
            var instanceInfo3 =
                (NonNativeObjectInfo)
                new LocalObjectIntrospector(new MockStorageEngine()).GetMetaRepresentation(user, ci, true, null,
                                                                                           new DefaultInstrumentationCallbackForStore
                                                                                               (null, null, false));
            instanceInfo3.GetHeader().SetOid(OIDFactory.BuildObjectOID(1));
            nnoiProfile = (NonNativeObjectInfo) instanceInfo3.GetAttributeValueFromId(2);
            nnoiProfile.SetOid(OIDFactory.BuildObjectOID(2));
            AssertTrue(comparator.HasChanged(instanceInfo, instanceInfo3));
            AssertEquals(3, comparator.GetNbChanges());
            AssertEquals(2, comparator.GetChangedAttributeActions().Count);
            var cnaa = (ChangedNativeAttributeAction) comparator.GetChangedAttributeActions()[0];
            AssertEquals("Olivier Smadja", cnaa.GetNoiWithNewValue().GetObject());
            cnaa = (ChangedNativeAttributeAction) comparator.GetChangedAttributeActions()[1];
            AssertEquals("olivier@neodatis.org", cnaa.GetNoiWithNewValue().GetObject());
        }

        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void TestIntrospectWithNull2()
        {
            var user = new User("olivier smadja", "olivier@neodatis.com", null);
            IObjectInfoComparator comparator = new ObjectInfoComparator();
            var ci = classIntrospector.Introspect(user.GetType(), true).GetMainClassInfo();
            var instanceInfo =
                (NonNativeObjectInfo)
                new LocalObjectIntrospector(new MockStorageEngine()).GetMetaRepresentation(user, ci, true, null,
                                                                                           new DefaultInstrumentationCallbackForStore
                                                                                               (null, null, false));
            // Sets attributes offsets - this is normally done by reading then from
            // disk, but in this junit,
            // we must set them manually
            var offsets = new[] {1L, 2L, 3L};
            var ids = new[] {1, 2, 3};
            instanceInfo.GetHeader().SetAttributesIdentification(offsets);
            instanceInfo.GetHeader().SetAttributesIds(ids);
            instanceInfo.GetHeader().SetOid(OIDFactory.BuildObjectOID(1));
            object o = instanceInfo.GetAttributeValueFromId(2);
            var nnoiProfile = (NonNativeObjectInfo) o;
            nnoiProfile.SetOid(OIDFactory.BuildObjectOID(2));
            user.SetProfile(new Profile("pname"));
            var instanceInfo3 =
                (NonNativeObjectInfo)
                new LocalObjectIntrospector(new MockStorageEngine()).GetMetaRepresentation(user, ci, true, null,
                                                                                           new DefaultInstrumentationCallbackForStore
                                                                                               (null, null, false));
            instanceInfo3.GetHeader().SetOid(OIDFactory.BuildObjectOID(1));
            nnoiProfile = (NonNativeObjectInfo) instanceInfo3.GetAttributeValueFromId(2);
            nnoiProfile.SetOid(OIDFactory.BuildObjectOID(2));
            bool b = comparator.HasChanged(instanceInfo, instanceInfo3);
            AssertTrue(b);
            AssertEquals(1, comparator.GetNbChanges());
            AssertEquals(0, comparator.GetChangedAttributeActions().Count);
            AssertEquals(1, comparator.GetNewObjectMetaRepresentations().Count);
        }

        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void TestGetDependentObjects()
        {
            var user = new User("olivier smadja", "olivier@neodatis.com", new Profile("operator", new Function("login")));
            var callback = new GetDependentObjectIntrospectingCallback();
            var ci = classIntrospector.Introspect(user.GetType(), true).GetMainClassInfo();
            var instanceInfo =
                (NonNativeObjectInfo)
                new LocalObjectIntrospector(new MockStorageEngine()).GetMetaRepresentation(user, ci, true, null,
                                                                                           callback);
            AssertEquals(user.GetType().FullName, instanceInfo.GetClassInfo().GetFullClassName());
            AssertEquals("olivier smadja", instanceInfo.GetAttributeValueFromId(ci.GetAttributeId("name")).ToString());
            AssertEquals(typeof (AtomicNativeObjectInfo),
                         instanceInfo.GetAttributeValueFromId(ci.GetAttributeId("name")).GetType());
            var objects = callback.GetObjects();
            AssertEquals(2, objects.Count);
            AssertTrue(objects.Contains(user.GetProfile()));
            AssertTrue(objects.Contains(user.GetProfile().GetFunctions()[0]));
        }

        /// <exception cref="System.Exception"></exception>
        [Test]
        public virtual void TestCopy()
        {
            var function = new Function("login");
            var profile = new Profile("operator", function);
            var user = new User("olivier smadja", "olivier@neodatis.org", profile);
            var ci = classIntrospector.Introspect(user.GetType(), true).GetMainClassInfo();
            var instanceInfo =
                (NonNativeObjectInfo)
                new LocalObjectIntrospector(new MockStorageEngine()).GetMetaRepresentation(user, ci, true, null,
                                                                                           new DefaultInstrumentationCallbackForStore
                                                                                               (null, null, false));
            var copy = (NonNativeObjectInfo) instanceInfo.CreateCopy(new Dictionary<OID, AbstractObjectInfo>(), true);
            AssertEquals(3, copy.GetAttributeValues().Length);
            var aois = copy.GetAttributeValues();
            for (var i = 0; i < aois.Length; i++)
            {
                var aoi = aois[i];
                AssertEquals(instanceInfo.GetAttributeValues()[i].GetOdbTypeId(), aoi.GetOdbTypeId());
            }
        }
    }
}
