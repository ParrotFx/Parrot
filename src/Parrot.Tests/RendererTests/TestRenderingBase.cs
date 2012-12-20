// -----------------------------------------------------------------------
// <copyright file="TestRenderingBase.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System.IO;
using Parrot.Infrastructure;
using Parrot.Mvc.Renderers;
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

            var writer = host.DependencyResolver.Resolve<IParrotWriter>();
            documentView.Render(writer);

            return writer.Result();
            //return renderer.Render(document, model);
        }

        protected string Render(string parrot, object documentHost)
        {
            return Render(parrot, new Dictionary<string, object> { { "Model", documentHost } });
        }

        protected string Render(string parrot, IDictionary<string, object> documentHost)
        {
            return Render(parrot, documentHost, new MemoryHost());
        }

        protected string Render(string parrot)
        {
            return Render(parrot, new Dictionary<string, object>(), new MemoryHost());
        }

        protected string Render(string parrot, IHost host)
        {
            return Render(parrot, new Dictionary<string, object>(), host);
        }
    }
}
