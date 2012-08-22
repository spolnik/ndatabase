using System.Text;
using NDatabase.Odb.Core;
using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Odb.Impl.Tool;

namespace NDatabase.Odb.Impl.Core.Layers.Layer2.Meta.Serialization
{
    public sealed class NativeObjectSerializer : ISerializer
    {
        public static readonly string ClassId = Serializer.GetClassId(typeof (NativeObjectInfo));

        #region ISerializer Members

        
        public object FromString(string data)
        {
            var tokens = data.Split(Serializer.FieldSeparator);

            if (!tokens[0].Equals(ClassId))
                throw new OdbRuntimeException(
                    NDatabaseError.SerializationFromString.AddParameter(ClassId).AddParameter(tokens[0]));
            var odbTypeId = int.Parse(tokens[1]);
            return ObjectTool.StringToObjectInfo(odbTypeId, tokens[2], ObjectTool.IdCallerIsSerializer, null);
        }

        public string ToString(object @object)
        {
            var anoi = (AtomicNativeObjectInfo) @object;
            var buffer = new StringBuilder();
            
            buffer.Append(ClassId).Append(Serializer.FieldSeparator);
            buffer.Append(anoi.GetOdbTypeId()).Append(Serializer.FieldSeparator);
            buffer.Append(ObjectTool.AtomicNativeObjectToString(anoi, ObjectTool.IdCallerIsSerializer));
            return buffer.ToString();
        }

        #endregion
    }
}
