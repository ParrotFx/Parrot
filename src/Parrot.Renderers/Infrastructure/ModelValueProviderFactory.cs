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
    using System.Text;

    public class ModelValueProviderFactory : IModelValueProviderFactory
    {
        readonly Dictionary<Type, Func<IModelValueProvider>> _providers;

        public ModelValueProviderFactory()
        {
            _providers = new Dictionary<Type, Func<IModelValueProvider>>();
            _providers.Add(typeof (object), () => new ObjectModelValueProvider());
            _providers.Add(typeof(ExpandoObject), () => new ExpandoObjectModelValueProvider());
        }

        public void Register(Type type, Func<IModelValueProvider> provider)
        {
            if (_providers.ContainsKey(type))
            {
                _providers.Remove(type);
            }

            _providers.Add(type, provider);
        }



        public IModelValueProvider Get(Type type)
        {
            if (_providers.ContainsKey(type))
            {
                return _providers[type]();
            }

            return _providers[typeof(object)]();
        }
    }
}
