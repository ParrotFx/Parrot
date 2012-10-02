// -----------------------------------------------------------------------
// <copyright file="AspNetHost.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System.Web.Mvc;
using Parrot.Infrastructure;
using Parrot.Mvc.Renderers;
using DependencyResolver = Parrot.Infrastructure.DependencyResolver;

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
            DependencyResolver.Register(typeof(Renderers.DocumentRenderer), () => new Renderers.DocumentRenderer(this));
            DependencyResolver.Register(typeof(IModelValueProviderFactory), () => new ModelValueProviderFactory());
            DependencyResolver.Register(typeof(IViewEngine), () => new ParrotViewEngine(this));
        }

        private void InitializeRendererFactory()
        {
            var factory = new RendererFactory(this);

            factory.RegisterFactory("layout", new LayoutRenderer(this));
            factory.RegisterFactory("partial", new PartialRenderer(this));
            factory.RegisterFactory(new[] { "base", "basefont", "frame", "link", "meta", "area", "br", "col", "hr", "img", "param" }, new Renderers.SelfClosingRenderer(this));

            factory.RegisterFactory("*", new Renderers.HtmlRenderer(this));

            DependencyResolver.Register(typeof(IRendererFactory), () => factory);
        }
    }
}
