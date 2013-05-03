using NDatabase.Api;
using NDatabase.Api.Query;
using NDatabase.Core.Query;

namespace NDatabase.Core.Layers.Layer3
{
    internal interface IQueryEngine
    {
        IValues GetValues(IInternalValuesQuery query, int startIndex, int endIndex);

        IInternalObjectSet<T> GetObjects<T>(IQuery query, bool inMemory, int startIndex, int endIndex);

        OID GetObjectId<T>(T plainObject, bool throwExceptionIfDoesNotExist) where T : class;

        object GetObjectFromOid(OID oid);
    }
}