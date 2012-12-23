using System.Collections;
using System.Collections.Generic;
using NDatabase2.Tool.Wrappers.List;

namespace NDatabase2.Odb.Core.Layers.Layer2.Meta
{
    /// <summary>
    ///   Meta representation of a collection
    /// </summary>
    internal sealed class CollectionObjectInfo : GroupObjectInfo
    {
        private string _realCollectionClassName;

        public CollectionObjectInfo() : base(null, OdbType.CollectionId)
        {
            _realCollectionClassName = OdbType.DefaultCollectionClassName;
        }

        public CollectionObjectInfo(ICollection<AbstractObjectInfo> collection) : base(collection, OdbType.CollectionId)
        {
            _realCollectionClassName = OdbType.DefaultCollectionClassName;
        }

        public CollectionObjectInfo(ICollection<AbstractObjectInfo> collection,
                                    ICollection<NonNativeObjectInfo> nonNativeObjects)
            : base(collection, OdbType.CollectionId)
        {
            _realCollectionClassName = OdbType.DefaultCollectionClassName;
            SetNonNativeObjects(nonNativeObjects);
        }

        private CollectionObjectInfo(ICollection<AbstractObjectInfo> collection, OdbType type,
                                    ICollection<NonNativeObjectInfo> nonNativeObjects) : base(collection, type)
        {
            _realCollectionClassName = OdbType.DefaultCollectionClassName;
            SetNonNativeObjects(nonNativeObjects);
        }

        public ICollection<AbstractObjectInfo> GetCollection()
        {
            return (ICollection<AbstractObjectInfo>) TheObject;
        }

        public override string ToString()
        {
            return TheObject != null 
                ? TheObject.ToString() 
                : "null collection";
        }

        public override bool IsCollectionObject()
        {
            return true;
        }

        public string GetRealCollectionClassName()
        {
            return _realCollectionClassName;
        }

        public void SetRealCollectionClassName(string realCollectionClass)
        {
            _realCollectionClassName = realCollectionClass;
        }

        public override AbstractObjectInfo CreateCopy(IDictionary<OID, AbstractObjectInfo> cache, bool onlyData)
        {
            var collection = (ICollection) TheObject;
            ICollection<AbstractObjectInfo> newCollection = new OdbList<AbstractObjectInfo>();
            // To keep track of non native objects
            IOdbList<NonNativeObjectInfo> nonNatives = new OdbList<NonNativeObjectInfo>();

            foreach (AbstractObjectInfo aoi in collection)
            {
                // create copy
                var aoiCopy = aoi.CreateCopy(cache, onlyData);
                newCollection.Add(aoiCopy);

                if (aoiCopy.IsNonNativeObject())
                    nonNatives.Add((NonNativeObjectInfo)aoiCopy);
            }
            
            var coi = new CollectionObjectInfo(newCollection, OdbType, nonNatives);
            coi.SetRealCollectionClassName(_realCollectionClassName);
            return coi;
        }
    }
}
