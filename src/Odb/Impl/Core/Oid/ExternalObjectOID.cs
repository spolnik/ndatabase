using System;
using System.Globalization;
using System.Text;
using NDatabase.Odb.Core;

namespace NDatabase.Odb.Impl.Core.Oid
{
    [Serializable]
    public sealed class ExternalObjectOID : OdbObjectOID, IExternalOID
    {
        private readonly IDatabaseId _databaseId;

        public ExternalObjectOID(OID oid, IDatabaseId databaseId)
            : base(oid.ObjectId)
        {
            _databaseId = databaseId;
        }

        public IDatabaseId GetDatabaseId()
        {
            return _databaseId;
        }

        public override string GetTypeName()
        {
            return OIDTypes.TypeNameExternalObjectOid;
        }

        public override int GetType()
        {
            return OIDTypes.TypeExternalObjectOid;
        }

        /// <summary>
        ///   To retrieve a string representation of an OID
        /// </summary>
        /// <returns> </returns>
        public override string OidToString()
        {
            var buffer =
                new StringBuilder(GetTypeName())
                    .Append(":").Append(_databaseId)
                    .Append(":").Append(ObjectId.ToString(CultureInfo.InvariantCulture));

            return buffer.ToString();
        }

        public new static ExternalObjectOID OidFromString(string oidString)
        {
            var tokens = oidString.Split(':');

            if (tokens.Length != 3 || !(tokens[0].Equals(OIDTypes.TypeNameExternalObjectOid)))
                throw new OdbRuntimeException(NDatabaseError.InvalidOidRepresentation.AddParameter(oidString));

            var oid = long.Parse(tokens[2]);
            var databaseid = tokens[1];
            return new ExternalObjectOID(new OdbClassOID(oid), DatabaseIdImpl.FromString(databaseid));
        }
    }
}
