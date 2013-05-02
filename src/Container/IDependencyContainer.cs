using System;
using System.Collections.Generic;

namespace NDatabase.Container
{
    internal static class DependencyContainer
    {
        private static readonly Dictionary<Type, Func<object>> Factory = new Dictionary<Type, Func<object>>();

        internal static void Register<TInterface>(Func<object> factoryMethod)
        {
            if (Factory.ContainsKey(typeof (TInterface)))
                return;

            Factory.Add(typeof(TInterface), factoryMethod);
        }

        internal static TInterface Resolve<TInterface>()
        {
            return (TInterface)Factory[typeof (TInterface)]();
        }
    }
}