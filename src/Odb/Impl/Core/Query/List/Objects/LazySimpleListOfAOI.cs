using System;
using System.Collections.Generic;
using System.Text;
using NDatabase.Odb.Core;
using NDatabase.Odb.Core.Layers.Layer2.Instance;
using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Odb.Core.Lookup;
using NDatabase.Odb.Impl.Core.Layers.Layer2.Instance;
using NDatabase.Tool.Wrappers;
using NDatabase.Tool.Wrappers.List;

namespace NDatabase.Odb.Impl.Core.Query.List.Objects
{
    /// <summary>
    ///   A simple list to hold query result.
    /// </summary>
    /// <remarks>
    ///   A simple list to hold query result. It is used when no index and no order by This collection does not store the objects, it only holds the Abstract Object Info (AOI) of the objects. When user ask an object the object is lazy loaded by the buildInstance method
    /// </remarks>
    /// <author>osmadja</author>
    [Serializable]
    public class LazySimpleListOfAoi<T> : OdbArrayList<T>, IObjects<T>
    {
        /// <summary>
        ///   indicate if objects must be returned as instance (true) or as non native objects (false)
        /// </summary>
        private readonly bool _returnInstance;

        /// <summary>
        ///   this session id is used to store the odb session id.
        /// </summary>
        /// <remarks>
        ///   this session id is used to store the odb session id. When in true client server mode, when the lazy list is sent back to the client, the instance builder (declared as transient) will be null on the client side. Then the client will use the Lookup class with the base id to obtain the client instance builder
        /// </remarks>
        private readonly string _sessionId;

        /// <summary>
        ///   a cursor when getting objects
        /// </summary>
        private int _currentPosition;

        /// <summary>
        ///   The odb engine to lazily get objects
        /// </summary>
        [NonSerialized]
        private IInstanceBuilder _instanceBuilder;

        public LazySimpleListOfAoi() : base(10)
        {
        }

        /// <param name="builder"> </param>
        /// <param name="returnInstance"> </param>
        public LazySimpleListOfAoi(IInstanceBuilder builder, bool returnInstance) : base(10)
        {
            _instanceBuilder = builder;
            _sessionId = builder.GetSessionId();
            _returnInstance = returnInstance;
        }

        #region IObjects<T> Members

        public virtual bool AddWithKey(IOdbComparable key, T @object)
        {
            throw new OdbRuntimeException(NDatabaseError.OperationNotImplemented);
        }

        public virtual bool AddWithKey(int key, T @object)
        {
            throw new OdbRuntimeException(NDatabaseError.OperationNotImplemented);
        }

        public virtual T GetFirst()
        {
            try
            {
                return this[0];
            }
            catch (Exception e)
            {
                throw new OdbRuntimeException(NDatabaseError.ErrorWhileGettingObjectFromListAtIndex.AddParameter(0), e);
            }
        }

        public virtual bool HasNext()
        {
            return _currentPosition < Count;
        }

        public virtual IEnumerator<T> Iterator(OrderByConstants orderByType)
        {
            throw new OdbRuntimeException(NDatabaseError.OperationNotImplemented);
        }

        public virtual T Next()
        {
            try
            {
                return this[_currentPosition++];
            }
            catch (Exception e)
            {
                throw new OdbRuntimeException(NDatabaseError.ErrorWhileGettingObjectFromListAtIndex.AddParameter(0), e);
            }
        }

        public void AddOid(OID oid)
        {
            throw new Exception("Add Oid not implemented ");
        }

        public virtual void Reset()
        {
            _currentPosition = 0;
        }

        #endregion

        public override T Get(int index)
        {
            object o = base[index];
            var aoi = (AbstractObjectInfo) o;
            try
            {
                if (aoi.IsNull())
                    return default(T);

                if (_returnInstance)
                {
                    if (aoi.IsNative())
                        return (T) aoi.GetObject();

                    if (_instanceBuilder == null)
                    {
                        // Lookup the instance builder
                        _instanceBuilder = (IInstanceBuilder) LookupFactory.Get(_sessionId).Get(InstanceBuilder.InstanceBuilderKey);

                        if (_instanceBuilder == null)
                            throw new OdbRuntimeException(
                                NDatabaseError.LookupKeyNotFound.AddParameter(InstanceBuilder.InstanceBuilderKey));
                    }

                    return (T) _instanceBuilder.BuildOneInstance((NonNativeObjectInfo) aoi);
                }
                // No need to return Instance return the layer 2 representation
                o = aoi;
                return (T) o;
            }
            catch (Exception e)
            {
                throw new OdbRuntimeException(NDatabaseError.ErrorWhileGettingObjectFromListAtIndex.AddParameter(index),
                                              e);
            }
        }

        public override string ToString()
        {
            var buffer = new StringBuilder();
            buffer.Append("list with ").Append(Count).Append(" elements");
            return buffer.ToString();
        }
    }
}
