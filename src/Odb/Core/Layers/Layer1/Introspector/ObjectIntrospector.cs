using System;
using System.Collections;
using System.Collections.Generic;
using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Odb.Core.Layers.Layer3;
using NDatabase.Tool.Wrappers;
using NDatabase.Tool.Wrappers.Map;

namespace NDatabase.Odb.Core.Layers.Layer1.Introspector
{
    /// <summary>
    ///   The local implementation of the Object Instrospector.
    /// </summary>
    public sealed class ObjectIntrospector : IObjectIntrospector
    {
        private readonly IClassIntrospector _classIntrospector;
        private IStorageEngine _storageEngine;

        public ObjectIntrospector(IStorageEngine storageEngine)
        {
            _storageEngine = storageEngine;
            _classIntrospector = ClassIntrospector.Instance;
        }

        #region IObjectIntrospector Members

        public AbstractObjectInfo GetMetaRepresentation(object o, ClassInfo ci, bool recursive,
                                                        IDictionary<object, NonNativeObjectInfo> alreadyReadObjects,
                                                        IIntrospectionCallback callback)
        {
            return GetObjectInfo(o, ci, recursive, alreadyReadObjects, callback);
        }

        public void Clear()
        {
            _storageEngine = null;
        }

        #endregion

        private NonNativeObjectInfo BuildNnoi(object o, ClassInfo classInfo, AbstractObjectInfo[] values,
                                              long[] attributesIdentification, int[] attributeIds)
        {
            var nnoi = new NonNativeObjectInfo(o, classInfo, values, attributesIdentification, attributeIds);

            if (_storageEngine != null)
            {
                // for unit test purpose
                var cache = _storageEngine.GetSession(true).GetCache();

                // Check if object is in the cache, if so sets its oid
                var oid = cache.GetOid(o, false);
                if (oid != null)
                {
                    nnoi.SetOid(oid);
                    // Sets some values to the new header to keep track of the infos when
                    // executing NDatabase without closing it, just committing.
                    // Bug reported by Andy
                    var objectInfoHeader = cache.GetObjectInfoHeaderFromOid(oid, true);
                    nnoi.GetHeader().SetObjectVersion(objectInfoHeader.GetObjectVersion());
                    nnoi.GetHeader().SetUpdateDate(objectInfoHeader.GetUpdateDate());
                    nnoi.GetHeader().SetCreationDate(objectInfoHeader.GetCreationDate());
                }
            }
            return nnoi;
        }

        /// <summary>
        ///   retrieve object data
        /// </summary>
        /// <returns> The object info </returns>
        private AbstractObjectInfo GetObjectInfo(object o, ClassInfo ci, bool recursive,
                                                 IDictionary<object, NonNativeObjectInfo> alreadyReadObjects,
                                                 IIntrospectionCallback callback)
        {
            return GetObjectInfoInternal(null, o, ci, recursive, alreadyReadObjects, callback);
        }

