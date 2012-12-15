// -----------------------------------------------------------------------
// <copyright file="AspNetHost.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using Parrot.Renderers;

namespace Parrot.Mvc
{
    using System.Web.Mvc;

    using Infrastructure;
    using Renderers;
    using Parrot.Renderers.Infrastructure;

    /// <summary>
    /// Asp.Net host for Parrot
    /// </summary>
    public class AspNetHost : Host
    {
        public AspNetHost() : base(new Infrastructure.DependencyResolver())
        {
            InitializeRendererFactory();
            DependencyResolver.Register(typeof(IPathResolver), () => new PathResolver());
            DependencyResolver.Register(typeof(IModelValueProviderFactory), () => new ModelValueProviderFactory());

            DependencyResolver.Register(typeof(IAttributeRenderer), () => new AttributeRenderer());
            DependencyResolver.Register(typeof(IViewEngine), () => new ParrotViewEngine(this));
            DependencyResolver.Register(typeof(IParrotWriter), () => new StandardWriter());
        }

        private void InitializeRendererFactory()
        {
            DependencyResolver.Register(typeof(IRendererFactory), () =>
            {
                var rendererFactory = new RendererFactory(this);

                rendererFactory.RegisterFactory("*", new HtmlRenderer(this));
                rendererFactory.RegisterFactory("string", new StringLiteralRenderer(this));
                rendererFactory.RegisterFactory("doctype", new DocTypeRenderer(this));
                rendererFactory.RegisterFactory("layout", new LayoutRenderer(this));
                rendererFactory.RegisterFactory("partial", new PartialRenderer(this));
                rendererFactory.RegisterFactory("content", new ContentRenderer(this));
                rendererFactory.RegisterFactory("foreach", new ForeachRenderer(this));
                rendererFactory.RegisterFactory("input", new InputRenderer(this));
                rendererFactory.RegisterFactory("conditional", new ConditionalRenderer(this));
                rendererFactory.RegisterFactory(new[] { "ul", "ol" }, new ListRenderer(this));
                rendererFactory.RegisterFactory(
                    new[] { "base", "basefont", "frame", "link", "meta", "area", "br", "col", "hr", "img", "param" },
                    new SelfClosingRenderer(this)
                );

                return rendererFactory;
            });
        }
    }
}
