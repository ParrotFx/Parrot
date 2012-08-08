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
    public static class Host
    {
        private static readonly Lazy<IDependencyResolver> DefaultResolver = new Lazy<IDependencyResolver>(() => new DependencyResolver());
        
        private static IDependencyResolver _resolver;
        public static IDependencyResolver DependencyResolver
        {
            get
            {
                return _resolver ?? DefaultResolver.Value;
            }
            set
            {
                _resolver = value;
            }
        }
    }
}
