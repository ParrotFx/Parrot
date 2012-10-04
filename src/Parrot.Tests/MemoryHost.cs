// -----------------------------------------------------------------------
// <copyright file="MemoryHost.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------


namespace Parrot.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Web.Mvc;
    using Renderers;
    using Renderers.Infrastructure;
    using Parrot.Infrastructure;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class MemoryHost : Host
    {
        public MemoryHost() : base(new Infrastructure.DependencyResolver())
        {
            //InitializeRendererFactory();
            //DependencyResolver.Register(typeof(DocumentRenderer), () => new DocumentRenderer(this));
            //DependencyResolver.Register(typeof(IViewEngine), () => new ParrotViewEngine(this));
            DependencyResolver.Register(typeof(IModelValueProviderFactory), () => new ModelValueProviderFactory());
        }

        private void InitializeRendererFactory()
        {
            //var factory = new RendererFactory(this);

            //factory.RegisterFactory("layout", new LayoutRenderer(this));
            //factory.RegisterFactory("content", new ContentRenderer(this));
            //factory.RegisterFactory("conditional", new ConditionalRenderer(this));

            //DependencyResolver.Register(typeof(IRendererFactory), () => factory);
        }

    }
}
