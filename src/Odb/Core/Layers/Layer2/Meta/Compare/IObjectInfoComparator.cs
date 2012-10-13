using System.Collections.Generic;

namespace NDatabase2.Odb.Core.Layers.Layer2.Meta.Compare
{
    internal interface IObjectInfoComparator
    {
        bool HasChanged(AbstractObjectInfo aoi1, AbstractObjectInfo aoi2);

        void Clear();

        int GetNbChanges();

        IList<ChangedObjectInfo> GetChanges();

        IList<NewNonNativeObjectAction> GetNewObjectMetaRepresentations();

        NewNonNativeObjectAction GetNewObjectMetaRepresentation(int i);

        IList<object> GetNewObjects();

        int GetMaxObjectRecursionLevel();

        IList<IChangedAttribute> GetChangedAttributeActions();

        IList<ArrayModifyElement> GetArrayChanges();

        IList<SetAttributeToNullAction> GetAttributeToSetToNull();

        AbstractObjectInfo GetChangedObjectMetaRepresentation(int i);
    }
}
