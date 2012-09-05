using System;
using System.Globalization;
using NDatabase.Odb.Core;
using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Odb.Core.Oid;

namespace NDatabase.Odb.Impl.Tool
{
    /// <summary>
    ///   Basic native Object formatter.
    /// </summary>
    public static class ObjectTool
    {
        /// <summary>
        ///   Convert a string representation to the right object <pre>If it is a representation of an int, it will return an Integer.</pre>
        /// </summary>
        /// <remarks>
        ///   Convert a string representation to the right object <pre>If it is a representation of an int, it will return an Integer.</pre>
        /// </remarks>
        /// <param name="odbTypeId"> The native object type </param>
        /// <param name="value"> The real value </param>
        /// <returns> The right object </returns>
        public static object StringToObject(int odbTypeId, string value)
        {
            object theObject = null;

            if (value == null || value.Equals("null"))
                return new NullNativeObjectInfo(odbTypeId);

            switch (odbTypeId)
            {
                case OdbType.ByteId:
                {
                    theObject = Convert.ToByte(value);
                    break;
                }

                case OdbType.ShortId:
                {
                    theObject = Convert.ToInt16(value);
                    break;
                }

                case OdbType.DecimalId:
                {
                    theObject = Convert.ToDecimal(value);
                    break;
                }

                case OdbType.BooleanId:
                {
                    theObject = value.Equals("true");
                    break;
                }

                case OdbType.CharacterId:
                {
                    theObject = value[0];
                    break;
                }

                case OdbType.DateId:
                {
                    theObject = new DateTime(long.Parse(value));
                    break;
                }

                case OdbType.FloatId:
                {
                    theObject = Convert.ToSingle(value);
                    break;
                }

                case OdbType.DoubleId:
                {
                    theObject = Convert.ToDouble(value);
                    break;
                }

                case OdbType.IntegerId:
                {
                    theObject = Convert.ToInt32(value);
                    break;
                }

                case OdbType.LongId:
                {
                    theObject = Convert.ToInt64(value);
                    break;
                }

                case OdbType.StringId:
                {
                    theObject = value;
                    break;
                }

                case OdbType.OidId:
                {
                    theObject = OIDFactory.BuildObjectOID(long.Parse(value));
                    break;
                }

                case OdbType.ObjectOidId:
                {
                    theObject = OIDFactory.BuildObjectOID(long.Parse(value));
                    break;
                }

                case OdbType.ClassOidId:
                {
                    theObject = OIDFactory.BuildClassOID(long.Parse(value));
                    break;
                }
            }
            if (theObject == null)
                throw new OdbRuntimeException(
                    NDatabaseError.NativeTypeNotSupported.AddParameter(OdbType.GetNameFromId(odbTypeId)));

            return theObject;
        }

        /// <param name="odbTypeId"> The native object type </param>
        /// <param name="value"> The real value </param>
        /// <param name="ci"> The ClassInfo. It is only used for enum where we need the enum class info. In other cases, it is null </param>
        /// <returns> The NativeObjectInfo that represents the specific value </returns>
        public static NativeObjectInfo StringToObjectInfo(int odbTypeId, string value)
        {
            if (OdbType.IsAtomicNative(odbTypeId))
            {
                var theObject = StringToObject(odbTypeId, value);
                return new AtomicNativeObjectInfo(theObject, odbTypeId);
            }

            if (OdbType.IsEnum(odbTypeId))
                return new EnumNativeObjectInfo(null, value);

            return NullNativeObjectInfo.GetInstance();
        }

        public static string AtomicNativeObjectToString(AtomicNativeObjectInfo anoi)
        {
            if (anoi == null || anoi.IsNull())
                return "null";

            if (anoi.GetObject() is DateTime)
                return ((DateTime) anoi.GetObject()).Millisecond.ToString(CultureInfo.InvariantCulture);

            return anoi.GetObject().ToString();
        }
    }
}
