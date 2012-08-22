using System.Collections;
using System.Collections.Generic;
using System.Text;
using NDatabase.Odb.Core;
using NDatabase.Odb.Core.Layers.Layer2.Meta;

namespace NDatabase.Odb.Impl.Core.Layers.Layer2.Meta.Serialization
{
    public sealed class CollectionObjectInfoSerializer : ISerializer
    {
        public static readonly string ClassId = Serializer.GetClassId(typeof (CollectionObjectInfo));

        #region ISerializer Members

        public object FromString(string data)
        {
            var tokens = data.Split(Serializer.FieldSeparator);

            if (!tokens[0].Equals(ClassId))
                throw new OdbRuntimeException(
                    NDatabaseError.SerializationFromString.AddParameter(ClassId).AddParameter(tokens[0]));

            var realCollectionName = tokens[1];
            var collectionSize = int.Parse(tokens[2]);
            var collectionData = tokens[3];
            
            var objects = collectionData.Split(Serializer.CollectionElementSeparator);

            if (objects.Length != collectionSize)
            {
                throw new OdbRuntimeException(
                    NDatabaseError.SerializationCollection.AddParameter(collectionSize).AddParameter(objects.Length));
            }

            ICollection<AbstractObjectInfo> l = new List<AbstractObjectInfo>(collectionSize);
            for (var i = 0; i < collectionSize; i++)
                l.Add((AbstractObjectInfo) Serializer.FromOneString(objects[i]));

            var coi = new CollectionObjectInfo(l);
            coi.SetRealCollectionClassName(realCollectionName);
            return coi;
        }

        public string ToString(object @object)
        {
            var coi = (CollectionObjectInfo) @object;
            var buffer = new StringBuilder();
            
            buffer.Append(ClassId).Append(Serializer.FieldSeparator);
            buffer.Append(coi.GetRealCollectionClassName()).Append(Serializer.FieldSeparator);
            buffer.Append(coi.GetCollection().Count).Append(Serializer.FieldSeparator);
            buffer.Append(Serializer.CollectionStart);
            IEnumerator iterator = coi.GetCollection().GetEnumerator();
            while (iterator.MoveNext())
            {
                buffer.Append(Serializer.ToString(iterator.Current));
                if (iterator.MoveNext())
                    buffer.Append(Serializer.CollectionElementSeparator);
            }
            buffer.Append(Serializer.CollectionEnd);
            return buffer.ToString();
        }

        #endregion
    }
}
