// -----------------------------------------------------------------------
// <copyright file="Host.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Parrot.Infrastructure
{
    using System;

    /// <summary>
    /// Host context used to store the dependency resolver.
    /// </summary>
    public abstract class Host : IHost
    {
        private readonly Lazy<IDependencyResolver> _defaultResolver = new Lazy<IDependencyResolver>(() => new DependencyResolver());
        private IDependencyResolver _resolver;

        protected Host(IDependencyResolver resolver)
        {
            _resolver = resolver;
        }

        public IDependencyResolver DependencyResolver
        {
            get
            {
                return _resolver ?? _defaultResolver.Value;
            }
            set
            {
                _resolver = value;
            }
        }
    }

    /// <summary>
    /// Host interface
    /// </summary>
    public interface IHost
    {
        IDependencyResolver DependencyResolver { get; set; }
    }
}