        private AbstractObjectInfo GetNativeObjectInfoInternal(OdbType type, object o, bool recursive,
                                                               IDictionary<object, NonNativeObjectInfo>
                                                                   alreadyReadObjects, IIntrospectionCallback callback)
        {
            AbstractObjectInfo aoi = null;
            if (type.IsAtomicNative())
            {
                if (o == null)
                    aoi = new NullNativeObjectInfo(type.Id);
                else
                    aoi = new AtomicNativeObjectInfo(o, type.Id);
            }
            else
            {
                if (type.IsCollection())
                    aoi = IntrospectCollection((ICollection) o, recursive, alreadyReadObjects, callback);
                else
                {
                    if (type.IsArray())
                    {
                        if (o == null)
                            aoi = new ArrayObjectInfo(null);
                        else
                        {
                            // Gets the type of the elements of the array
                            var realArrayClassName = OdbClassUtil.GetFullName(o.GetType().GetElementType());
                            var arrayObjectInfo = recursive
                                                      ? IntrospectArray(o, true, alreadyReadObjects, type, callback)
                                                      : new ArrayObjectInfo((object[]) o);

                            arrayObjectInfo.SetRealArrayComponentClassName(realArrayClassName);
                            aoi = arrayObjectInfo;
                        }
                    }
                    else
                    {
                        if (type.IsMap())
                        {
                            if (o == null)
                                aoi = new MapObjectInfo(null, type, typeof(Hashtable).FullName);
                            else
                            {
                                MapObjectInfo moi;
                                var realMapClassName = OdbClassUtil.GetFullName(o.GetType());
                                var isGeneric = o.GetType().IsGenericType;

                                if (isGeneric)
                                {
                                    moi =
                                        new MapObjectInfo(
                                            IntrospectGenericMap((IDictionary<object, object>) o, recursive,
                                                                 alreadyReadObjects, callback), type, realMapClassName);
                                }
                                else
                                {
                                    moi =
                                        new MapObjectInfo(
                                            IntrospectNonGenericMap((IDictionary) o, recursive, alreadyReadObjects,
                                                                    callback), type, realMapClassName);
                                }
                                aoi = moi;
                            }
                        }
                        else
                        {
                            if (type.IsEnum())
                            {
                                var enumObject = (Enum) o;
                                if (enumObject == null)
                                    aoi = new NullNativeObjectInfo(type.Size);
                                else
                                {
                                    var enumClassName = OdbClassUtil.GetFullName(enumObject.GetType());
                                    // Here we must check if the enum is already in the meta model. Enum must be stored in the meta
                                    // model to optimize its storing as we need to keep track of the enum class
                                    // for each enum stored. So instead of storing the enum class name, we can store enum class id, a long
                                    // instead of the full enum class name string
                                    var classInfo = GetClassInfo(enumClassName);
                                    var enumValue = enumObject.ToString();
                                    aoi = new EnumNativeObjectInfo(classInfo, enumValue);
                                }
                            }
                        }
                    }
                }
            }
            return aoi;
        }

        /// <summary>
        ///   Build a meta representation of an object 
        ///   <pre>warning: When an object has two fields with the same name 
        ///        (a private field with the same name in a parent class, the deeper field (of the parent) is ignored!)</pre>
        /// </summary>
        /// <returns> The ObjectInfo </returns>
        private AbstractObjectInfo GetObjectInfoInternal(AbstractObjectInfo nnoi, object o, ClassInfo classInfo,
                                                         bool recursive,
                                                         IDictionary<object, NonNativeObjectInfo> alreadyReadObjects,
                                                         IIntrospectionCallback callback)
        {
            if (o == null)
                return NullNativeObjectInfo.GetInstance();

            var clazz = o.GetType();
            var type = OdbType.GetFromClass(clazz);
            var className = OdbClassUtil.GetFullName(clazz);
            if (type.IsNative())
                return GetNativeObjectInfoInternal(type, o, recursive, alreadyReadObjects, callback);

            // sometimes the clazz.getName() may not match the ci.getClassName()
            // It happens when the attribute is an interface or superclass of the
            // real attribute class
            // In this case, ci must be updated to the real class info
            if (classInfo != null && !classInfo.GetFullClassName().Equals(clazz.FullName))
            {
                classInfo = GetClassInfo(className);
                nnoi = null;
            }
            var mainAoi = (NonNativeObjectInfo) nnoi;
            var isRootObject = false;

            if (alreadyReadObjects == null)
            {
                alreadyReadObjects = new OdbHashMap<object, NonNativeObjectInfo>();
                isRootObject = true;
            }

            NonNativeObjectInfo cachedNnoi;
            alreadyReadObjects.TryGetValue(o, out cachedNnoi);

            if (cachedNnoi != null)
                return new ObjectReference(cachedNnoi);

            if (callback != null)
                callback.ObjectFound(o);

            if (mainAoi == null)
                mainAoi = BuildNnoi(o, classInfo, null, null, null);

            alreadyReadObjects[o] = mainAoi;
            var fields = _classIntrospector.GetAllFields(className);
            // For all fields

            foreach (var field in fields)
            {
                try
                {
                    var value = field.GetValue(o);
                    var attributeId = classInfo.GetAttributeId(field.Name);
                    if (attributeId == -1)
                    {
                        throw new OdbRuntimeException(
                            NDatabaseError.ObjectIntrospectorNoFieldWithName.AddParameter(classInfo.GetFullClassName()).
                                AddParameter(field.Name));
                    }

                    var valueType = OdbType.GetFromClass(value == null
                                                             ? field.FieldType
                                                             : value.GetType());
                    // for native fields
                    AbstractObjectInfo abstractObjectInfo;

                    if (valueType.IsNative())
                    {
                        abstractObjectInfo = GetNativeObjectInfoInternal(valueType, value, recursive, alreadyReadObjects,
                                                                         callback);
                        mainAoi.SetAttributeValue(attributeId, abstractObjectInfo);
                    }
                    else
                    {
                        //callback.objectFound(value);
                        // Non Native Objects
                        if (value == null)
                        {
                            var classInfo1 = GetClassInfo(OdbClassUtil.GetFullName(field.GetType()));

                            abstractObjectInfo = new NonNativeNullObjectInfo(classInfo1);
                            mainAoi.SetAttributeValue(attributeId, abstractObjectInfo);
                        }
                        else
                        {
                            var classInfo2 = GetClassInfo(OdbClassUtil.GetFullName(value.GetType()));
                            if (recursive)
                            {
                                abstractObjectInfo = GetObjectInfoInternal(null, value, classInfo2, true,
                                                                           alreadyReadObjects, callback);
                                mainAoi.SetAttributeValue(attributeId, abstractObjectInfo);
                            }
                            else
                            {
                                // When it is not recursive, simply add the object
                                // values.add(value);
                                throw new OdbRuntimeException(
                                    NDatabaseError.InternalError.AddParameter(
                                        "Should not enter here - ObjectIntrospector - 'simply add the object'"));
                            }
                        }
                    }
                }
                catch (ArgumentException e)
                {
                    throw new OdbRuntimeException(
                        NDatabaseError.InternalError.AddParameter("in getObjectInfoInternal"), e);
                }
                catch (MemberAccessException e)
                {
                    throw new OdbRuntimeException(NDatabaseError.InternalError.AddParameter("getObjectInfoInternal"), e);
                }
            }

            if (isRootObject)
                alreadyReadObjects.Clear();

            return mainAoi;
        }

