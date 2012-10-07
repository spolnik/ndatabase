using System;
using System.Collections;
using System.Globalization;
using System.Runtime.Serialization;
using NDatabase.Odb.Core.Layers.Layer1.Introspector;
using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Odb.Core.Layers.Layer3;
using NDatabase.Odb.Core.Oid;
using NDatabase.Odb.Core.Transaction;
using NDatabase.Odb.Core.Trigger;
using NDatabase.Tool;
using NDatabase.Tool.Wrappers;

namespace NDatabase.Odb.Core.Layers.Layer2.Instance
{
    /// <summary>
    ///   Class used to build instance from Meta Object representation.
    /// </summary>
    /// <remarks>
    ///   Class used to build instance from Meta Object representation. Layer 2 to Layer 1 conversion.
    /// </remarks>
    internal sealed class InstanceBuilder : IInstanceBuilder
    {
        private const string LogIdDebug = "InstanceBuilder.debug";

        private readonly IStorageEngine _engine;
        private readonly ITriggerManager _triggerManager;

        public InstanceBuilder(IStorageEngine engine)
        {
            _triggerManager = engine.GetLocalTriggerManager();
            _engine = engine;
        }

        #region IInstanceBuilder Members

        public object BuildOneInstance(NonNativeObjectInfo objectInfo)
        {
            return BuildOneInstance(objectInfo, _engine.GetSession(true).GetCache());
        }

        public object BuildOneInstance(NonNativeObjectInfo objectInfo, IOdbCache cache)
        {
            // verify if the object is check to delete
            if (objectInfo.IsDeletedObject())
                throw new OdbRuntimeException(
                    NDatabaseError.ObjectIsMarkedAsDeletedForOid.AddParameter(objectInfo.GetOid()));

            // Then check if object is in cache
            var o = cache.GetObject(objectInfo.GetOid());
            if (o != null)
                return o;

            var instanceClazz = OdbClassPool.GetClass(objectInfo.GetClassInfo().FullClassName);

            try
            {
                o = FormatterServices.GetUninitializedObject(instanceClazz);
            }
            catch (Exception e)
            {
                throw new OdbRuntimeException(
                    NDatabaseError.InstanciationError.AddParameter(objectInfo.GetClassInfo().FullClassName), e);
            }

            // This can happen if ODB can not create the instance from security reasons
            if (o == null)
                throw new OdbRuntimeException(
                    NDatabaseError.InstanciationError.AddParameter(objectInfo.GetClassInfo().FullClassName));

            // Keep the initial hash code. In some cases, when the class redefines
            // the hash code method
            // Hash code can return wrong values when attributes are not set (when
            // hash code depends on attribute values)
            // Hash codes are used as the key of the map,
            // So at the end of this method, if hash codes are different, object
            // will be removed from the cache and inserted back
            var hashCodeIsOk = true;
            var initialHashCode = 0;

            try
            {
                initialHashCode = o.GetHashCode();
            }
            catch (Exception)
            {
                hashCodeIsOk = false;
            }

            // Adds this incomplete instance in the cache to manage cyclic reference
            if (hashCodeIsOk)
                cache.AddObject(objectInfo.GetOid(), o, objectInfo.GetHeader());

            var classInfo = objectInfo.GetClassInfo();
            var fields = ClassIntrospector.GetAllFieldsFrom(classInfo.UnderlyingType);

            object value = null;

