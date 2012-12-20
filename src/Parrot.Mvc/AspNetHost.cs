// -----------------------------------------------------------------------
// <copyright file="AspNetHost.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
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
            DependencyResolver.Register(typeof(IPathResolver), () => new PathResolver());
            DependencyResolver.Register(typeof(IModelValueProviderFactory), () => new ModelValueProviderFactory());

            DependencyResolver.Register(typeof(IAttributeRenderer), () => new AttributeRenderer());
            DependencyResolver.Register(typeof(IViewEngine), () => new ParrotViewEngine(this));
            DependencyResolver.Register(typeof(IParrotWriter), () => new StandardWriter());
            DependencyResolver.Register(typeof(IRendererFactory), () =>
                {
                    return new RendererFactory(new IRenderer[]
                        {
                            new HtmlRenderer(this),
                            new StringLiteralRenderer(this),
                            new DocTypeRenderer(this),
                            new LayoutRenderer(this),
                            new PartialRenderer(this),
                            new ContentRenderer(this),
                            new ForeachRenderer(this),
                            new InputRenderer(this),
                            new ConditionalRenderer(this),
                            new ListRenderer(this),
                            new SelfClosingRenderer(this)
                        });
                });
        }
    }
}
