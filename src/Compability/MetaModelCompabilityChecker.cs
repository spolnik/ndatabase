using System;
using System.Collections.Generic;
using NDatabase.Exceptions;
using NDatabase.Odb.Core.Layers.Layer2.Meta;
using NDatabase.Odb.Core.Layers.Layer3.Engine;
using NDatabase.Tool;

namespace NDatabase.Compability
{
    internal class MetaModelCompabilityChecker
    {
        /// <summary>
        ///     Receive the current class info (loaded from current classes present on runtime and check against the persisted meta model
        /// </summary>
        internal static void Check(IDictionary<Type, ClassInfo> currentCIs, IMetaModel metaModel,
                                   Action updateMetaModel)
        {
            var checkMetaModelResult = new CheckMetaModelResult();

            foreach (var persistedCI in metaModel.GetAllClasses())
                CheckClass(currentCIs, persistedCI, checkMetaModelResult);

            for (var i = 0; i < checkMetaModelResult.Size(); i++)
            {
                var result = checkMetaModelResult.GetResults()[i];
                DLogger.Info(string.Format("MetaModelCompabilityChecker: Class {0} has changed :", result.GetFullClassName()));
                DLogger.Info("MetaModelCompabilityChecker: " + result);
            }

            if (checkMetaModelResult.GetResults().Count == 0)
                return;

            updateMetaModel();
        }

        private static void CheckClass(IDictionary<Type, ClassInfo> currentCIs, ClassInfo persistedCI, CheckMetaModelResult checkMetaModelResult)
        {
            var currentCI = currentCIs[persistedCI.UnderlyingType];
            var classInfoCompareResult = persistedCI.ExtractDifferences(currentCI, true);

            if (!classInfoCompareResult.IsCompatible())
                throw new OdbRuntimeException(NDatabaseError.IncompatibleMetamodel.AddParameter(currentCI.ToString()));

            if (classInfoCompareResult.HasCompatibleChanges())
                checkMetaModelResult.Add(classInfoCompareResult);
        }
    }
}