using System.Collections.Generic;

namespace NDatabase.Odb.Core.Layers.Layer2.Meta.Compare
{
    internal interface IObjectInfoComparator
    {
        bool HasChanged(AbstractObjectInfo aoi1, AbstractObjectInfo aoi2);

        void Clear();

        int GetNbChanges();

        IList<IChangedAttribute> GetChangedAttributeActions();

        IList<ArrayModifyElement> GetArrayChanges();

        IList<SetAttributeToNullAction> GetAttributeToSetToNull();
    }
}
