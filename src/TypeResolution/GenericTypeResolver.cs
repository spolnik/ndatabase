using System;

namespace NDatabase.TypeResolution
{
    /// <summary>
    /// Resolves a generic <see cref="System.Type"/> by name.
    /// </summary>
    internal class GenericTypeResolver : TypeResolver
    {
        /// <summary>
        /// Resolves the supplied generic <paramref name="typeName"/> to a
        /// <see cref="System.Type"/> instance.
        /// </summary>
        /// <param name="typeName">
        /// The unresolved (possibly generic) name of a <see cref="System.Type"/>.
        /// </param>
        /// <returns>
        /// A resolved <see cref="System.Type"/> instance.
        /// </returns>
        /// <exception cref="System.TypeLoadException">
        /// If the supplied <paramref name="typeName"/> could not be resolved
        /// to a <see cref="System.Type"/>.
        /// </exception>
        public override Type Resolve(string typeName)
        {
            if (string.IsNullOrEmpty(typeName))
                throw BuildTypeLoadException(typeName);

            var genericInfo = new GenericArgumentsHolder(typeName);
            Type type = null;
            try
            {
                if (genericInfo.ContainsGenericArguments)
                {
                    type = TypeResolutionUtils.ResolveType(genericInfo.GenericTypeName);
                    if (!genericInfo.IsGenericDefinition)
                    {
                        string[] unresolvedGenericArgs = genericInfo.GetGenericArguments();
                        Type[] genericArgs = new Type[unresolvedGenericArgs.Length];
                        for (int i = 0; i < unresolvedGenericArgs.Length; i++)
                        {
                            genericArgs[i] = TypeResolutionUtils.ResolveType(unresolvedGenericArgs[i]);
                        }
                        type = type.MakeGenericType(genericArgs);
                    }
                    if (genericInfo.IsArrayDeclaration)
                    {
                        typeName = string.Format("{0}{1},{2}", type.FullName, genericInfo.GetArrayDeclaration(), type.Assembly.FullName);
                        type = null;
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex is TypeLoadException)
                {
                    throw;
                }
                throw BuildTypeLoadException(typeName, ex);
            }

            if (type == null)
            {
                type = base.Resolve(typeName);
            }

            return type;
        }
    }
}
