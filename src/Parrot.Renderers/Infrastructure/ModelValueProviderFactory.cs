// -----------------------------------------------------------------------
// <copyright file="Class1.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Parrot.Renderers.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using Parrot.Infrastructure;

    public class ModelValueProviderFactory : IModelValueProviderFactory
    {
        private readonly Dictionary<Type, Func<IModelValueProvider>> _providers;

        public ModelValueProviderFactory(IValueTypeProvider valueTypeProvider)
        {
            _providers = new Dictionary<Type, Func<IModelValueProvider>>();
            _providers.Add(typeof (object), () => new ObjectModelValueProvider(valueTypeProvider));
            _providers.Add(typeof (ExpandoObject), () => new ExpandoObjectModelValueProvider(valueTypeProvider));
        }

        public void Register(Type type, Func<IModelValueProvider> provider)
        {
            if (type != null && _providers.ContainsKey(type))
            {
                _providers.Remove(type);
            }

            if (type != null)
            {
                _providers.Add(type, provider);
            }
        }


        public IModelValueProvider Get(Type type)
        {
            if (type != null && _providers.ContainsKey(type))
            {
                return _providers[type]();
            }

            return _providers[typeof (object)]();
        }
    }
}