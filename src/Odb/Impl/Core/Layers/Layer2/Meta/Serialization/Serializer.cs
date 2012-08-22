using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Tool.Wrappers.Map;

namespace NDatabase.Odb.Impl.Core.Layers.Layer2.Meta.Serialization
{
    public static class Serializer
    {
        public static readonly char CollectionElementSeparator = ',';

        public static readonly char FieldSeparator = ';';

        public static readonly char AttributeSeparator = '|';

        public static readonly string CollectionStart = "(";

        public static readonly string CollectionEnd = ")";

        private static readonly IDictionary<string, ISerializer> Serializers;

        static Serializer()
        {
            Serializers = new OdbHashMap<string, ISerializer>
                               {
                                   {GetClassId(typeof (AtomicNativeObjectInfo)), new AtomicNativeObjectSerializer()},
                                   {GetClassId(typeof (CollectionObjectInfo)), new CollectionObjectInfoSerializer()}
                               };
        }

        public static string ToString(IList objectList)
        {
            var buffer = new StringBuilder();

            foreach (var item in objectList)
                buffer.Append(ToString(item)).Append("\n");

            return buffer.ToString();
        }

        public static string ToString(object @object)
        {
            var classId = GetClassId(@object.GetType());
            var serializer = Serializers[classId];

            if (serializer != null)
                return serializer.ToString(@object);

            throw new Exception(string.Format("toString not implemented for {0}", @object.GetType().FullName));
        }

        public static object FromOneString(string data)
        {
            var index = data.IndexOf(";", StringComparison.Ordinal);

            if (index == -1)
                return null;

            var type = data.Substring(0, index);
            var serializer = Serializers[type];

            if (serializer != null)
                return serializer.FromString(data);

            throw new Exception(string.Format("fromString unimplemented for {0}", type));
        }

        public static string GetClassId(Type clazz)
        {
            if (clazz == typeof (AtomicNativeObjectInfo))
                return "1";

            if (clazz == typeof (CollectionObjectInfo))
                return "2";
            return "0";
        }
    }
}