            foreach (var fieldInfo in fields)
            {
                // Gets the id of this field
                var attributeId = classInfo.GetAttributeId(fieldInfo.Name);
                if (OdbConfiguration.IsDebugEnabled(LogIdDebug))
                    DLogger.Debug(string.Concat("getting field with name ", fieldInfo.Name, ", attribute id is ", attributeId.ToString()));

                var abstractObjectInfo = objectInfo.GetAttributeValueFromId(attributeId);

                // Check consistency
                // ensureClassCompatibily(field,
                // instanceInfo.getClassInfo().getAttributeinfo(i).getFullClassname());
                if (abstractObjectInfo != null && (!abstractObjectInfo.IsNull()))
                {
                    if (abstractObjectInfo.IsNative())
                    {
                        if (abstractObjectInfo.IsAtomicNativeObject())
                        {
                            value = abstractObjectInfo.IsNull()
                                        ? null
                                        : abstractObjectInfo.GetObject();
                        }

                        if (abstractObjectInfo.IsCollectionObject())
                        {
                            value = BuildCollectionInstance((CollectionObjectInfo) abstractObjectInfo);
                            // Manage a specific case of Set
                            /*
							if (typeof(Java.Util.Set).IsAssignableFrom(field.GetType()) && typeof(ICollection).IsAssignableFrom(value.GetType()))
							{
								Java.Util.Set s = new Java.Util.HashSet();
								s.AddAll((System.Collections.ICollection)value);
								value = s;
							}*/
                        }
                        if (abstractObjectInfo.IsArrayObject())
                            value = BuildArrayInstance((ArrayObjectInfo) abstractObjectInfo);
                        if (abstractObjectInfo.IsMapObject())
                            value = BuildMapInstance((MapObjectInfo) abstractObjectInfo);
                        if (abstractObjectInfo.IsEnumObject())
                            value = BuildEnumInstance((EnumNativeObjectInfo) abstractObjectInfo, fieldInfo.FieldType);
                    }
                    else
                    {
                        if (abstractObjectInfo.IsNonNativeObject())
                        {
                            if (abstractObjectInfo.IsDeletedObject())
                            {
                                if (OdbConfiguration.DisplayWarnings())
                                {
                                    var warning =
                                        NDatabaseError.AttributeReferencesADeletedObject.AddParameter(
                                            objectInfo.GetClassInfo().FullClassName).AddParameter(
                                                objectInfo.GetOid()).AddParameter(fieldInfo.Name);
                                    DLogger.Info(warning.ToString());
                                }
                                value = null;
                            }
                            else
                                value = BuildOneInstance((NonNativeObjectInfo) abstractObjectInfo);
                        }
                    }
                    if (value != null)
                    {
                        if (OdbConfiguration.IsDebugEnabled(LogIdDebug))
                        {
                            DLogger.Debug(String.Format("Setting field {0}({1}) to {2} / {3}", fieldInfo.Name,
                                                        fieldInfo.GetType().FullName, value, value.GetType().FullName));
                        }
                        try
                        {
                            fieldInfo.SetValue(o, value);
                        }
                        catch (Exception e)
                        {
                            throw new OdbRuntimeException(
                                NDatabaseError.InstanceBuilderWrongObjectContainerType.AddParameter(
                                    objectInfo.GetClassInfo().FullClassName).AddParameter(value.GetType().FullName)
                                    .AddParameter(fieldInfo.GetType().FullName), e);
                        }
                    }
                }
            }
            if (!OdbClassUtil.GetFullName(o.GetType()).Equals(objectInfo.GetClassInfo().FullClassName))
            {
                throw new OdbRuntimeException(
                    NDatabaseError.InstanceBuilderWrongObjectType.AddParameter(
                        objectInfo.GetClassInfo().FullClassName).AddParameter(o.GetType().FullName));
            }
            if (hashCodeIsOk || initialHashCode != o.GetHashCode())
            {
                // Bug (sf bug id=1875544 )detected by glsender
                // This can happen when an object has redefined its own hashcode
                // method and depends on the field values
                // Then, we have to remove object from the cache and re-insert to
                // correct map hash code
                cache.RemoveObjectByOid(objectInfo.GetOid());
                // re-Adds instance in the cache
                cache.AddObject(objectInfo.GetOid(), o, objectInfo.GetHeader());
            }
            if (_triggerManager != null)
            {
                _triggerManager.ManageSelectTriggerAfter(objectInfo.GetClassInfo().UnderlyingType, objectInfo,
                                                         objectInfo.GetOid());
            }

            return o;
        }

        public string GetSessionId()
        {
            return _engine.GetSession(true).GetId();
        }

        #endregion

        public object BuildOneInstance(AbstractObjectInfo objectInfo)
        {
            if (objectInfo is NonNativeNullObjectInfo)
                return null;

            var instance = objectInfo.GetType() == typeof (NonNativeObjectInfo)
                               ? BuildOneInstance((NonNativeObjectInfo) objectInfo)
                               : BuildOneInstance((NativeObjectInfo) objectInfo);