        private CollectionObjectInfo IntrospectCollection(ICollection collection, bool introspect,
                                                          IDictionary<object, NonNativeObjectInfo> alreadyReadObjects,
                                                          IIntrospectionCallback callback)
        {
            if (collection == null)
                return new CollectionObjectInfo();

            // A collection that contain all meta representations of the collection
            // objects
            ICollection<AbstractObjectInfo> collectionCopy = new List<AbstractObjectInfo>(collection.Count);
            // A collection to keep references all all non native objects of the
            // collection
            // This will be used later to get all non native objects contained in an
            // object
            ICollection<NonNativeObjectInfo> nonNativesObjects = new List<NonNativeObjectInfo>(collection.Count);

            foreach (var o in collection)
            {
                // Null objects are not inserted in list
                if (o == null)
                    continue;

                var classInfo = GetClassInfo(OdbClassUtil.GetFullName(o.GetType()));
                var abstractObjectInfo = GetObjectInfo(o, classInfo, introspect, alreadyReadObjects, callback);

                collectionCopy.Add(abstractObjectInfo);

                if (abstractObjectInfo.IsNonNativeObject())
                {
                    // o is not null, call the callback with it
                    //callback.objectFound(o);
                    // This is a non native object
                    nonNativesObjects.Add((NonNativeObjectInfo) abstractObjectInfo);
                }
            }

            var collectionObjectInfo = new CollectionObjectInfo(collectionCopy, nonNativesObjects);

            var realCollectionClassName = OdbClassUtil.GetFullName(collection.GetType());

            collectionObjectInfo.SetRealCollectionClassName(realCollectionClassName);
            return collectionObjectInfo;
        }

        private IDictionary<AbstractObjectInfo, AbstractObjectInfo> IntrospectNonGenericMap(IDictionary map,
                                                                                            bool introspect,
                                                                                            IDictionary <object, NonNativeObjectInfo> alreadyReadObjects,
                                                                                            IIntrospectionCallback callback)
        {
            IDictionary<AbstractObjectInfo, AbstractObjectInfo> mapCopy =
                new OdbHashMap<AbstractObjectInfo, AbstractObjectInfo>();
            var keySet = map.Keys;
            ClassInfo ciValue = null;

            foreach (var key in keySet)
            {
                var value = map[key];

                if (key == null)
                    continue;

                var classInfoKey = GetClassInfo(OdbClassUtil.GetFullName(key.GetType()));
                if (value != null)
                    ciValue = GetClassInfo(OdbClassUtil.GetFullName(value.GetType()));

                var abstractObjectInfoForKey = GetObjectInfo(key, classInfoKey, introspect, alreadyReadObjects, callback);
                var abstractObjectInfoForValue = GetObjectInfo(value, ciValue, introspect, alreadyReadObjects, callback);
                mapCopy.Add(abstractObjectInfoForKey, abstractObjectInfoForValue);
            }

            return mapCopy;
        }

