// -----------------------------------------------------------------------
// <copyright file="DependencyResolver.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Parrot.Infrastructure
{
    using System;
    using System.Collections.Generic;

    public interface IDependencyResolver {
        void Register(Type type, Func<object> activator);
        object Get(Type type);
        T Get<T>() where T : class;
    }

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class DependencyResolver : IDependencyResolver
    {
        private readonly Dictionary<Type, Func<object>> _resolvers = new Dictionary<Type, Func<object>>();

        public DependencyResolver()
        {
            RegisterDefaults();
        }

        private void RegisterDefaults()
        {
            //register the default renderer
            //register the default file location view engine
            Register(typeof(IRendererFactory), () => new RendererFactory());
            Register(typeof(IValueTypeProvider), () => new ValueTypeProvider());
        }

        public virtual void Register(Type type, Func<object> activator)
        {
            if (_resolvers.ContainsKey(type))
            {
                _resolvers.Remove(type);
            }

            _resolvers.Add(type, activator);
        }

        public virtual void Register<T>(Func<object> activator )
        {
            Register(typeof (T), activator);
        }

        public virtual object Get(Type type)
        {
            if (_resolvers.ContainsKey(type))
            {
                return _resolvers[type]();
            }

            return null;
        }

        public virtual T Get<T>() where T : class
        {
            return Get(typeof(T)) as T;
        }
    }
}