            return instance;
        }

        public object BuildCollectionInstance(CollectionObjectInfo coi)
        {
            var type = OdbClassPool.GetClass(coi.GetRealCollectionClassName());

            return type.IsGenericType
                       ? BuildGenericCollectionInstance(coi, type)
                       : BuildNonGenericCollectionInstance(coi, type);
        }

        public object BuildGenericCollectionInstance(CollectionObjectInfo coi, Type t)
        {
            object newCollection;
            try
            {
                newCollection = Activator.CreateInstance(t);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new OdbRuntimeException(
                    NDatabaseError.CollectionInstanciationError.AddParameter(coi.GetRealCollectionClassName()));
            }

            var method = t.GetMethod("Add");

            foreach (var abstractObjectInfo in coi.GetCollection())
            {
                if (!abstractObjectInfo.IsDeletedObject())
                    method.Invoke(newCollection, new[] { BuildOneInstance(abstractObjectInfo) });
            }

            return newCollection;
        }

        public object BuildNonGenericCollectionInstance(CollectionObjectInfo coi, Type t)
        {
            var newCollection = (IList)Activator.CreateInstance(OdbClassPool.GetClass(coi.GetRealCollectionClassName()));
            IEnumerator iterator = coi.GetCollection().GetEnumerator();

            while (iterator.MoveNext())
            {
                var abstractObjectInfo = (AbstractObjectInfo) iterator.Current;
                if (!abstractObjectInfo.IsDeletedObject())
                    newCollection.Add(BuildOneInstance(abstractObjectInfo));
            }
            return newCollection;
        }

        /// <summary>
        ///   Builds an instance of an enum
        /// </summary>
        public object BuildEnumInstance(EnumNativeObjectInfo enoi, Type enumClass)
        {
            return Enum.Parse(enumClass, enoi.GetEnumName(), false);
        }

        /// <summary>
        ///   Builds an instance of an array
        /// </summary>
        public object BuildArrayInstance(ArrayObjectInfo aoi)
        {
            // first check if array element type is native (int,short, for example)
            var type = OdbType.GetFromName(aoi.GetRealArrayComponentClassName());

            var arrayClazz = type.GetNativeClass();
            object array = Array.CreateInstance(arrayClazz, aoi.GetArray().Length);

            for (var i = 0; i < aoi.GetArrayLength(); i++)
            {
                var abstractObjectInfo = (AbstractObjectInfo) aoi.GetArray()[i];
                if (abstractObjectInfo != null && !abstractObjectInfo.IsDeletedObject() && !abstractObjectInfo.IsNull())
                {
                    var instance = BuildOneInstance(abstractObjectInfo);
                    ((Array) array).SetValue(instance, i);
                }
            }
            return array;
        }

        public object BuildMapInstance(MapObjectInfo moi)
        {
            var type = OdbClassPool.GetClass(moi.GetRealMapClassName());

            return type.IsGenericType
                       ? BuildGenericMapInstance(moi, type)
                       : BuildNonGenericMapInstance(moi, type);
        }

        public IDictionary BuildNonGenericMapInstance(MapObjectInfo mapObjectInfo, Type t)
        {
            var map = mapObjectInfo.GetMap();
            IDictionary newMap;

            try
            {
                newMap = (IDictionary) Activator.CreateInstance(t);
            }
            catch (Exception e)
            {
                throw new OdbRuntimeException(
                    NDatabaseError.MapInstanciationError.AddParameter(map.GetType().FullName), e);
            }

            foreach (var key in map.Keys)
            {
                var realKey = BuildOneInstance(key);
                var realValue = BuildOneInstance(map[key]);
                newMap[realKey] = realValue;
            }

            return newMap;
        }

        public object BuildGenericMapInstance(MapObjectInfo mapObjectInfo, Type t)
        {
            var map = mapObjectInfo.GetMap();
            object newMap;
            try
            {
                newMap = Activator.CreateInstance(t);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new OdbRuntimeException(NDatabaseError.MapInstanciationError.AddParameter(map.GetType().FullName),
                                              e);
            }

            var method = t.GetMethod("Add", t.GetGenericArguments());