        private IDictionary<AbstractObjectInfo, AbstractObjectInfo> IntrospectGenericMap(
            IDictionary<object, object> map, bool introspect,
            IDictionary<object, NonNativeObjectInfo> alreadyReadObjects, IIntrospectionCallback callback)
        {
            var mapCopy = new OdbHashMap<AbstractObjectInfo, AbstractObjectInfo>();

            ClassInfo ciValue = null;

            foreach (var key in map.Keys)
            {
                var value = map[key];
                if (key == null)
                    continue;

                var classInfoKey = GetClassInfo(OdbClassUtil.GetFullName(key.GetType()));
                if (value != null)
                    ciValue = GetClassInfo(OdbClassUtil.GetFullName(value.GetType()));

                var abstractObjectInfoForKey = GetObjectInfo(key, classInfoKey, introspect, alreadyReadObjects,
                                                             callback);
                var abstractObjectInfoForValue = GetObjectInfo(value, ciValue, introspect, alreadyReadObjects,
                                                               callback);
                mapCopy.Add(abstractObjectInfoForKey, abstractObjectInfoForValue);
            }

            return mapCopy;
        }

        private ClassInfo GetClassInfo(string fullClassName)
        {
            if (OdbType.GetFromName(fullClassName).IsNative())
                return null;

            var session = _storageEngine.GetSession(true);
            var metaModel = session.GetMetaModel();
            if (metaModel.ExistClass(fullClassName))
                return metaModel.GetClassInfo(fullClassName, true);

            var classInfoList = _classIntrospector.Introspect(fullClassName, true);

            // to enable junit tests
            if (_storageEngine != null)
                _storageEngine.AddClasses(classInfoList);
            else
                metaModel.AddClasses(classInfoList);

            return metaModel.GetClassInfo(fullClassName, true);
        }

        private ArrayObjectInfo IntrospectArray(object array, bool introspect,
                                                IDictionary<object, NonNativeObjectInfo> alreadyReadObjects,
                                                OdbType valueType, IIntrospectionCallback callback)
        {
            var length = OdbArray.GetArrayLength(array);
            var elementType = array.GetType().GetElementType();
            var type = OdbType.GetFromClass(elementType);

            if (type.IsAtomicNative())
                return IntropectAtomicNativeArray(array, type);

            if (!introspect)
                return new ArrayObjectInfo((object[]) array);

            var arrayCopy = new object[length];
            for (var i = 0; i < length; i++)
            {
                var o = OdbArray.GetArrayElement(array, i);
                if (o != null)
                {
                    var classInfo = GetClassInfo(OdbClassUtil.GetFullName(o.GetType()));
                    var abstractObjectInfo = GetObjectInfo(o, classInfo, true, alreadyReadObjects, callback);
                    arrayCopy[i] = abstractObjectInfo;
                }
                else
                    arrayCopy[i] = new NonNativeNullObjectInfo();
            }

            return new ArrayObjectInfo(arrayCopy, valueType, type.Id);
        }

        private static ArrayObjectInfo IntropectAtomicNativeArray(object array, OdbType type)
        {
            var length = OdbArray.GetArrayLength(array);
            var arrayCopy = new object[length];
            for (var i = 0; i < length; i++)
            {
                var o = OdbArray.GetArrayElement(array, i);
                if (o != null)
                {
                    // If object is not null, try to get the exact type
                    var typeId = OdbType.GetFromClass(o.GetType()).Id;
                    var atomicNativeObjectInfo = new AtomicNativeObjectInfo(o, typeId);
                    arrayCopy[i] = atomicNativeObjectInfo;
                }
                else
                {
                    // Else take the declared type
                    arrayCopy[i] = new NullNativeObjectInfo(type.Id);
                }
            }

            return new ArrayObjectInfo(arrayCopy, OdbType.Array, type.Id);
        }
    }
}
