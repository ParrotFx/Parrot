// -----------------------------------------------------------------------
// <copyright file="TestRenderingBase.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System.IO;
using Parrot.Infrastructure;
using Parrot.Nodes;
using Parrot.Renderers.Infrastructure;

namespace Parrot.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Renderers;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class TestRenderingBase
    {
        protected string Render(string parrot, IDictionary<string, object> documentHost, IHost host)
        {
            Parser.Parser parser = new Parser.Parser(host);
            Document document;

            parser.Parse(parrot, out document);

            var rendererFactory = new RendererFactory(host);
            rendererFactory.RegisterFactory("*", new HtmlRenderer(host, rendererFactory));
            rendererFactory.RegisterFactory("string", new StringLiteralRenderer(host, rendererFactory));
            rendererFactory.RegisterFactory("doctype", new DocTypeRenderer(host));
            rendererFactory.RegisterFactory("foreach", new ForeachRenderer(host, rendererFactory));
            rendererFactory.RegisterFactory("conditional", new ConditionalRenderer(host, rendererFactory));
            rendererFactory.RegisterFactory(new[] { "ul", "ol" }, new ListRenderer(host, rendererFactory));
            rendererFactory.RegisterFactory(
                new[] { "base", "basefont", "frame", "link", "meta", "area", "br", "col", "hr", "img", "param" }, 
                new SelfClosingRenderer(host, rendererFactory)
            );

            DocumentView documentView = new DocumentView(new MemoryHost(), rendererFactory, documentHost, document);

            StringBuilder sb = new StringBuilder();
            StringWriter writer = new StringWriter(sb);
            documentView.Render(writer);

            return sb.ToString();
            //return renderer.Render(document, model);
        }

        protected string Render(string parrot, object documentHost)
        {
            return Render(parrot, new Dictionary<string, object> { {"Model", documentHost }});
        }

        protected string Render(string parrot, IDictionary<string, object> documentHost)
        {
            return Render(parrot, documentHost, new MemoryHost());
        }

        protected string Render(string parrot)
        {
            return Render(parrot, new Dictionary<string, object>(), new MemoryHost());
        }

    }
}