            foreach (var key in map.Keys)
            {
                var realKey = BuildOneInstance(key);
                var realValue = BuildOneInstance(map[key]);
                method.Invoke(newMap, new[] { realKey, realValue });
            }
            
            return newMap;
        }

        public object BuildOneInstance(NativeObjectInfo objectInfo)
        {
            if (objectInfo.IsAtomicNativeObject())
                return BuildOneInstance((AtomicNativeObjectInfo) objectInfo);
            if (objectInfo.IsCollectionObject())
            {
                var coi = (CollectionObjectInfo) objectInfo;
                var value = BuildCollectionInstance(coi);
                
                return value;
            }
            if (objectInfo.IsArrayObject())
                return BuildArrayInstance((ArrayObjectInfo) objectInfo);
            if (objectInfo.IsMapObject())
                return BuildMapInstance((MapObjectInfo) objectInfo);
            if (objectInfo.IsNull())
                return null;
            throw new OdbRuntimeException(
                NDatabaseError.InstanceBuilderNativeType.AddParameter(OdbType.GetNameFromId(objectInfo.GetOdbTypeId())));
        }

        public object BuildOneInstance(AtomicNativeObjectInfo objectInfo)
        {
            var odbTypeId = objectInfo.GetOdbTypeId();
            long l;

            switch (odbTypeId)
            {
                case OdbType.NullId:
                {
                    return null;
                }

                case OdbType.StringId:
                {
                    return objectInfo.GetObject();
                }

                case OdbType.DateId:
                {
                    return objectInfo.GetObject();
                }

                case OdbType.LongId:
                {
                    if (objectInfo.GetObject() is long)
                        return objectInfo.GetObject();
                    return Convert.ToInt64(objectInfo.GetObject().ToString());
                }

                case OdbType.IntegerId:
                {
                    if (objectInfo.GetObject() is int)
                        return objectInfo.GetObject();
                    return Convert.ToInt32(objectInfo.GetObject().ToString());
                }

                case OdbType.BooleanId:
                {
                    if (objectInfo.GetObject() is bool)
                        return objectInfo.GetObject();
                    return Convert.ToBoolean(objectInfo.GetObject().ToString());
                }

                case OdbType.ByteId:
                {
                    if (objectInfo.GetObject() is byte)
                        return objectInfo.GetObject();
                    return Convert.ToByte(objectInfo.GetObject().ToString());
                }

                case OdbType.ShortId:
                {
                    if (objectInfo.GetObject() is short)
                        return objectInfo.GetObject();
                    return Convert.ToInt16(objectInfo.GetObject().ToString());
                }

                case OdbType.FloatId:
                {
                    if (objectInfo.GetObject() is float)
                        return objectInfo.GetObject();
                    return Convert.ToSingle(objectInfo.GetObject().ToString());
                }

                case OdbType.DoubleId:
                {
                    if (objectInfo.GetObject() is double)
                        return objectInfo.GetObject();
                    return Convert.ToDouble(objectInfo.GetObject().ToString());
                }

                case OdbType.DecimalId:
                {
                    return Decimal.Parse(objectInfo.GetObject().ToString(), NumberStyles.Any);
                }

                case OdbType.CharacterId:
                {
                    if (objectInfo.GetObject() is char)
                        return objectInfo.GetObject();
                    return objectInfo.GetObject().ToString()[0];
                }

                case OdbType.ObjectOidId:
                {
                    if (objectInfo.GetObject() is long)
                        l = (long) objectInfo.GetObject();
                    else
                    {
                        var oid = (OID) objectInfo.GetObject();
                        l = oid.ObjectId;
                    }
                    return OIDFactory.BuildObjectOID(l);
                }

                case OdbType.ClassOidId:
                {
                    if (objectInfo.GetObject() is long)
                        l = (long) objectInfo.GetObject();
                    else
                        l = Convert.ToInt64(objectInfo.GetObject().ToString());
                    return OIDFactory.BuildClassOID(l);
                }

                default:
                {
                    throw new OdbRuntimeException(
                        NDatabaseError.InstanceBuilderNativeTypeInCollectionNotSupported.AddParameter(
                            OdbType.GetNameFromId(odbTypeId)));
                }
            }
        }

        public static readonly string InstanceBuilderKey = "InstanceBuilder";
    }
}
