using System;
using System.Globalization;
using System.Text;
using NDatabase.Odb.Core;
using NDatabase.Odb.Core.Layers.Layer2.Meta;

namespace NDatabase.Odb.Impl.Core.Oid
{
    [Serializable]
    public class ExternalClassOID : OdbClassOID, IExternalOID
    {
        private readonly IDatabaseId _databaseId;

        public ExternalClassOID(OID oid, IDatabaseId databaseId) : base(oid.ObjectId)
        {
            _databaseId = databaseId;
        }

        #region IExternalOID Members

        public IDatabaseId GetDatabaseId()
        {
            return _databaseId;
        }

        #endregion
    }
}
