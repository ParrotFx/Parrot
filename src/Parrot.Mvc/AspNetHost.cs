// -----------------------------------------------------------------------
// <copyright file="AspNetHost.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using Parrot.Infrastructure;
using Parrot.Mvc.Renderers;

namespace Parrot.Mvc
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Parrot.Renderers;
    using Parrot.Renderers.Infrastructure;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class AspNetHost : Host
    {
        public AspNetHost() : base(new DependencyResolver())
        {
            InitializeRendererFactory();
            DependencyResolver.Register(typeof(IPathResolver), () => new PathResolver());
            DependencyResolver.Register(typeof(DocumentRenderer), () => new DocumentRenderer(this));
        }

        private void InitializeRendererFactory()
        {
            var factory = new RendererFactory(this);

            factory.RegisterFactory("layout", new LayoutRenderer(this));
            factory.RegisterFactory("content", new ContentRenderer(this));

            DependencyResolver.Register(typeof(IRendererFactory), () => factory);
        }
    }
}
