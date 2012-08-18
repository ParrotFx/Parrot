// -----------------------------------------------------------------------
// <copyright file="Host.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Parrot.Infrastructure
{
    using System;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class Host : IHost
    {
        private readonly Lazy<IDependencyResolver> _defaultResolver = new Lazy<IDependencyResolver>(() => new DependencyResolver());
        private IDependencyResolver _resolver;

        public Host(IDependencyResolver resolver)
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

    public interface IHost
    {
        IDependencyResolver DependencyResolver { get; set; }
    }
}
