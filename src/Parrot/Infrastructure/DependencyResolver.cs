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
        object Resolve(Type type);
        T Resolve<T>() where T : class;
    }

    /// <summary>
    /// The default dependency resolver for Parrot
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
            Register(typeof(IValueTypeProvider), () => new ValueTypeProvider());
        }

        /// <summary>
        /// Registers a type and activator with the dependency resolver
        /// </summary>
        /// <param name="type">Type you're registering</param>
        /// <param name="activator">Activator used to generate the type</param>
        public virtual void Register(Type type, Func<object> activator)
        {
            if (_resolvers.ContainsKey(type))
            {
                _resolvers.Remove(type);
            }

            _resolvers.Add(type, activator);
        }

        /// <summary>
        /// Registers a type via generics with the dependency resolver
        /// </summary>
        /// <typeparam name="T">Type you're registering</typeparam>
        /// <param name="activator">Activator used to generate the type</param>
        public virtual void Register<T>(Func<object> activator)
        {
            Register(typeof (T), activator);
        }

        /// <summary>
        /// Executes the activator registered for a certain type. 
        /// </summary>
        /// <param name="type">Type you're attempting to resolve</param>
        /// <returns>Returns null if an activator for a specific type isn't registered</returns>
        public virtual object Resolve(Type type)
        {
            if (_resolvers.ContainsKey(type))
            {
                return _resolvers[type]();
            }

            return null;
        }

        /// <summary>
        /// Executes the activator registered for a certain type. 
        /// </summary>
        /// <typeparam name="T">Type you're attempting to resolve</typeparam>
        /// <returns>Returns null if an activator for a specific type isn't registered</returns>
        public virtual T Resolve<T>() where T : class
        {
            return Resolve(typeof(T)) as T;
        }
    }
}
