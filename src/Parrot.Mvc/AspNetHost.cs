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
    /// TODO: Update summary.
    /// </summary>
    public class AspNetHost : Host
    {
        public AspNetHost() : base(new Infrastructure.DependencyResolver())
        {
            InitializeRendererFactory();
            DependencyResolver.Register(typeof(IPathResolver), () => new PathResolver());
            //DependencyResolver.Register(typeof(DocumentRenderer), () => new DocumentRenderer(this));
            DependencyResolver.Register(typeof(IModelValueProviderFactory), () => new ModelValueProviderFactory());

            DependencyResolver.Register(typeof(IAttributeRenderer), () => new AttributeRenderer());
            DependencyResolver.Register(typeof(IViewEngine), () => new ParrotViewEngine(this));
        }

        private void InitializeRendererFactory()
        {
            DependencyResolver.Register(typeof(IRendererFactory), () =>
            {
                var rendererFactory = new RendererFactory(this);

                rendererFactory.RegisterFactory("*", new HtmlRenderer(this, rendererFactory));
                rendererFactory.RegisterFactory("string", new StringLiteralRenderer(this, rendererFactory));
                rendererFactory.RegisterFactory("doctype", new DocTypeRenderer(this));
                rendererFactory.RegisterFactory("layout", new LayoutRenderer(this, rendererFactory));
                rendererFactory.RegisterFactory("partial", new PartialRenderer(this, rendererFactory));
                rendererFactory.RegisterFactory("content", new ContentRenderer(this, rendererFactory));
                rendererFactory.RegisterFactory("foreach", new ForeachRenderer(this, rendererFactory));
                rendererFactory.RegisterFactory("conditional", new ConditionalRenderer(this, rendererFactory));
                rendererFactory.RegisterFactory(new[] { "ul", "ol" }, new ListRenderer(this, rendererFactory));
                rendererFactory.RegisterFactory(
                    new[] { "base", "basefont", "frame", "link", "meta", "area", "br", "col", "hr", "img", "param" },
                    new SelfClosingRenderer(this, rendererFactory)
                );

                return rendererFactory;
            });
        }
    }
}
