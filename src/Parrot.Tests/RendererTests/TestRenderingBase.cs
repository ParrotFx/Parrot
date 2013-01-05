// -----------------------------------------------------------------------
// <copyright file="TestRenderingBase.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Parrot.Tests
{
    using System.Collections.Generic;
    using Parrot.Mvc;
    using Parrot.Mvc.Renderers;
    using Parrot.Nodes;
    using Parrot.Renderers;
    using Parrot.Renderers.Infrastructure;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class TestRenderingBase
    {
        protected string Render(string parrot, IDictionary<string, object> documentHost, AspNetHost host)
        {
            Parser.Parser parser = new Parser.Parser();
            Document document;

            parser.Parse(parrot, out document);

            var rendererFactory = new RendererFactory(new IRenderer[]
                {
                    new HtmlRenderer(host),
                    new StringLiteralRenderer(host),
                    new DocTypeRenderer(host),
                    new LayoutRenderer(host),
                    new PartialRenderer(host),
                    new ContentRenderer(host),
                    new ForeachRenderer(host),
                    new InputRenderer(host),
                    new ConditionalRenderer(host),
                    new ListRenderer(host),
                    new SelfClosingRenderer(host)
                });

            DocumentView documentView = new DocumentView(new MemoryHost(), rendererFactory, documentHost, document);

            var writer = host.CreateWriter();
            documentView.Render(writer);

            return writer.Result();
            //return renderer.Render(document, model);
        }

        protected string Render(string parrot, object documentHost)
        {
            return Render(parrot, new Dictionary<string, object> {{"Model", documentHost}});
        }

        protected string Render(string parrot, IDictionary<string, object> documentHost)
        {
            return Render(parrot, documentHost, new MemoryHost());
        }

        protected string Render(string parrot)
        {
            return Render(parrot, new Dictionary<string, object>(), new MemoryHost());
        }

        protected string Render(string parrot, AspNetHost host)
        {
            return Render(parrot, new Dictionary<string, object>(), host);
        }
    }
}