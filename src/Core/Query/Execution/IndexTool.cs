using System;
using System.Collections.Generic;
using NDatabase.Core.Layers.Layer2.Meta;
using NDatabase.Core.Query.Criteria;
using NDatabase.Exceptions;
using NDatabase.Tool.Wrappers;

namespace NDatabase.Core.Query.Execution
{
    internal static class IndexTool
    {
        internal static IOdbComparable BuildIndexKey(string indexName, NonNativeObjectInfo oi, int[] fieldIds)
        {
            var keys = new IOdbComparable[fieldIds.Length];

            for (var i = 0; i < fieldIds.Length; i++)
            {
                // Todo : can we assume that the object is a Comparable
                try
                {
                    var aoi = oi.GetAttributeValueFromId(fieldIds[i]);
                    var item = (IComparable) aoi.GetObject();
                    
                    // If the index is on NonNativeObjectInfo, then the key is the oid 
                    // of the object
                    if (aoi.IsNonNativeObject())
                    {
                        var nnoi = (NonNativeObjectInfo) aoi;
                        item = nnoi.GetOid();
                    }

                    keys[i] = new SimpleCompareKey(item);
                }
                catch (Exception)
                {
                    throw new OdbRuntimeException(
                        NDatabaseError.IndexKeysMustImplementComparable.AddParameter(indexName).AddParameter(fieldIds[i]).AddParameter(
                            oi.GetAttributeValueFromId(fieldIds[i]).GetType().FullName));
                }
            }

            return keys.Length == 1 ? keys[0] : new ComposedCompareKey(keys);
        }

        internal static IOdbComparable BuildIndexKey(string indexName, AttributeValuesMap values, IList<string> fields)
        {
            if (fields.Count == 1)
                return new SimpleCompareKey(values.GetComparable(fields[0]));

            var keys = new IOdbComparable[fields.Count];
            for (var i = 0; i < fields.Count; i++)
            {
                // TODO : can we assume that the object is a Comparable
                try
                {
                    var @object = (IComparable) values[fields[i]];
                    
                    keys[i] = new SimpleCompareKey(@object);
                }
                catch (Exception)
                {
                    throw new OdbRuntimeException(
                        NDatabaseError.IndexKeysMustImplementComparable.AddParameter(indexName).AddParameter(fields[i]).
                            AddParameter(values[fields[i]].GetType().FullName));
                }
            }

            var key = new ComposedCompareKey(keys);
            return key;
        }

        /// <summary>
        ///   Take the fields of the index and take value from the query
        /// </summary>
        /// <param name="ci"> The class info involved </param>
        /// <param name="index"> The index </param>
        /// <param name="query"> </param>
        /// <returns> The key of the index </returns>
        internal static IOdbComparable ComputeKey(ClassInfo ci, ClassInfoIndex index, SodaQuery query)
        {
            var attributesNames = ci.GetAttributeNames(index.AttributeIds);
            var constraint = query.GetCriteria();
            var values = ((IInternalConstraint)constraint).GetValues();
            return BuildIndexKey(index.Name, values, attributesNames);
        }
    }
}
