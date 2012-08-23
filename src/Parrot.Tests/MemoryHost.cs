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
            DependencyResolver.Register(typeof (IViewEngine), () => new ParrotViewEngine(this));
        }

        private void InitializeRendererFactory()
        {
            RendererFactory factory = new RendererFactory();

            factory.RegisterFactory(new[] { "base", "basefont", "frame", "link", "meta", "area", "br", "col", "hr", "img", "param" }, new SelfClosingRenderer(this));
            factory.RegisterFactory("doctype", new DocTypeRenderer());
            factory.RegisterFactory("rawoutput", new RawOutputRenderer());
            factory.RegisterFactory("output", new OutputRenderer());
            factory.RegisterFactory("input", new InputRenderer());
            factory.RegisterFactory("string", new StringLiteralRenderer());
            factory.RegisterFactory("layout", new LayoutRenderer(this));
            factory.RegisterFactory("content", new ContentRenderer(this));
            factory.RegisterFactory("foreach", new ForeachRenderer(this));
            factory.RegisterFactory("ul", new UlRenderer(this));

            //default renderer
            factory.RegisterFactory("*", new HtmlRenderer(this));

            DependencyResolver.Register(typeof(IRendererFactory), () => factory);
        }

    }
}
