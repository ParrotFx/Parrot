// -----------------------------------------------------------------------
// <copyright file="MemoryHost.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using Parrot.Infrastructure;
using Parrot.Mvc.Renderers;

namespace Parrot.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Web.Mvc;
    using Mvc;
    using Renderers;
    using Renderers.Infrastructure;
    using DependencyResolver = Infrastructure.DependencyResolver;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class MemoryHost : Host
    {
        public MemoryHost() : base(new DependencyResolver())
        {
            InitializeRendererFactory();
            DependencyResolver.Register(typeof(DocumentRenderer), () => new DocumentRenderer(this));
            DependencyResolver.Register(typeof(IViewEngine), () => new ParrotViewEngine(this));
            DependencyResolver.Register(typeof(IModelValueProviderFactory), () => new ModelValueProviderFactory());
        }

        private void InitializeRendererFactory()
        {
            var factory = new RendererFactory(this);

            factory.RegisterFactory("layout", new LayoutRenderer(this));
            factory.RegisterFactory("content", new ContentRenderer(this));
            factory.RegisterFactory("conditional", new ConditionalRenderer(this));

            DependencyResolver.Register(typeof(IRendererFactory), () => factory);
        }

    }
}
