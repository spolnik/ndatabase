using System;
using System.Reflection;

namespace NDatabase.TypeResolution
{
    /// <summary>
    /// Resolves a <see cref="System.Type"/> by name.
    /// </summary>
    internal class TypeResolver : ITypeResolver
    {
        /// <summary>
        /// Resolves the supplied <paramref name="typeName"/> to a
        /// <see cref="System.Type"/> instance.
        /// </summary>
        /// <param name="typeName">
        /// The unresolved (possibly partially assembly qualified) name 
        /// of a <see cref="System.Type"/>.
        /// </param>
        /// <returns>
        /// A resolved <see cref="System.Type"/> instance.
        /// </returns>
        /// <exception cref="System.TypeLoadException">
        /// If the supplied <paramref name="typeName"/> could not be resolved
        /// to a <see cref="System.Type"/>.
        /// </exception>
        public virtual Type Resolve(string typeName)
        {
            if (string.IsNullOrEmpty(typeName))
                throw BuildTypeLoadException(typeName);

            var typeInfo = new TypeAssemblyHolder(typeName);
            Type type;
            try
            {
                type = (typeInfo.IsAssemblyQualified) ?
                     LoadTypeDirectlyFromAssembly(typeInfo) :
                     LoadTypeByIteratingOverAllLoadedAssemblies(typeInfo);
            }
            catch (Exception ex)
            {
                if (ex is TypeLoadException)
                {
                    throw;
                }
                throw BuildTypeLoadException(typeName, ex);
            }

            return type;
        }

        /// <summary>
        /// Uses <see cref="System.Reflection.Assembly.LoadWithPartialName(string)"/>
        /// to load an <see cref="System.Reflection.Assembly"/> and then the attendant
        /// <see cref="System.Type"/> referred to by the <paramref name="typeInfo"/>
        /// parameter.
        /// </summary>
        /// <remarks>
        /// <p>
        /// <see cref="System.Reflection.Assembly.LoadWithPartialName(string)"/> is
        /// deprecated in .NET 2.0, but is still used here (even when this class is
        /// compiled for .NET 2.0);
        /// <see cref="System.Reflection.Assembly.LoadWithPartialName(string)"/> will
        /// still resolve (non-.NET Framework) local assemblies when given only the
        /// display name of an assembly (the behavior for .NET Framework assemblies
        /// and strongly named assemblies is documented in the docs for the
        /// <see cref="System.Reflection.Assembly.LoadWithPartialName(string)"/> method).
        /// </p>
        /// </remarks>
        /// <param name="typeInfo">
        /// The assembly and type to be loaded.
        /// </param>
        /// <returns>
        /// A <see cref="System.Type"/>, or <see lang="null"/>.
        /// </returns>
        /// <exception cref="System.Exception">
        /// <see cref="System.Reflection.Assembly.LoadWithPartialName(string)"/>
        /// </exception>
        private static Type LoadTypeDirectlyFromAssembly(TypeAssemblyHolder typeInfo)
        {
            Type type = null;


            var assembly = Assembly.Load(new AssemblyName(typeInfo.GetAssemblyName()));

            if (assembly != null)
                type = assembly.GetType(typeInfo.GetTypeName());

            return type;
        }

        /// <summary>
        /// Uses <see cref="M:System.AppDomain.CurrentDomain.GetAssemblies()"/>
        /// to load the attendant <see cref="System.Type"/> referred to by 
        /// the <paramref name="typeInfo"/> parameter.
        /// </summary>
        /// <param name="typeInfo">
        /// The type to be loaded.
        /// </param>
        /// <returns>
        /// A <see cref="System.Type"/>, or <see lang="null"/>.
        /// </returns>
        private static Type LoadTypeByIteratingOverAllLoadedAssemblies(TypeAssemblyHolder typeInfo)
        {
            return typeof(TypeResolver).GetTypeInfo().Assembly.GetType(typeInfo.GetTypeName());
        }

        /// <summary>
        /// Creates a new <see cref="TypeLoadException"/> instance 
        /// from the given <paramref name="typeName"/>
        /// </summary>
        protected static TypeLoadException BuildTypeLoadException(string typeName)
        {
            return new TypeLoadException("Could not load type from string value '" + typeName + "'.");
        }

        /// <summary>
        /// Creates a new <see cref="TypeLoadException"/> instance
        /// from the given <paramref name="typeName"/> with the given inner <see cref="Exception"/> 
        /// </summary>
        protected static TypeLoadException BuildTypeLoadException(string typeName, Exception ex)
        {
            return new TypeLoadException("Could not load type from string value '" + typeName + "'.", ex);
        }
    }
}