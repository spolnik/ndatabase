using System.Globalization;

namespace NDatabase.Odb
{
    /// <summary>
    ///   The abstract class to represent OID.
    /// </summary>
    /// <remarks>
    ///   The abstract class to represent OID. OID is a unique identifier for NDatabase ODB entities like objects and classes. The id is generated by NDatabase
    /// </remarks>
    internal abstract class BaseOID : OID
    {
        protected BaseOID(long oid)
        {
            ObjectId = oid;
        }

        public long ObjectId { get; private set; }

        public abstract int CompareTo(OID obj);

        protected static long UrShift(long number, int bits)
        {
            if (number >= 0)
                return number >> bits;

            return (number >> bits) + (2L << ~bits);
        }

        public override string ToString()
        {
            return ObjectId.ToString(CultureInfo.InvariantCulture);
        }

        public int CompareTo(object obj)
        {
            var oid = obj as OID;
            return CompareTo(oid);
        }
    }
}